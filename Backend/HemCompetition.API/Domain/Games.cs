namespace HemCompetition.API.Domain;

/// <summary>One of the three sections on the student dashboard (number, quiz, word).</summary>
public class GameCategory
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Game> Games { get; set; } = new();
}

/// <summary>A generated game. Content is produced by the engine named in <see cref="EngineKey"/>.</summary>
public class Game
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EngineKey { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public int Rounds { get; set; } = 5;
    public decimal PointsPerRound { get; set; } = 10;
    /// <summary>Optional limit for the whole attempt. Null means limited only by the competition window.</summary>
    public int? TimeLimitSeconds { get; set; }
    /// <summary>Engine specific options (JSON object).</summary>
    public string Config { get; set; } = "{}";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UpdatedAtUtc { get; set; }
    public GameCategory Category { get; set; } = null!;
}

/// <summary>Vocabulary the word engines generate from.</summary>
public class WordBankEntry
{
    public int Id { get; set; }
    public string Word { get; set; } = string.Empty;
    /// <summary>Question shown in Word Challenge. Words without a clue are used only by Scramble and Puzzle.</summary>
    public string? Clue { get; set; }
    public string? Topic { get; set; }
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
}

public class GameAttempt
{
    public Guid Id { get; set; }
    public Guid CompetitionId { get; set; }
    public long StudentId { get; set; }
    public int GameId { get; set; }
    public AttemptStatus Status { get; set; }
    public Difficulty Difficulty { get; set; }
    /// <summary>Game configuration at the time the attempt started (JSON).</summary>
    public string ConfigSnapshot { get; set; } = "{}";
    public int TotalRounds { get; set; }
    public int CurrentRound { get; set; } = 1;
    public int CorrectCount { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public int TimeTakenSeconds { get; set; }
    public bool IsFlagged { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? DeadlineUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Game Game { get; set; } = null!;
    public CompetitionSetting Competition { get; set; } = null!;
    public List<GameAttemptRound> Rounds { get; set; } = new();
}

public class GameAttemptRound
{
    public long Id { get; set; }
    public Guid AttemptId { get; set; }
    public int RoundNo { get; set; }
    /// <summary>What the student sees (JSON). Never contains the answer.</summary>
    public string Puzzle { get; set; } = "{}";
    /// <summary>Server-held answer or validation data (JSON). Never sent to the student during an attempt.</summary>
    public string Solution { get; set; } = "{}";
    /// <summary>Last answer the student submitted (JSON).</summary>
    public string? Submission { get; set; }
    public RoundStatus Status { get; set; }
    public int WrongSubmissions { get; set; }
    public decimal Score { get; set; }
    public DateTime? PresentedAtUtc { get; set; }
    public DateTime? AnsweredAtUtc { get; set; }
    public int TimeTakenSeconds { get; set; }
    public GameAttempt Attempt { get; set; } = null!;
}
