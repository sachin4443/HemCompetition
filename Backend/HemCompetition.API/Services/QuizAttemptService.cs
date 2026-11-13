using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HemCompetition.API.Services;

/// <summary>A question frozen at the moment an attempt started. Stored as JSON, never sent to students as-is.</summary>
public sealed class QuestionSnapshot
{
    public string Text { get; set; } = "";
    public string? Explanation { get; set; }
    public string? ImageKey { get; set; }
    public string? ImageContentType { get; set; }
    public string Difficulty { get; set; } = "";
    public decimal Marks { get; set; }
    public List<OptionSnapshot> Options { get; set; } = new();
    public int CorrectOptionId { get; set; }
}

public sealed class OptionSnapshot
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
}

public sealed class QuizAttemptService(AppDbContext db, CompetitionService competitions)
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    /// <summary>Answers arriving this long after the deadline are still accepted (network latency).</summary>
    private static readonly TimeSpan Grace = TimeSpan.FromSeconds(5);

    /// <summary>Questions a quiz can draw from: its explicit list, or else its category.</summary>
    public IQueryable<Question> Pool(Quiz quiz)
    {
        int quizId = quiz.Id;
        int? categoryId = quiz.CategoryId;
        var usable = db.Questions.Where(q => !q.IsDeleted && q.IsActive && q.Options.Count(o => o.IsCorrect) == 1 && q.Options.Count >= 2)
            .Where(q => db.QuizQuestions.Any(x => x.QuizId == quizId && x.QuestionId == q.Id) ||
                        (!db.QuizQuestions.Any(x => x.QuizId == quizId) && q.CategoryId == categoryId));
        if (quiz.Difficulty != null && quiz.SelectionMode == QuizSelectionMode.Random)
            usable = usable.Where(q => q.Difficulty == quiz.Difficulty);
        return usable;
    }

    /// <summary>Picks and orders the questions for one sitting. Shared by real attempts and the admin preview.</summary>
    public async Task<List<(Question Question, QuestionSnapshot Snapshot)>> BuildPaperAsync(Quiz quiz, Random rng)
    {
        var pool = await Pool(quiz).Include(q => q.Options).AsNoTracking().AsSplitQuery().ToListAsync();
        List<Question> chosen;
        if (quiz.SelectionMode == QuizSelectionMode.Fixed)
        {
            var order = await db.QuizQuestions.AsNoTracking().Where(x => x.QuizId == quiz.Id)
                .ToDictionaryAsync(x => x.QuestionId, x => x.SortOrder);
            chosen = pool.OrderBy(q => order.GetValueOrDefault(q.Id)).ThenBy(q => q.Id).ToList();
            if (quiz.ShuffleQuestions) Shuffle(chosen, rng);
        }
        else
        {
            Shuffle(pool, rng);
            chosen = pool.Take(quiz.QuestionCount).ToList();
            if (!quiz.ShuffleQuestions) chosen = chosen.OrderBy(q => q.Id).ToList();
        }

        return chosen.Select(q =>
        {
            var options = q.Options.OrderBy(o => o.SortOrder).ThenBy(o => o.Id).ToList();
            if (quiz.ShuffleOptions) Shuffle(options, rng);
            return (q, new QuestionSnapshot
            {
                Text = q.Text, Explanation = q.Explanation, ImageKey = q.ImageKey, ImageContentType = q.ImageContentType,
                Difficulty = q.Difficulty.ToString(), Marks = quiz.MarksPerQuestion ?? q.Marks,
                Options = options.Select(o => new OptionSnapshot { Id = o.Id, Text = o.Text }).ToList(),
                CorrectOptionId = options.First(o => o.IsCorrect).Id
            });
        }).ToList();
    }

    public async Task<QuizAttempt> StartAsync(long studentId, int quizId)
    {
        var competition = await competitions.RequireLiveAsync();
        // Unpublished quizzes do not exist as far as students are concerned.
        var quiz = await db.Quizzes.AsNoTracking().FirstOrDefaultAsync(q => q.Id == quizId && q.IsPublished && !q.IsDeleted)
                   ?? throw ApiException.NotFound("This quiz is not available.");

        var existing = await Find(competition.Id, studentId, quizId);
        if (existing != null)
        {
            if (await SubmitIfExpiredAsync(existing)) await db.SaveChangesAsync();
            return existing;
        }

        var paper = await BuildPaperAsync(quiz, new Random());
        if (paper.Count == 0) throw ApiException.Conflict("This quiz has no questions yet.", "QUIZ_EMPTY");

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(quiz.DurationMinutes);
        if (expires > competition.EndDateTimeUtc) expires = competition.EndDateTimeUtc;
        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(), CompetitionId = competition.Id, StudentId = studentId, QuizId = quiz.Id,
            QuizVersion = quiz.Version, QuizTitle = quiz.Title, NegativeMarks = quiz.NegativeMarks,
            Status = AttemptStatus.InProgress, StartedAtUtc = now, ExpiresAtUtc = expires,
            MaxScore = paper.Sum(p => p.Snapshot.Marks), UnansweredCount = paper.Count
        };
        var order = 1;
        foreach (var (question, snapshot) in paper)
            attempt.Questions.Add(new QuizAttemptQuestion
            {
                OrderNo = order++, QuestionId = question.Id, Snapshot = JsonSerializer.Serialize(snapshot, Json)
            });

        db.QuizAttempts.Add(attempt);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.ChangeTracker.Clear();
            var raced = await Find(competition.Id, studentId, quizId);
            if (raced == null) throw;
            return raced;
        }
        return attempt;
    }

    private Task<QuizAttempt?> Find(Guid competitionId, long studentId, int quizId) =>
        db.QuizAttempts.Include(a => a.Questions)
            .FirstOrDefaultAsync(a => a.CompetitionId == competitionId && a.StudentId == studentId && a.QuizId == quizId);

    public async Task<QuizAttempt> LoadOwnedAsync(long studentId, Guid attemptId, bool forUpdate = false)
    {
        if (forUpdate)
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM quiz_attempts WHERE id = {attemptId} FOR UPDATE");
        var attempt = await db.QuizAttempts.Include(a => a.Questions)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.StudentId == studentId);
        return attempt ?? throw ApiException.NotFound("Attempt not found.");
    }

    /// <summary>Saves (or clears) the answer to one question. Allowed any number of times until submission.</summary>
    public async Task SaveAnswerAsync(long studentId, Guid attemptId, int orderNo, int? optionId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var attempt = await LoadOwnedAsync(studentId, attemptId, forUpdate: true);
        if (attempt.Status != AttemptStatus.InProgress)
            throw ApiException.Conflict("This quiz has already been submitted.", "ATTEMPT_FINISHED");
        if (DateTime.UtcNow > attempt.ExpiresAtUtc + Grace)
        {
            Score(attempt, AttemptStatus.Expired);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            throw ApiException.Conflict("Time is up. Your quiz has been submitted.", "ATTEMPT_EXPIRED");
        }
        var question = attempt.Questions.FirstOrDefault(q => q.OrderNo == orderNo) ?? throw ApiException.NotFound("Question not found.");
        if (optionId != null)
        {
            var snapshot = JsonSerializer.Deserialize<QuestionSnapshot>(question.Snapshot, Json)!;
            if (snapshot.Options.All(o => o.Id != optionId))
                throw ApiException.BadRequest("That option does not belong to this question.");
        }
        question.SelectedOptionId = optionId;
        question.AnsweredAtUtc = optionId == null ? null : DateTime.UtcNow;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    /// <summary>Final submission. Calling it again returns the same attempt unchanged.</summary>
    public async Task<QuizAttempt> SubmitAsync(long studentId, Guid attemptId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var attempt = await LoadOwnedAsync(studentId, attemptId, forUpdate: true);
        if (attempt.Status == AttemptStatus.InProgress)
        {
            Score(attempt, DateTime.UtcNow > attempt.ExpiresAtUtc + Grace ? AttemptStatus.Expired : AttemptStatus.Completed);
            await db.SaveChangesAsync();
        }
        await tx.CommitAsync();
        return attempt;
    }

    public Task<bool> SubmitIfExpiredAsync(QuizAttempt attempt)
    {
        if (attempt.Status != AttemptStatus.InProgress || DateTime.UtcNow <= attempt.ExpiresAtUtc + Grace) return Task.FromResult(false);
        Score(attempt, AttemptStatus.Expired);
        return Task.FromResult(true);
    }

    /// <summary>Scores and closes every running quiz whose time has run out (called by the maintenance job).</summary>
    public async Task<int> SweepExpiredAsync()
    {
        var cutoff = DateTime.UtcNow - Grace;
        var due = await db.QuizAttempts.Include(a => a.Questions)
            .Where(a => a.Status == AttemptStatus.InProgress && a.ExpiresAtUtc < cutoff).Take(200).ToListAsync();
        foreach (var a in due) Score(a, AttemptStatus.Expired);
        if (due.Count > 0) await db.SaveChangesAsync();
        return due.Count;
    }

    public async Task<List<QuizAttempt>> TerminateActiveAsync(long studentId)
    {
        var active = await db.QuizAttempts.Include(a => a.Questions)
            .Where(a => a.StudentId == studentId && a.Status == AttemptStatus.InProgress).ToListAsync();
        foreach (var a in active)
        {
            a.IsFlagged = true;
            Score(a, AttemptStatus.Terminated);
        }
        return active;
    }

    /// <summary>The only place quiz marks are calculated. Uses the frozen answer key, never client input.</summary>
    public static void Score(QuizAttempt attempt, AttemptStatus finalStatus)
    {
        decimal score = 0;
        int correct = 0, incorrect = 0, unanswered = 0;
        foreach (var q in attempt.Questions)
        {
            var snapshot = JsonSerializer.Deserialize<QuestionSnapshot>(q.Snapshot, Json)!;
            if (q.SelectedOptionId == null)
            {
                q.IsCorrect = null; q.AwardedMarks = 0; unanswered++;
            }
            else if (q.SelectedOptionId == snapshot.CorrectOptionId)
            {
                q.IsCorrect = true; q.AwardedMarks = snapshot.Marks; correct++;
            }
            else
            {
                q.IsCorrect = false; q.AwardedMarks = -attempt.NegativeMarks; incorrect++;
            }
            score += q.AwardedMarks;
        }
        var now = DateTime.UtcNow;
        var end = now > attempt.ExpiresAtUtc ? attempt.ExpiresAtUtc : now;
        attempt.Score = score;
        attempt.CorrectCount = correct;
        attempt.IncorrectCount = incorrect;
        attempt.UnansweredCount = unanswered;
        attempt.Status = finalStatus;
        attempt.SubmittedAtUtc = end;
        attempt.TimeTakenSeconds = (int)Math.Max(0, (end - attempt.StartedAtUtc).TotalSeconds);
    }

    /// <summary>The paper as a student sees it: no answer key, no explanations.</summary>
    public static object PaperView(QuizAttempt a) => new
    {
        id = a.Id,
        quizId = a.QuizId,
        title = a.QuizTitle,
        status = a.Status.ToString(),
        startedAtUtc = a.StartedAtUtc,
        expiresAtUtc = a.ExpiresAtUtc,
        serverTimeUtc = DateTime.UtcNow,
        negativeMarks = a.NegativeMarks,
        questions = a.Status != AttemptStatus.InProgress ? null : a.Questions.OrderBy(q => q.OrderNo).Select(q =>
        {
            var s = JsonSerializer.Deserialize<QuestionSnapshot>(q.Snapshot, Json)!;
            return new
            {
                orderNo = q.OrderNo,
                text = s.Text,
                marks = s.Marks,
                hasImage = s.ImageKey != null,
                options = s.Options.Select(o => new { id = o.Id, text = o.Text }),
                selectedOptionId = q.SelectedOptionId
            };
        })
    };

    /// <summary>Result for the student, limited by what the competition allows them to see.</summary>
    public static object ResultView(QuizAttempt a, CompetitionSetting competition)
    {
        var showScore = competition.ShowResultsToStudents;
        // Answer keys are only released after the competition closes, so they cannot be passed on.
        var showReview = showScore && DateTime.UtcNow > competition.EndDateTimeUtc;
        return new
        {
            id = a.Id,
            title = a.QuizTitle,
            status = a.Status.ToString(),
            submittedAtUtc = a.SubmittedAtUtc,
            totalQuestions = a.Questions.Count,
            answered = a.Questions.Count(q => q.SelectedOptionId != null),
            resultsVisible = showScore,
            score = showScore ? a.Score : (decimal?)null,
            maxScore = showScore ? a.MaxScore : (decimal?)null,
            correct = showScore ? a.CorrectCount : (int?)null,
            incorrect = showScore ? a.IncorrectCount : (int?)null,
            unanswered = showScore ? a.UnansweredCount : (int?)null,
            timeTakenSeconds = a.TimeTakenSeconds,
            review = !showReview ? null : a.Questions.OrderBy(q => q.OrderNo).Select(q =>
            {
                var s = JsonSerializer.Deserialize<QuestionSnapshot>(q.Snapshot, Json)!;
                return new
                {
                    orderNo = q.OrderNo, text = s.Text, explanation = s.Explanation,
                    options = s.Options.Select(o => new { id = o.Id, text = o.Text }),
                    selectedOptionId = q.SelectedOptionId, correctOptionId = s.CorrectOptionId,
                    isCorrect = q.IsCorrect, awardedMarks = q.AwardedMarks
                };
            })
        };
    }

    private static void Shuffle<T>(IList<T> list, Random rng)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
