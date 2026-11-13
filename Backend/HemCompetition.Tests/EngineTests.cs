using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Games;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Services;

namespace HemCompetition.Tests;

public class EngineTests
{
    private static readonly IReadOnlyList<WordEntry> Words = StarterWords.All.Select(w => new WordEntry(w.Word, w.Clue, w.Topic)).ToList();

    private static GenerationContext Ctx(Difficulty d, int seed, bool words = false) =>
        new() { Rng = new Random(seed), Difficulty = d, Words = words ? Words : [] };

    private static JsonElement Json(object o) => JsonSerializer.SerializeToElement(o);

    public static IEnumerable<object[]> Difficulties() => Enum.GetValues<Difficulty>().Select(d => new object[] { d });

    // ------------------------------------------------------------------ Game 24

    [Theory, MemberData(nameof(Difficulties))]
    public void Game24_generated_puzzles_are_solvable_and_their_example_validates(Difficulty d)
    {
        var engine = new Game24Engine();
        for (var seed = 0; seed < 150; seed++)
        {
            var round = engine.Generate(Ctx(d, seed));
            var example = round.Solution["example"]!.GetValue<string>();
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { expression = example })).IsCorrect, example);
            Assert.DoesNotContain("example", round.Puzzle.ToJsonString());
        }
    }

    [Theory]
    [InlineData("(1+2+3)*4", true)]
    [InlineData("1*2*3*4", true)]
    [InlineData("4 × (1 + 2 + 3)", true)]
    [InlineData("(1+2+3)*4*1", false)]      // extra number
    [InlineData("12*2", false)]             // digits glued together
    [InlineData("1+2+3+4", false)]          // equals 10
    [InlineData("24", false)]
    [InlineData("(1+2+3)*4)", false)]
    [InlineData("1+2+3*", false)]
    [InlineData("-1+2+3*4", false)]
    [InlineData("alert(1)", false)]
    [InlineData("process.exit()", false)]
    [InlineData("", false)]
    public void Game24_validates_expressions_without_executing_them(string expression, bool expected)
    {
        var puzzle = new JsonObject { ["numbers"] = new JsonArray(1, 2, 3, 4), ["target"] = 24 };
        Assert.Equal(expected, new Game24Engine().Validate(puzzle, new JsonObject(), Json(new { expression })).IsCorrect);
    }

    [Fact]
    public void Game24_fraction_solutions_are_exact()
    {
        // 8 / (3 - 8/3) = 24 only with exact fractions
        var puzzle = new JsonObject { ["numbers"] = new JsonArray(3, 3, 8, 8), ["target"] = 24 };
        Assert.True(new Game24Engine().Validate(puzzle, new JsonObject(), Json(new { expression = "8/(3-8/3)" })).IsCorrect);
        Assert.NotNull(Game24Engine.Solve([3, 3, 8, 8]));
        Assert.Null(Game24Engine.Solve([1, 1, 1, 1]));
    }

    [Fact]
    public void Game24_rejects_division_by_zero_and_malformed_submissions()
    {
        var engine = new Game24Engine();
        var puzzle = new JsonObject { ["numbers"] = new JsonArray(1, 1, 5, 5), ["target"] = 24 };
        Assert.False(engine.Validate(puzzle, new JsonObject(), Json(new { expression = "5/(1-1)*5" })).IsCorrect);
        Assert.False(engine.Validate(puzzle, new JsonObject(), Json(new { expression = 5 })).IsCorrect);
        Assert.False(engine.Validate(puzzle, new JsonObject(), Json("text")).IsCorrect);
        Assert.False(engine.Validate(puzzle, new JsonObject(), Json(new { expression = new string('(', 500) })).IsCorrect);
    }

    // ------------------------------------------------------------------ KenKen

    [Theory, MemberData(nameof(Difficulties))]
    public void KenKen_solution_validates_and_a_changed_grid_does_not(Difficulty d)
    {
        var engine = new KenKenEngine();
        for (var seed = 0; seed < 60; seed++)
        {
            var round = engine.Generate(Ctx(d, seed));
            var grid = round.Solution["grid"]!.AsArray().Select(r => r!.AsArray().Select(v => v!.GetValue<int>()).ToArray()).ToArray();
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { grid })).IsCorrect);

            // Every cell belongs to exactly one cage.
            var size = round.Puzzle["size"]!.GetValue<int>();
            var cells = round.Puzzle["cages"]!.AsArray().SelectMany(c => c!["cells"]!.AsArray()).Select(c => (c![0]!.GetValue<int>(), c[1]!.GetValue<int>())).ToList();
            Assert.Equal(size * size, cells.Count);
            Assert.Equal(size * size, cells.Distinct().Count());

            (grid[0][0], grid[0][1]) = (grid[0][1], grid[0][0]);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { grid })).IsCorrect);
            Assert.DoesNotContain("\"grid\"", round.Puzzle.ToJsonString());
        }
    }

    [Fact]
    public void KenKen_rejects_wrong_shapes_and_out_of_range_values()
    {
        var engine = new KenKenEngine();
        var round = engine.Generate(Ctx(Difficulty.Medium, 1));
        Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { grid = new[] { new[] { 1, 2 } } })).IsCorrect);
        Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { grid = new[] { new[] { 9, 9, 9 }, new[] { 9, 9, 9 }, new[] { 9, 9, 9 } } })).IsCorrect);
        Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { nothing = true })).IsCorrect);
    }

    // ------------------------------------------------------------------ Futoshiki

    [Theory, MemberData(nameof(Difficulties))]
    public void Futoshiki_puzzles_have_exactly_one_solution(Difficulty d)
    {
        var engine = new FutoshikiEngine();
        for (var seed = 0; seed < 40; seed++)
        {
            var round = engine.Generate(Ctx(d, seed));
            var n = round.Puzzle["size"]!.GetValue<int>();
            var solution = round.Solution["grid"]!.AsArray().Select(r => r!.AsArray().Select(v => v!.GetValue<int>()).ToArray()).ToArray();
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { grid = solution })).IsCorrect);

            var givens = round.Puzzle["givens"]!.AsArray().Select(r => r!.AsArray().Select(v => v!.GetValue<int>()).ToArray()).ToArray();
            var count = LatinSquares.Count(n, givens, g => engine.Validate(round.Puzzle, round.Solution, Json(new { grid = g })).IsCorrect, limit: 3);
            Assert.Equal(1, count);
            // Not everything is given away.
            Assert.True(givens.SelectMany(r => r).Count(v => v != 0) < n * n);
        }
    }

    // ------------------------------------------------------------------ Word games

    [Theory, MemberData(nameof(Difficulties))]
    public void Scramble_five_rounds_do_not_repeat_and_never_leak_answers(Difficulty d)
    {
        var engine = new ScrambleEngine();
        var ctx = Ctx(d, 7, words: true);
        var finals = new HashSet<string>();
        for (var i = 0; i < 5; i++)
        {
            var round = engine.Generate(ctx);
            var rows = round.Solution["rows"]!.AsArray().Select(r => r![0]!.GetValue<string>()).ToArray();
            var final = round.Solution["final"]![0]!.GetValue<string>();
            Assert.True(finals.Add(final));
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { words = rows, final })).IsCorrect);
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { words = rows.Select(r => r.ToLowerInvariant()), final = final.ToLowerInvariant() })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { words = rows, final = "ZZZ" })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { words = rows.Select(_ => "WRONG"), final })).IsCorrect);

            var puzzleRows = round.Puzzle["rows"]!.AsArray();
            var highlighted = "";
            for (var r = 0; r < rows.Length; r++)
            {
                var scrambled = puzzleRows[r]!["scrambled"]!.GetValue<string>();
                Assert.NotEqual(rows[r], scrambled);
                Assert.Equal(EngineHelpers.SortedLetters(rows[r]), EngineHelpers.SortedLetters(scrambled));
                highlighted += rows[r][puzzleRows[r]!["highlight"]!.GetValue<int>()];
            }
            // The highlighted letters really do make the final word.
            Assert.Equal(EngineHelpers.SortedLetters(final), EngineHelpers.SortedLetters(highlighted));
            var json = round.Puzzle.ToJsonString();
            Assert.DoesNotContain($"\"{final}\"", json);
            Assert.All(rows, w => Assert.DoesNotContain($"\"{w}\"", json));
        }
    }

    [Theory, MemberData(nameof(Difficulties))]
    public void WordSearch_words_are_findable_at_their_recorded_positions(Difficulty d)
    {
        var engine = new WordSearchEngine();
        var ctx = Ctx(d, 11, words: true);
        for (var i = 0; i < 5; i++)
        {
            var round = engine.Generate(ctx);
            var found = round.Solution["placements"]!.AsArray().Select(p =>
            {
                var word = p!["word"]!.GetValue<string>();
                int r = p["row"]!.GetValue<int>(), c = p["col"]!.GetValue<int>(), dr = p["dr"]!.GetValue<int>(), dc = p["dc"]!.GetValue<int>();
                return new { start = new[] { r, c }, end = new[] { r + dr * (word.Length - 1), c + dc * (word.Length - 1) } };
            }).ToList();
            Assert.Equal(5, found.Count);
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { found })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { found = found.Take(4) })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { found = new[] { new { start = new[] { 0, 0 }, end = new[] { 99, 99 } } } })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { found = "all" })).IsCorrect);
        }
    }

    [Fact]
    public void WordChallenge_checks_the_answer_and_hides_it()
    {
        var engine = new WordChallengeEngine();
        var ctx = Ctx(Difficulty.Medium, 3, words: true);
        var seen = new HashSet<string>();
        for (var i = 0; i < 20; i++)
        {
            var round = engine.Generate(ctx);
            var answer = round.Solution["answer"]!.GetValue<string>();
            Assert.True(seen.Add(answer));
            Assert.DoesNotContain(answer, round.Puzzle.ToJsonString());
            Assert.Equal(answer.Length, round.Puzzle["length"]!.GetValue<int>());
            Assert.True(engine.Validate(round.Puzzle, round.Solution, Json(new { answer = " " + answer.ToLowerInvariant() + " " })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { answer = answer + "X" })).IsCorrect);
            Assert.False(engine.Validate(round.Puzzle, round.Solution, Json(new { })).IsCorrect);
        }
    }

    [Fact]
    public void Word_engines_report_an_empty_word_bank_clearly()
    {
        var ctx = new GenerationContext { Rng = new Random(1) };
        Assert.Throws<GameUnavailableException>(() => new ScrambleEngine().Generate(ctx));
        Assert.Throws<GameUnavailableException>(() => new WordSearchEngine().Generate(ctx));
        Assert.Throws<GameUnavailableException>(() => new WordChallengeEngine().Generate(ctx));
    }

    // ------------------------------------------------------------------ scoring & helpers

    [Fact]
    public void Quiz_scoring_applies_marks_and_negative_marking_from_the_snapshot()
    {
        string Snap(decimal marks, int correct) => JsonSerializer.Serialize(new QuestionSnapshot
        {
            Text = "q", Marks = marks, CorrectOptionId = correct, Options = [new() { Id = 1, Text = "a" }, new() { Id = 2, Text = "b" }]
        }, QuizAttemptService.Json);
        var attempt = new QuizAttempt
        {
            NegativeMarks = 0.5m, StartedAtUtc = DateTime.UtcNow.AddMinutes(-3), ExpiresAtUtc = DateTime.UtcNow.AddMinutes(7),
            Questions =
            [
                new() { OrderNo = 1, Snapshot = Snap(2, 1), SelectedOptionId = 1 },   // right: +2
                new() { OrderNo = 2, Snapshot = Snap(1, 2), SelectedOptionId = 1 },   // wrong: -0.5
                new() { OrderNo = 3, Snapshot = Snap(3, 2), SelectedOptionId = null } // blank: 0
            ]
        };
        QuizAttemptService.Score(attempt, AttemptStatus.Completed);
        Assert.Equal(1.5m, attempt.Score);
        Assert.Equal((1, 1, 1), (attempt.CorrectCount, attempt.IncorrectCount, attempt.UnansweredCount));
        Assert.Equal(AttemptStatus.Completed, attempt.Status);
        Assert.InRange(attempt.TimeTakenSeconds, 179, 182);
    }

    [Fact]
    public void Game_scoring_sums_correct_rounds_only()
    {
        var attempt = new GameAttempt { TotalRounds = 5, MaxScore = 50, Rounds = [] };
        for (var i = 1; i <= 5; i++)
        {
            var status = i <= 3 ? RoundStatus.Correct : RoundStatus.Skipped;
            var round = new GameAttemptRound { RoundNo = i, Status = status, TimeTakenSeconds = 10 };
            round.Score = ScoreCalculator.RoundScore(attempt, status);
            attempt.Rounds.Add(round);
        }
        ScoreCalculator.Apply(attempt);
        Assert.Equal((30m, 3, 50), (attempt.Score, attempt.CorrectCount, attempt.TimeTakenSeconds));
    }

    [Fact]
    public void Csv_parser_handles_quotes_commas_and_newlines()
    {
        var rows = TabularFile.ParseCsv("a,b,c\r\n\"x, y\",\"he said \"\"hi\"\"\",\"line1\nline2\"\nlast,,\n");
        Assert.Equal(3, rows.Count);
        Assert.Equal(["x, y", "he said \"hi\"", "line1\nline2"], rows[1]);
        Assert.Equal(["last", "", ""], rows[2]);
    }

    [Fact]
    public void Csv_export_neutralises_formulas_but_keeps_numbers()
    {
        var csv = System.Text.Encoding.UTF8.GetString(TabularFile.ToCsv(["a", "b"], [["=HYPERLINK(\"x\")", -5m]]));
        Assert.Contains("'=HYPERLINK", csv);
        Assert.Contains(",-5", csv);
    }

    [Fact]
    public void File_sniffer_trusts_bytes_not_names()
    {
        Assert.Equal("image/jpeg", FileSniffer.DetectContentType([0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0]));
        Assert.Equal("image/png", FileSniffer.DetectContentType([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]));
        Assert.Equal("video/webm", FileSniffer.DetectContentType([0x1A, 0x45, 0xDF, 0xA3, 0, 0]));
        Assert.Null(FileSniffer.DetectContentType("<html><script>"u8));
        Assert.Null(FileSniffer.DetectContentType("MZ\x90\0\x03\0\0\0"u8));
    }
}
