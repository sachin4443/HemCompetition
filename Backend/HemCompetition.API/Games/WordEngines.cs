using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Domain;

namespace HemCompetition.API.Games;

/// <summary>
/// Unscramble each row; the highlighted letter of every row, rearranged, spells the final word.
/// </summary>
public sealed class ScrambleEngine : IGameEngine
{
    public string Key => "scramble";
    public bool RequiresWords => true;

    public GeneratedRound Generate(GenerationContext ctx)
    {
        var rng = ctx.Rng;
        var rows = Math.Clamp(ctx.ConfigInt("rows", ctx.Difficulty switch { Difficulty.Easy => 3, Difficulty.Medium => 4, _ => 5 }), 3, 7);
        var byLetters = ctx.Words.GroupBy(w => EngineHelpers.SortedLetters(w.Word))
            .ToDictionary(g => g.Key, g => g.Select(w => w.Word).ToList());

        var finals = ctx.Words.Where(w => w.Word.Length == rows && !ctx.UsedWords.Contains(w.Word)).ToList();
        rng.Shuffle(finals);
        foreach (var final in finals)
        {
            var letters = final.Word.ToList();
            rng.Shuffle(letters);
            var chosen = new List<(string Word, int Highlight)>();
            var taken = new HashSet<string>(ctx.UsedWords) { final.Word };
            foreach (var letter in letters)
            {
                var options = ctx.Words.Where(w => w.Word.Length is >= 4 and <= 8 && w.Word.Contains(letter) && !taken.Contains(w.Word)).ToList();
                if (options.Count == 0) break;
                var pick = options[rng.Next(options.Count)].Word;
                var positions = Enumerable.Range(0, pick.Length).Where(i => pick[i] == letter).ToList();
                chosen.Add((pick, positions[rng.Next(positions.Count)]));
                taken.Add(pick);
            }
            if (chosen.Count != rows) continue;

            ctx.UsedWords.Add(final.Word);
            foreach (var c in chosen) ctx.UsedWords.Add(c.Word);

            var puzzleRows = new JsonArray();
            var solutionRows = new JsonArray();
            foreach (var (word, highlight) in chosen)
            {
                puzzleRows.Add(new JsonObject { ["scrambled"] = ScrambleWord(word, rng), ["length"] = word.Length, ["highlight"] = highlight });
                // Other dictionary words with the same letters are accepted when they keep the highlighted letter.
                var accepted = byLetters[EngineHelpers.SortedLetters(word)].Where(w => w[highlight] == word[highlight]);
                solutionRows.Add(new JsonArray(accepted.Select(w => (JsonNode)w).ToArray()));
            }
            return new GeneratedRound(
                new JsonObject { ["rows"] = puzzleRows, ["finalLength"] = final.Word.Length },
                new JsonObject
                {
                    ["rows"] = solutionRows,
                    ["final"] = new JsonArray(byLetters[EngineHelpers.SortedLetters(final.Word)].Select(w => (JsonNode)w).ToArray())
                });
        }
        throw new GameUnavailableException("The word bank does not have enough words for Scramble.");
    }

    public ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission)
    {
        if (submission.ValueKind != JsonValueKind.Object || !submission.TryGetProperty("words", out var words) ||
            words.ValueKind != JsonValueKind.Array)
            return ValidationResult.Wrong("Complete every row.");
        var expected = solution["rows"]!.AsArray();
        if (words.GetArrayLength() != expected.Count) return ValidationResult.Wrong("Complete every row.");
        var i = 0;
        foreach (var w in words.EnumerateArray())
        {
            var given = w.ValueKind == JsonValueKind.String ? w.GetString()!.Trim().ToUpperInvariant() : "";
            if (!expected[i]!.AsArray().Any(a => a!.GetValue<string>() == given))
                return ValidationResult.Wrong($"Row {i + 1} is not correct.");
            i++;
        }
        var final = EngineHelpers.ReadString(submission, "final");
        if (string.IsNullOrEmpty(final)) return ValidationResult.Wrong("Enter the final word.");
        return solution["final"]!.AsArray().Any(a => a!.GetValue<string>() == final)
            ? ValidationResult.Correct
            : ValidationResult.Wrong("The final word is not correct.");
    }

    private static string ScrambleWord(string word, Random rng)
    {
        var letters = word.ToList();
        for (var i = 0; i < 20; i++)
        {
            rng.Shuffle(letters);
            var s = new string(letters.ToArray());
            if (s != word) return s;
        }
        return new string(word.Reverse().ToArray());
    }
}

/// <summary>Word search ("Puzzle"): find every listed word in a letter grid.</summary>
public sealed class WordSearchEngine : IGameEngine
{
    public string Key => "puzzle";
    public bool RequiresWords => true;

    public GeneratedRound Generate(GenerationContext ctx)
    {
        var rng = ctx.Rng;
        var size = Math.Clamp(ctx.ConfigInt("gridSize", ctx.Difficulty switch { Difficulty.Easy => 8, Difficulty.Medium => 10, _ => 12 }), 6, 15);
        var count = Math.Clamp(ctx.ConfigInt("words", 5), 2, 10);
        var directions = new List<(int Dr, int Dc)> { (0, 1), (1, 0) };
        if (ctx.Difficulty != Difficulty.Easy) directions.Add((1, 1));
        if (ctx.Difficulty == Difficulty.Hard) directions.AddRange([(0, -1), (-1, 0), (-1, 1), (1, -1), (-1, -1)]);

        var candidates = ctx.Words.Select(w => w.Word)
            .Where(w => w.Length >= 3 && w.Length <= size - 1 && !ctx.UsedWords.Contains(w)).ToList();
        if (candidates.Count < count) throw new GameUnavailableException("The word bank does not have enough words for Puzzle.");

        for (var attempt = 0; attempt < 50; attempt++)
        {
            rng.Shuffle(candidates);
            // A word contained in another chosen word would be ambiguous to find.
            var words = new List<string>();
            foreach (var w in candidates)
            {
                if (words.Any(x => x.Contains(w) || w.Contains(x))) continue;
                words.Add(w);
                if (words.Count == count) break;
            }
            if (words.Count < count) continue;

            var grid = new char[size, size];
            var placements = new JsonArray();
            var ok = true;
            foreach (var word in words.OrderByDescending(w => w.Length))
            {
                var placed = false;
                for (var t = 0; t < 200 && !placed; t++)
                {
                    var (dr, dc) = directions[rng.Next(directions.Count)];
                    int r = rng.Next(size), c = rng.Next(size);
                    int endR = r + dr * (word.Length - 1), endC = c + dc * (word.Length - 1);
                    if (endR < 0 || endR >= size || endC < 0 || endC >= size) continue;
                    var fits = true;
                    for (var i = 0; i < word.Length && fits; i++)
                    {
                        var existing = grid[r + dr * i, c + dc * i];
                        fits = existing == '\0' || existing == word[i];
                    }
                    if (!fits) continue;
                    for (var i = 0; i < word.Length; i++) grid[r + dr * i, c + dc * i] = word[i];
                    placements.Add(new JsonObject { ["word"] = word, ["row"] = r, ["col"] = c, ["dr"] = dr, ["dc"] = dc });
                    placed = true;
                }
                if (!placed) { ok = false; break; }
            }
            if (!ok) continue;

            var rowsJson = new JsonArray();
            for (var r = 0; r < size; r++)
            {
                var chars = new char[size];
                for (var c = 0; c < size; c++) chars[c] = grid[r, c] == '\0' ? (char)('A' + rng.Next(26)) : grid[r, c];
                rowsJson.Add(new string(chars));
            }
            foreach (var w in words) ctx.UsedWords.Add(w);
            rng.Shuffle(words);
            return new GeneratedRound(
                new JsonObject { ["size"] = size, ["grid"] = rowsJson, ["words"] = new JsonArray(words.Select(w => (JsonNode)w).ToArray()) },
                new JsonObject { ["placements"] = placements });
        }
        throw new GameUnavailableException("Could not build a word-search grid.");
    }

    public ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission)
    {
        if (submission.ValueKind != JsonValueKind.Object || !submission.TryGetProperty("found", out var found) ||
            found.ValueKind != JsonValueKind.Array || found.GetArrayLength() > 50)
            return ValidationResult.Wrong("Find all the words first.");
        var grid = puzzle["grid"]!.AsArray().Select(r => r!.GetValue<string>()).ToArray();
        var size = grid.Length;
        var remaining = puzzle["words"]!.AsArray().Select(w => w!.GetValue<string>()).ToHashSet();

        foreach (var sel in found.EnumerateArray())
        {
            if (!TryPoint(sel, "start", size, out var s) || !TryPoint(sel, "end", size, out var e)) continue;
            int dr = Math.Sign(e.R - s.R), dc = Math.Sign(e.C - s.C);
            int lenR = Math.Abs(e.R - s.R), lenC = Math.Abs(e.C - s.C);
            if (lenR != 0 && lenC != 0 && lenR != lenC) continue; // not a straight line
            var len = Math.Max(lenR, lenC) + 1;
            var chars = new char[len];
            for (var i = 0; i < len; i++) chars[i] = grid[s.R + dr * i][s.C + dc * i];
            var text = new string(chars);
            if (!remaining.Remove(text))
            {
                Array.Reverse(chars);
                remaining.Remove(new string(chars));
            }
        }
        return remaining.Count == 0
            ? ValidationResult.Correct
            : ValidationResult.Wrong($"{remaining.Count} word(s) still to find.");
    }

    private static bool TryPoint(JsonElement sel, string name, int size, out (int R, int C) point)
    {
        point = default;
        if (sel.ValueKind != JsonValueKind.Object || !sel.TryGetProperty(name, out var p) || p.ValueKind != JsonValueKind.Array ||
            p.GetArrayLength() != 2 || !p[0].TryGetInt32(out var r) || !p[1].TryGetInt32(out var c)) return false;
        if (r < 0 || c < 0 || r >= size || c >= size) return false;
        point = (r, c);
        return true;
    }
}

/// <summary>Word Challenge: answer a clue with a word of a stated length.</summary>
public sealed class WordChallengeEngine : IGameEngine
{
    public string Key => "word";
    public bool RequiresWords => true;

    public GeneratedRound Generate(GenerationContext ctx)
    {
        var options = ctx.Words.Where(w => !string.IsNullOrWhiteSpace(w.Clue) && !ctx.UsedWords.Contains(w.Word)).ToList();
        if (options.Count == 0) throw new GameUnavailableException("The word bank does not have enough clued words for Word Challenge.");
        var pick = options[ctx.Rng.Next(options.Count)];
        ctx.UsedWords.Add(pick.Word);
        return new GeneratedRound(
            new JsonObject { ["question"] = pick.Clue, ["length"] = pick.Word.Length, ["topic"] = pick.Topic },
            new JsonObject { ["answer"] = pick.Word });
    }

    public ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission)
    {
        var answer = EngineHelpers.ReadString(submission, "answer");
        if (string.IsNullOrEmpty(answer)) return ValidationResult.Wrong("Type your answer.");
        var length = puzzle["length"]!.GetValue<int>();
        if (answer.Length != length) return ValidationResult.Wrong($"The answer has {length} letters.");
        return answer == solution["answer"]!.GetValue<string>()
            ? ValidationResult.Correct
            : ValidationResult.Wrong("That is not the right word. Try again.");
    }
}
