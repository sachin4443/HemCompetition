using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers;

[ApiController]
[Route("api/competition")]
public class CompetitionInfoController(CompetitionService competitions) : ControllerBase
{
    /// <summary>Public: lets the login page show whether the competition is upcoming, live or over.</summary>
    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<IActionResult> Active()
    {
        var c = await competitions.GetActiveAsync();
        var now = DateTime.UtcNow;
        if (c == null) return Ok(new { status = "NONE", serverTimeUtc = now });
        return Ok(new
        {
            id = c.Id,
            competitionName = c.CompetitionName,
            startDateTimeUtc = c.StartDateTimeUtc,
            endDateTimeUtc = c.EndDateTimeUtc,
            serverTimeUtc = now,
            status = CompetitionService.PhaseOf(c, now) switch
            {
                CompetitionPhase.NotStarted => "NOT_STARTED", CompetitionPhase.Live => "LIVE", _ => "ENDED"
            }
        });
    }
}

[ApiController]
[Route("api/student")]
[Authorize(Policy = Policies.Student)]
public class StudentController(
    AppDbContext db, CompetitionService competitions, LeaderboardService leaderboard, SettingsService settingsService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var studentId = User.StudentId();
        var user = await db.AuthUsers.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == studentId) ?? throw ApiException.NotFound();
        var c = await competitions.GetActiveAsync();
        var now = DateTime.UtcNow;
        var unread = await db.Notifications.CountAsync(n => (n.StudentId == null || n.StudentId == studentId) &&
            !db.NotificationReads.Any(r => r.NotificationId == n.Id && r.StudentId == studentId));
        var settings = await settingsService.GetAsync();
        return Ok(new
        {
            profile = new
            {
                userId = user.UserId, username = user.Username, firstName = user.FirstName, lastName = user.LastName,
                email = user.EmailId ?? user.Email, mobileNo = user.PhoneNumber, address = user.AddressLine1,
                city = user.City, pincode = user.Pincode, lastLoginAtUtc = user.LastLoginAtUtc
            },
            competition = c == null ? null : new
            {
                id = c.Id, name = c.CompetitionName, description = c.Description,
                startDateTimeUtc = c.StartDateTimeUtc, endDateTimeUtc = c.EndDateTimeUtc,
                phase = CompetitionService.PhaseOf(c, now).ToString(),
                showResults = c.ShowResultsToStudents, showLeaderboard = c.ShowLeaderboard,
                proctoringRequired = c.ProctoringRequired
            },
            serverTimeUtc = now,
            accountStatus = "Active",
            unreadNotifications = unread,
            supportContact = settings.SupportContact
        });
    }

    /// <summary>Every attempt the student has made. Scores appear only when the competition publishes results.</summary>
    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        var studentId = User.StudentId();
        var games = await db.GameAttempts.AsNoTracking().Where(a => a.StudentId == studentId)
            .Select(a => new
            {
                a.Id, Kind = "game", Section = a.Game.Category.Name, Name = a.Game.Name, a.Status, a.StartedAtUtc,
                FinishedAtUtc = a.CompletedAtUtc, a.Score, a.MaxScore, Correct = a.CorrectCount, Total = a.TotalRounds,
                a.TimeTakenSeconds, a.Competition.CompetitionName, a.Competition.ShowResultsToStudents
            }).ToListAsync();
        var quizzes = await db.QuizAttempts.AsNoTracking().Where(a => a.StudentId == studentId)
            .Select(a => new
            {
                a.Id, Kind = "quiz", Section = "Mega Quiz", Name = a.QuizTitle, a.Status, a.StartedAtUtc,
                FinishedAtUtc = a.SubmittedAtUtc, a.Score, a.MaxScore, Correct = a.CorrectCount, Total = a.Questions.Count,
                a.TimeTakenSeconds, a.Competition.CompetitionName, a.Competition.ShowResultsToStudents
            }).ToListAsync();

        return Ok(games.Concat(quizzes).OrderByDescending(a => a.StartedAtUtc).Select(a =>
        {
            var show = a.ShowResultsToStudents && a.Status != AttemptStatus.InProgress;
            return new
            {
                id = a.Id, kind = a.Kind, section = a.Section, name = a.Name, competition = a.CompetitionName,
                status = a.Status.ToString(), startedAtUtc = a.StartedAtUtc, finishedAtUtc = a.FinishedAtUtc,
                timeTakenSeconds = a.TimeTakenSeconds, total = a.Total,
                score = show ? a.Score : (decimal?)null, maxScore = show ? a.MaxScore : (decimal?)null,
                correct = show ? a.Correct : (int?)null
            };
        }));
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> Leaderboard()
    {
        var c = await competitions.GetActiveAsync();
        if (c is not { ShowLeaderboard: true }) return Ok(new { visible = false });
        var settings = await settingsService.GetAsync();
        var rows = await leaderboard.BuildAsync(c.Id);
        var studentId = User.StudentId();
        var top = rows.Take(settings.LeaderboardSize).ToList();
        var mine = rows.FirstOrDefault(r => r.StudentId == studentId);
        await leaderboard.FillNamesAsync(mine == null || top.Contains(mine) ? top : [.. top, mine]);
        object Shape(LeaderboardRow r) => new
        {
            rank = r.Rank, name = r.Name, city = r.City, totalScore = r.TotalScore,
            timeTakenSeconds = r.TimeTakenSeconds, isMe = r.StudentId == studentId
        };
        return Ok(new { visible = true, participants = rows.Count, top = top.Select(Shape), me = mine == null ? null : Shape(mine) });
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> Notifications()
    {
        var studentId = User.StudentId();
        var items = await db.Notifications.AsNoTracking().Where(n => n.StudentId == null || n.StudentId == studentId)
            .OrderByDescending(n => n.CreatedAtUtc).Take(50)
            .Select(n => new
            {
                id = n.Id, title = n.Title, body = n.Body, category = n.Category, createdAtUtc = n.CreatedAtUtc,
                read = db.NotificationReads.Any(r => r.NotificationId == n.Id && r.StudentId == studentId)
            }).ToListAsync();
        return Ok(items);
    }

    [HttpPost("notifications/read")]
    public async Task<IActionResult> MarkAllRead()
    {
        var studentId = User.StudentId();
        var unread = await db.Notifications.Where(n => (n.StudentId == null || n.StudentId == studentId) &&
                !db.NotificationReads.Any(r => r.NotificationId == n.Id && r.StudentId == studentId))
            .Select(n => n.Id).ToListAsync();
        db.NotificationReads.AddRange(unread.Select(id => new NotificationRead { NotificationId = id, StudentId = studentId, ReadAtUtc = DateTime.UtcNow }));
        await db.SaveChangesAsync();
        return Ok(new { marked = unread.Count });
    }
}
