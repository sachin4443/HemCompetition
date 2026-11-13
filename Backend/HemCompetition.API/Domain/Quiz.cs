namespace HemCompetition.API.Domain;

/// <summary>A question bank.</summary>
public class QuizCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
}

public class Question
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public decimal Marks { get; set; } = 1;
    public string? ImageKey { get; set; }
    public string? ImageContentType { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public QuizCategory Category { get; set; } = null!;
    public List<QuestionOption> Options { get; set; } = new();
}

public class QuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
    public Question Question { get; set; } = null!;
}

public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>Question bank used when no explicit question list is attached.</summary>
    public int? CategoryId { get; set; }
    public QuizSelectionMode SelectionMode { get; set; } = QuizSelectionMode.Random;
    public Difficulty? Difficulty { get; set; }
    public int QuestionCount { get; set; } = 10;
    public int DurationMinutes { get; set; } = 10;
    /// <summary>Overrides each question's own marks when set.</summary>
    public decimal? MarksPerQuestion { get; set; }
    public decimal NegativeMarks { get; set; }
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    /// <summary>Incremented on every change so attempts record which definition they were taken under.</summary>
    public int Version { get; set; } = 1;
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public QuizCategory? Category { get; set; }
    public List<QuizQuestion> Questions { get; set; } = new();
}

/// <summary>Explicit question list (fixed mode) or restricted pool (random mode) of a quiz.</summary>
public class QuizQuestion
{
    public int QuizId { get; set; }
    public int QuestionId { get; set; }
    public int SortOrder { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public Question Question { get; set; } = null!;
}

public class QuizAttempt
{
    public Guid Id { get; set; }
    public Guid CompetitionId { get; set; }
    public long StudentId { get; set; }
    public int QuizId { get; set; }
    public int QuizVersion { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public decimal NegativeMarks { get; set; }
    public AttemptStatus Status { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    public int UnansweredCount { get; set; }
    public int TimeTakenSeconds { get; set; }
    public bool IsFlagged { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public CompetitionSetting Competition { get; set; } = null!;
    public List<QuizAttemptQuestion> Questions { get; set; } = new();
}

/// <summary>
/// One question as it was presented in an attempt. <see cref="Snapshot"/> freezes text, options, marks
/// and the answer key so results stay accurate after the question bank is edited.
/// </summary>
public class QuizAttemptQuestion
{
    public long Id { get; set; }
    public Guid AttemptId { get; set; }
    public int OrderNo { get; set; }
    public int? QuestionId { get; set; }
    public string Snapshot { get; set; } = "{}";
    public int? SelectedOptionId { get; set; }
    public bool? IsCorrect { get; set; }
    public decimal AwardedMarks { get; set; }
    public DateTime? AnsweredAtUtc { get; set; }
    public QuizAttempt Attempt { get; set; } = null!;
}
