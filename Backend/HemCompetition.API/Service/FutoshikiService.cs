using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HemCompetition.API.DTOs;
using HemCompetition.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services
{
    public class FutoshikiService : IFutoshikiService
    {
        private readonly Hemvirtues2627Context _context;

        public FutoshikiService(Hemvirtues2627Context context)
        {
            _context = context;
        }
        public async Task<List<FutoshikiQuestionDto>> GetActiveQuestionsAsync(long userId, CancellationToken cancellationToken)
        {
            var answeredFcids = await _context.FutoshikiUserAnswers
                .AsNoTracking()
                .Where(u => u.UserId == (int)userId && u.Fcid.HasValue)
                .Select(u => u.Fcid!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            var masterGames = await _context.FutoshikiCompetitionMasters
                .AsNoTracking()
                .Where(m => (m.IsActive == "1" || m.IsActive == "true" || m.IsActive == "TRUE")
                         && !answeredFcids.Contains(m.Fcmid))
                .OrderBy(m => m.Fcmid)
                .Take(5)
                .ToListAsync(cancellationToken);

            if (!masterGames.Any())
                return new List<FutoshikiQuestionDto>();

            var fcmids = masterGames.Select(m => m.Fcmid).ToList();

            var allTargets = await _context.FutoshikiCompetitionTargets
                .AsNoTracking()
                .Where(t => fcmids.Contains(t.Fcmid))
                .OrderBy(t => t.Fctid)
                .ToListAsync(cancellationToken);

            var questionsList = new List<FutoshikiQuestionDto>();

            foreach (var master in masterGames)
            {
                var targets = allTargets.Where(t => t.Fcmid == master.Fcmid).OrderBy(t => t.Fctid).ToList();
                if (targets.Count < 7) continue;

                int[][] initialGrid = new int[4][];
                string[][] hSigns = new string[4][];
                string[][] vSigns = new string[3][];

                initialGrid[0] = new int[] { ParseInt(targets[0].Target1), ParseInt(targets[0].Target3), ParseInt(targets[0].Target5), ParseInt(targets[0].Target6) };
                hSigns[0] = new string[] { CleanSign(targets[0].Target2), CleanSign(targets[0].Target4), CleanSign(targets[0].Target6) };

                initialGrid[1] = new int[] { ParseInt(targets[2].Target1), ParseInt(targets[2].Target3), ParseInt(targets[2].Target5), ParseInt(targets[2].Target6) };
                hSigns[1] = new string[] { CleanSign(targets[2].Target2), CleanSign(targets[2].Target4), CleanSign(targets[2].Target6) };

                initialGrid[2] = new int[] { ParseInt(targets[4].Target1), ParseInt(targets[4].Target3), ParseInt(targets[4].Target5), ParseInt(targets[4].Target6) };
                hSigns[2] = new string[] { CleanSign(targets[4].Target2), CleanSign(targets[4].Target4), CleanSign(targets[4].Target6) };

                initialGrid[3] = new int[] { ParseInt(targets[6].Target1), ParseInt(targets[6].Target3), ParseInt(targets[6].Target5), ParseInt(targets[6].Target6) };
                hSigns[3] = new string[] { CleanSign(targets[6].Target2), CleanSign(targets[6].Target4), CleanSign(targets[6].Target6) };

                vSigns[0] = new string[] { CleanSign(targets[1].Target1), CleanSign(targets[1].Target3), CleanSign(targets[1].Target5), CleanSign(targets[1].Target6) };
                vSigns[1] = new string[] { CleanSign(targets[3].Target1), CleanSign(targets[3].Target3), CleanSign(targets[3].Target5), CleanSign(targets[3].Target6) };
                vSigns[2] = new string[] { CleanSign(targets[5].Target1), CleanSign(targets[5].Target3), CleanSign(targets[5].Target5), CleanSign(targets[5].Target6) };

                questionsList.Add(new FutoshikiQuestionDto
                {
                    FCID = master.Fcmid,
                    InitialGrid = initialGrid,
                    HSigns = hSigns,
                    VSigns = vSigns
                });
            }

            return questionsList;
        }

        public async Task<ApiResponseDto> ValidateAndSaveAnswerAsync(SaveFutoshikiAnswerDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserAnswer))
            {
                return new ApiResponseDto { Success = false, Message = "Invalid answer payload provided." };
            }

            _context.Database.SetCommandTimeout(60);

            var solutions = await _context.FutoshikiCompetitionSolutions
                .AsNoTracking()
                .Where(s => s.Fcmid == dto.FCID)
                .OrderBy(s => s.Fcsid)
                .ToListAsync(cancellationToken);

            bool isCorrect = false;

            if (solutions.Count == 4)
            {
                string dbSolution = $"{solutions[0].S1}{solutions[0].S2}{solutions[0].S3}{solutions[0].S4}" +
                                   $"{solutions[1].S1}{solutions[1].S2}{solutions[1].S3}{solutions[1].S4}" +
                                   $"{solutions[2].S1}{solutions[2].S2}{solutions[2].S3}{solutions[2].S4}" +
                                   $"{solutions[3].S1}{solutions[3].S2}{solutions[3].S3}{solutions[3].S4}";

                if (dto.UserAnswer.Trim() == dbSolution.Trim())
                {
                    isCorrect = true;
                }
            }

            if (!isCorrect)
            {
                return new ApiResponseDto { Success = false, Message = "❌ Incorrect solution. Please check your matrix!" };
            }

            int totalSeconds = 0;

            try
            {
                // Clean string: "Thu Aug 20 2026 13:18:29 GMT+0530" -> convert GMT+0530 to +05:30
                // C# expects the offset to have a colon (+05:30 instead of +0530)
                string NormalizeJsDate(string dateStr)
                {
                    if (string.IsNullOrEmpty(dateStr)) return null;

                    // Remove parenthetical timezone name if present
                    if (dateStr.Contains("("))
                    {
                        dateStr = dateStr.Substring(0, dateStr.IndexOf("(")).Trim();
                    }

                    // Replace "GMT+0530" with "+05:30" format for standard parsing
                    if (dateStr.Contains("GMT"))
                    {
                        dateStr = dateStr.Replace("GMT", "");
                        // Insert colon in timezone offset if it's 4 digits long (e.g., +0530 -> +05:30)
                        int plusIndex = dateStr.LastIndexOf('+');
                        int minusIndex = dateStr.LastIndexOf('-');
                        int tzIndex = plusIndex > -1 ? plusIndex : minusIndex;

                        if (tzIndex > -1 && dateStr.Length - tzIndex == 5)
                        {
                            dateStr = dateStr.Insert(tzIndex + 3, ":");
                        }
                    }
                    return dateStr;
                }

                string cleanStart = NormalizeJsDate(dto.StartTime);
                string cleanEnd = NormalizeJsDate(dto.EndTime);

                if (DateTimeOffset.TryParse(cleanStart, out DateTimeOffset startOffset) &&
                    DateTimeOffset.TryParse(cleanEnd, out DateTimeOffset endOffset))
                {
                    totalSeconds = Math.Max(0, (int)(endOffset - startOffset).TotalSeconds);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Date parse error: " + ex.Message);
            }

            string timeOffStr = totalSeconds.ToString();


            var userAnswerRecord = new FutoshikiUserAnswer
            {
                Fcid = (int)dto.FCID,
                UserId = (int)dto.UserId,
                UserAnswer = 1,     // Submit hone par UserAnswer = 1
                Remark = null,      // Submit hone par Remark = NULL
                StartTime = dto.StartTime ?? DateTime.Now.ToString(),
                EndTime = dto.EndTime ?? DateTime.Now.ToString(),
                TimeDiff = timeOffStr
            };

            await _context.FutoshikiUserAnswers.AddAsync(userAnswerRecord, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto
            {
                Success = true,
                Message = "🎉 Correct Answer Saved!",
                Data = new { IsLast = dto.IsLastQuestion }
            };
        }

        public async Task<ApiResponseDto> SkipQuestionAsync(SkipFutoshikiDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || dto.FCID <= 0 || dto.UserId <= 0)
            {
                return new ApiResponseDto { Success = false, Message = "Invalid Skip Request." };
            }

            _context.Database.SetCommandTimeout(60);

            string startStr = string.IsNullOrWhiteSpace(dto.StartTime) ? DateTime.Now.ToString() : dto.StartTime;
            string endStr = string.IsNullOrWhiteSpace(dto.EndTime) ? DateTime.Now.ToString() : dto.EndTime;



            int totalSeconds = 0;

            try
            {
                // Clean string: "Thu Aug 20 2026 13:18:29 GMT+0530" -> convert GMT+0530 to +05:30
                // C# expects the offset to have a colon (+05:30 instead of +0530)
                string NormalizeJsDate(string dateStr)
                {
                    if (string.IsNullOrEmpty(dateStr)) return null;

                    // Remove parenthetical timezone name if present
                    if (dateStr.Contains("("))
                    {
                        dateStr = dateStr.Substring(0, dateStr.IndexOf("(")).Trim();
                    }

                    // Replace "GMT+0530" with "+05:30" format for standard parsing
                    if (dateStr.Contains("GMT"))
                    {
                        dateStr = dateStr.Replace("GMT", "");
                        // Insert colon in timezone offset if it's 4 digits long (e.g., +0530 -> +05:30)
                        int plusIndex = dateStr.LastIndexOf('+');
                        int minusIndex = dateStr.LastIndexOf('-');
                        int tzIndex = plusIndex > -1 ? plusIndex : minusIndex;

                        if (tzIndex > -1 && dateStr.Length - tzIndex == 5)
                        {
                            dateStr = dateStr.Insert(tzIndex + 3, ":");
                        }
                    }
                    return dateStr;
                }

                string cleanStart = NormalizeJsDate(dto.StartTime);
                string cleanEnd = NormalizeJsDate(dto.EndTime);

                if (DateTimeOffset.TryParse(cleanStart, out DateTimeOffset startOffset) &&
                    DateTimeOffset.TryParse(cleanEnd, out DateTimeOffset endOffset))
                {
                    totalSeconds = Math.Max(0, (int)(endOffset - startOffset).TotalSeconds);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Date parse error: " + ex.Message);
            }

            string timeOffStr = totalSeconds.ToString();


            var skipRecord = new FutoshikiUserAnswer
            {
                Fcid = (int)dto.FCID,
                UserId = (int)dto.UserId,
                UserAnswer = 0,          // Skip hone par UserAnswer = 0
                Remark = "Skipped",       // Skip hone par Remark = "Skipped"
                StartTime = startStr,
                EndTime = endStr,
                TimeDiff = timeOffStr
            };

            await _context.FutoshikiUserAnswers.AddAsync(skipRecord, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto { Success = true, Message = "Question skipped and stored successfully." };
        }

        #region Helper Methods
        private static int ParseInt(object? val)
        {
            if (val == null) return 0;
            return int.TryParse(val.ToString()?.Trim(), out int res) ? res : 0;
        }

        private static string CleanSign(object? val)
        {
            if (val == null) return string.Empty;
            string str = val.ToString()?.Trim() ?? string.Empty;
            return (str == "&nbsp;" || str == "nbsp;") ? string.Empty : str;
        }
        #endregion
    }
}