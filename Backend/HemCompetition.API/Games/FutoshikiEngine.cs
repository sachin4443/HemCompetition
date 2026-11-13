using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Domain;

namespace HemCompetition.API.Games;

/// <summary>Latin square with "greater than / less than" signs between neighbouring cells.</summary>
public sealed class FutoshikiEngine : IGameEngine
{
    public string Key => "futoshiki";
    public bool RequiresWords => false;

    public GeneratedRound Generate(GenerationContext ctx)
    {
        var n = Math.Clamp(ctx.ConfigInt("size", ctx.Difficulty == Difficulty.Hard ? 5 : 4), 4, 5);
        var rng = ctx.Rng;
        var grid = LatinSquares.Random(n, rng);

        var givens = new int[n][];
        var h = new string[n][];       // h[r][c] sits between (r,c) and (r,c+1)
        var v = new string[n - 1][];   // v[r][c] sits between (r,c) and (r+1,c)
        for (var r = 0; r < n; r++) { givens[r] = new int[n]; h[r] = Enumerable.Repeat("", n - 1).ToArray(); }
        for (var r = 0; r < n - 1; r++) v[r] = Enumerable.Repeat("", n).ToArray();

        // Every possible clue, in random order. Clues are added until exactly one solution remains.
        var clues = new List<(char Kind, int R, int C)>();
        for (var r = 0; r < n; r++) for (var c = 0; c < n - 1; c++) clues.Add(('h', r, c));
        for (var r = 0; r < n - 1; r++) for (var c = 0; c < n; c++) clues.Add(('v', r, c));
        rng.Shuffle(clues);
        var cells = (from r in Enumerable.Range(0, n) from c in Enumerable.Range(0, n) select ('g', r, c)).ToList();
        rng.Shuffle(cells);

        var startingSigns = ctx.Difficulty switch { Difficulty.Easy => n + 2, Difficulty.Medium => n + 1, _ => n };
        var startingGivens = ctx.Difficulty switch { Difficulty.Easy => 3, Difficulty.Medium => 2, _ => 1 };
        var queue = new Queue<(char Kind, int R, int C)>(
            clues.Take(startingSigns).Concat(cells.Take(startingGivens))
                 // afterwards alternate signs and givens
                 .Concat(Interleave(clues.Skip(startingSigns), cells.Skip(startingGivens))));

        var placed = 0;
        while (queue.Count > 0)
        {
            var (kind, r, c) = queue.Dequeue();
            switch (kind)
            {
                case 'h': h[r][c] = grid[r][c] < grid[r][c + 1] ? "<" : ">"; break;
                case 'v': v[r][c] = grid[r][c] < grid[r + 1][c] ? "<" : ">"; break;
                default: givens[r][c] = grid[r][c]; break;
            }
            if (++placed < startingSigns + startingGivens) continue;
            if (LatinSquares.Count(n, givens, g => SignsHold(g, h, v)) == 1) break;
        }

        return new GeneratedRound(
            new JsonObject
            {
                ["size"] = n,
                ["givens"] = givens.ToJson(),
                ["hSigns"] = h.ToJson(),
                // "<" means the upper cell is smaller than the lower cell.
                ["vSigns"] = v.ToJson()
            },
            new JsonObject { ["grid"] = grid.ToJson() });
    }

    public ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission)
    {
        var n = puzzle["size"]!.GetValue<int>();
        var grid = EngineHelpers.ReadSubmittedGrid(submission, n);
        if (grid == null) return ValidationResult.Wrong($"Fill every cell with a number from 1 to {n}.");
        var givens = EngineHelpers.ReadGrid(puzzle["givens"]);
        for (var r = 0; r < n; r++)
            for (var c = 0; c < n; c++)
                if (givens[r][c] != 0 && givens[r][c] != grid[r][c])
                    return ValidationResult.Wrong("The numbers already given cannot be changed.");
        if (!EngineHelpers.IsLatin(grid)) return ValidationResult.Wrong("A number is repeated in a row or column.");
        return SignsHold(grid, EngineHelpers.ReadStringGrid(puzzle["hSigns"]), EngineHelpers.ReadStringGrid(puzzle["vSigns"]))
            ? ValidationResult.Correct
            : ValidationResult.Wrong("One of the greater-than / less-than signs is not satisfied.");
    }

    private static bool SignsHold(int[][] g, string[][] h, string[][] v)
    {
        for (var r = 0; r < h.Length; r++)
            for (var c = 0; c < h[r].Length; c++)
                if (!Holds(h[r][c], g[r][c], g[r][c + 1])) return false;
        for (var r = 0; r < v.Length; r++)
            for (var c = 0; c < v[r].Length; c++)
                if (!Holds(v[r][c], g[r][c], g[r + 1][c])) return false;
        return true;
    }

    private static bool Holds(string sign, int a, int b) => sign switch { "<" => a < b, ">" => a > b, _ => true };

    private static IEnumerable<T> Interleave<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
        using var a = first.GetEnumerator();
        using var b = second.GetEnumerator();
        bool hasA = a.MoveNext(), hasB = b.MoveNext();
        while (hasA || hasB)
        {
            if (hasA) { yield return a.Current; hasA = a.MoveNext(); }
            if (hasB) { yield return b.Current; hasB = b.MoveNext(); }
        }
    }
}
