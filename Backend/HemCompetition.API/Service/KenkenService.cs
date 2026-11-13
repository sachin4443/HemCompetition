using HemCompetition.API.DTOs;
using HemCompetition.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HemCompetition.API.Services
{
    public class KenkenService : IKenkenService
    {
        private readonly Hemvirtues2627Context _context;

        public KenkenService(Hemvirtues2627Context context)
        {
            _context = context;
        }
        public async Task<List<KenkenQuestionDto>> GetActiveQuestionsAsync(long userId, CancellationToken cancellationToken)
        {
            var answeredKcmids = await _context.KenCompUserAnswers
                .AsNoTracking()
                .Where(u => u.UserId == (int)userId && u.Kcmid.HasValue)
                .Select(u => u.Kcmid!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            var masterGames = await _context.KenCompMasters
                .AsNoTracking()
                .Where(m => (m.IsActive == "1" || m.IsActive == "true" || m.IsActive == "TRUE")
                         && !answeredKcmids.Contains(m.Kcmid))
                .OrderBy(m => m.Kcmid)
                .Take(5)
                .ToListAsync(cancellationToken);

            var questionsList = new List<KenkenQuestionDto>();

            foreach (var master in masterGames)
            {
                int size = 3;
                if (master.Size != null && int.TryParse(master.Size.ToString(), out int parsedSize))
                {
                    size = parsedSize;
                }

                var cages = await _context.KenCompCages
                    .AsNoTracking()
                    .Where(c => c.Kcmid == master.Kcmid)
                    .OrderBy(c => c.Kccid)
                    .Take(size)
                    .ToListAsync(cancellationToken);

                var targets = await _context.KenCompTargets
                    .AsNoTracking()
                    .Where(t => t.Kcmid == master.Kcmid)
                    .OrderBy(t => t.Kctid)
                    .Take(size)
                    .ToListAsync(cancellationToken);

                if (cages.Count < size || targets.Count < size) continue;

                string[][] cageGrid = new string[size][];
                string[][] targetGrid = new string[size][];

                for (int i = 0; i < size; i++)
                {
                    cageGrid[i] = new string[] {
                        cages[i].Cage1 ?? "", cages[i].Cage2 ?? "", cages[i].Cage3 ?? ""
                    };

                    targetGrid[i] = new string[] {
                        CleanTarget(targets[i].Target1), CleanTarget(targets[i].Target2), CleanTarget(targets[i].Target3)
                    };
                }

                questionsList.Add(new KenkenQuestionDto
                {
                    KCMID = master.Kcmid,
                    Size = size,
                    CageGrid = cageGrid,
                    TargetGrid = targetGrid
                });
            }

            return questionsList;
        }
        public async Task<ApiResponseDto> ValidateAndSaveAnswerAsync(SaveKenkenAnswerDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserAnswer))
            {
                return new ApiResponseDto { Success = false, Message = "Invalid payload." };
            }

            var solutions = await _context.KenCompSolutions
                .AsNoTracking()
                .Where(s => s.Kcmid == dto.KCMID)
                .OrderBy(s => s.Kcsid)
                .ToListAsync(cancellationToken);

            bool isCorrect = false;

            if (solutions.Count >= 3)
            {
                string dbSolution = $"{solutions[0].S1}{solutions[0].S2}{solutions[0].S3}" +
                                   $"{solutions[1].S1}{solutions[1].S2}{solutions[1].S3}" +
                                   $"{solutions[2].S1}{solutions[2].S2}{solutions[2].S3}";

                if (dto.UserAnswer.Trim() == dbSolution.Trim())
                {
                    isCorrect = true;
                }
            }

            if (!isCorrect)
            {
                return new ApiResponseDto { Success = false, Message = "❌ Incorrect solution! Rules mismatch." };
            }

            string timeDiff = "0";
            int totalSeconds = 0;

            try
            {
                string NormalizeJsDate(string dateStr)
                {
                    if (string.IsNullOrEmpty(dateStr)) return null;
                    if (dateStr.Contains("("))
                    {
                        dateStr = dateStr.Substring(0, dateStr.IndexOf("(")).Trim();
                    }
                    if (dateStr.Contains("GMT"))
                    {
                        dateStr = dateStr.Replace("GMT", "");
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
            catch(Exception ex)
{
                Console.WriteLine("Date parse error: " + ex.Message);
            }

            string timeOffStr = totalSeconds.ToString();
            var record = new KenCompUserAnswer
            {
                Kcmid = dto.KCMID,
                UserId = (int)dto.UserId,
                UserAnswer = "1",
                Remark = null,
                StartTime = dto.StartTime ?? DateTime.Now.ToString(),
                EndTime = dto.EndTime ?? DateTime.Now.ToString(),
                TimeDiff = timeOffStr,
                CourseId = null
            };

            await _context.KenCompUserAnswers.AddAsync(record, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto
            {
                Success = true,
                Message = "🎉 Correct Answer Saved!",
                Data = new { IsLast = dto.IsLastQuestion }
            };
        }
        public async Task<ApiResponseDto> SkipQuestionAsync(SkipKenkenDto dto, CancellationToken cancellationToken)
        {
            string startStr = string.IsNullOrWhiteSpace(dto.StartTime) ? DateTime.Now.ToString() : dto.StartTime;
            string endStr = string.IsNullOrWhiteSpace(dto.EndTime) ? DateTime.Now.ToString() : dto.EndTime;

            int totalSeconds = 0;

            try
            {
                string NormalizeJsDate(string dateStr)
                {
                    if (string.IsNullOrEmpty(dateStr)) return null;

                    if (dateStr.Contains("("))
                    {
                        dateStr = dateStr.Substring(0, dateStr.IndexOf("(")).Trim();
                    }

                    if (dateStr.Contains("GMT"))
                    {
                        dateStr = dateStr.Replace("GMT", "");
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

            // FIX FOR CS0029: Set UserAnswer as string "0"
            var skipRecord = new KenCompUserAnswer
            {
                Kcmid = dto.KCMID,
                UserId = (int)dto.UserId,
                UserAnswer = "0",
                Remark = "Skipped",
                StartTime = startStr,
                EndTime = endStr,
                TimeDiff = timeOffStr,
                CourseId = null
            };

            await _context.KenCompUserAnswers.AddAsync(skipRecord, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto { Success = true, Message = "Question skipped." };
        }
        private static string CleanTarget(object? val)
        {
            if (val == null) return string.Empty;
            string s = val.ToString()?.Trim() ?? string.Empty;
            return (s == "NULL" || s == "0" || s == "&nbsp;") ? string.Empty : s;
        }
    }
}