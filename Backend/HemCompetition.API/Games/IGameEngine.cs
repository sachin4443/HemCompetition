using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Domain;

namespace HemCompetition.API.Games;

public sealed record WordEntry(string Word, string? Clue, string? Topic);

/// <summary>Everything an engine may use to build one round.</summary>
public sealed class GenerationContext
{
    public required Random Rng { get; init; }
    public Difficulty Difficulty { get; init; } = Difficulty.Medium;
    /// <summary>Engine options from <c>games.config</c>.</summary>
    public JsonObject Config { get; init; } = new();
    /// <summary>Active word bank (upper-case). Empty for number games.</summary>
    public IReadOnlyList<WordEntry> Words { get; init; } = Array.Empty<WordEntry>();
    /// <summary>Words already used earlier in the same attempt, so rounds do not repeat.</summary>
    public HashSet<string> UsedWords { get; } = new();

    public int ConfigInt(string name, int fallback) =>
        Config.TryGetPropertyValue(name, out var n) && n is JsonValue v && v.TryGetValue<int>(out var i) ? i : fallback;
    public bool ConfigBool(string name, bool fallback) =>
        Config.TryGetPropertyValue(name, out var n) && n is JsonValue v && v.TryGetValue<bool>(out var b) ? b : fallback;
}

/// <param name="Puzzle">Sent to the student. Must not reveal the answer.</param>
/// <param name="Solution">Stored server-side only.</param>
public sealed record GeneratedRound(JsonObject Puzzle, JsonObject Solution);

public sealed record ValidationResult(bool IsCorrect, string? Message = null)
{
    public static readonly ValidationResult Correct = new(true);
    public static ValidationResult Wrong(string message) => new(false, message);
}

/// <summary>Thrown when a round cannot be generated (for example the word bank is too small).</summary>
public sealed class GameUnavailableException(string message) : Exception(message);

/// <summary>
/// Contract every game implements. Adding a game means adding one class that implements this
/// interface and one row in the <c>games</c> table - attempts, scoring, history, leaderboards and
/// the API are shared.
/// </summary>
public interface IGameEngine
{
    string Key { get; }
    bool RequiresWords { get; }
    GeneratedRound Generate(GenerationContext context);
    ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission);
}

public sealed class GameEngineRegistry
{
    private readonly Dictionary<string, IGameEngine> _engines;
    public GameEngineRegistry(IEnumerable<IGameEngine> engines) =>
        _engines = engines.ToDictionary(e => e.Key, StringComparer.OrdinalIgnoreCase);
    public IGameEngine Get(string key) =>
        _engines.TryGetValue(key, out var e) ? e : throw new GameUnavailableException($"No engine registered for '{key}'.");
    public IReadOnlyCollection<string> Keys => _engines.Keys;
}

public static class EngineHelpers
{
    public static void Shuffle<T>(this Random rng, IList<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static JsonArray ToJson(this int[][] grid) =>
        new(grid.Select(r => (JsonNode)new JsonArray(r.Select(v => (JsonNode)v).ToArray())).ToArray());

    public static JsonArray ToJson(this string[][] grid) =>
        new(grid.Select(r => (JsonNode)new JsonArray(r.Select(v => (JsonNode)v).ToArray())).ToArray());

    public static int[][] ReadGrid(JsonNode? node) =>
        node!.AsArray().Select(r => r!.AsArray().Select(v => v!.GetValue<int>()).ToArray()).ToArray();

    public static string[][] ReadStringGrid(JsonNode? node) =>
        node!.AsArray().Select(r => r!.AsArray().Select(v => v!.GetValue<string>()).ToArray()).ToArray();

    /// <summary>Reads an n×n grid of integers 1..n from a submission; null when the shape or values are wrong.</summary>
    public static int[][]? ReadSubmittedGrid(JsonElement submission, int size)
    {
        if (submission.ValueKind != JsonValueKind.Object ||
            !submission.TryGetProperty("grid", out var g) || g.ValueKind != JsonValueKind.Array || g.GetArrayLength() != size)
            return null;
        var grid = new int[size][];
        var r = 0;
        foreach (var row in g.EnumerateArray())
        {
            if (row.ValueKind != JsonValueKind.Array || row.GetArrayLength() != size) return null;
            grid[r] = new int[size];
            var c = 0;
            foreach (var cell in row.EnumerateArray())
            {
                if (cell.ValueKind != JsonValueKind.Number || !cell.TryGetInt32(out var v) || v < 1 || v > size) return null;
                grid[r][c++] = v;
            }
            r++;
        }
        return grid;
    }

    public static bool IsLatin(int[][] grid)
    {
        var n = grid.Length;
        for (var i = 0; i < n; i++)
        {
            int rowMask = 0, colMask = 0;
            for (var j = 0; j < n; j++)
            {
                rowMask |= 1 << grid[i][j];
                colMask |= 1 << grid[j][i];
            }
            var full = ((1 << n) - 1) << 1;
            if (rowMask != full || colMask != full) return false;
        }
        return true;
    }

    public static string? ReadString(JsonElement submission, string property)
    {
        if (submission.ValueKind != JsonValueKind.Object || !submission.TryGetProperty(property, out var p) ||
            p.ValueKind != JsonValueKind.String) return null;
        return p.GetString()?.Trim().ToUpperInvariant();
    }

    public static string SortedLetters(string word) => new(word.OrderBy(c => c).ToArray());
}

/// <summary>Latin square generation and counting shared by KenKen and Futoshiki.</summary>
public static class LatinSquares
{
    public static int[][] Random(int n, Random rng)
    {
        var rows = Enumerable.Range(0, n).ToList();
        var cols = Enumerable.Range(0, n).ToList();
        var symbols = Enumerable.Range(1, n).ToList();
        rng.Shuffle(rows); rng.Shuffle(cols); rng.Shuffle(symbols);
        var grid = new int[n][];
        for (var r = 0; r < n; r++)
        {
            grid[r] = new int[n];
            for (var c = 0; c < n; c++) grid[r][c] = symbols[(rows[r] + cols[c]) % n];
        }
        return grid;
    }

    /// <summary>Counts Latin squares that keep the given cells and satisfy <paramref name="accept"/>, stopping at <paramref name="limit"/>.</summary>
    public static int Count(int n, int[][]? givens, Func<int[][], bool> accept, int limit = 2)
    {
        var grid = new int[n][];
        for (var i = 0; i < n; i++) grid[i] = new int[n];
        var rowUsed = new int[n];
        var colUsed = new int[n];
        var count = 0;

        void Fill(int pos)
        {
            if (count >= limit) return;
            if (pos == n * n)
            {
                if (accept(grid)) count++;
                return;
            }
            int r = pos / n, c = pos % n;
            var fixedValue = givens?[r][c] ?? 0;
            for (var v = 1; v <= n; v++)
            {
                if (fixedValue != 0 && v != fixedValue) continue;
                var bit = 1 << v;
                if ((rowUsed[r] & bit) != 0 || (colUsed[c] & bit) != 0) continue;
                grid[r][c] = v; rowUsed[r] |= bit; colUsed[c] |= bit;
                Fill(pos + 1);
                rowUsed[r] &= ~bit; colUsed[c] &= ~bit;
                if (count >= limit) return;
            }
            grid[r][c] = 0;
        }

        Fill(0);
        return count;
    }
}
