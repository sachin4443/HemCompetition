using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Domain;

namespace HemCompetition.API.Games;

/// <summary>Latin square with arithmetic cages.</summary>
public sealed class KenKenEngine : IGameEngine
{
    public string Key => "kenken";
    public bool RequiresWords => false;

    private sealed record Cage(List<(int R, int C)> Cells, string Op, int Target);

    public GeneratedRound Generate(GenerationContext ctx)
    {
        var size = Math.Clamp(ctx.ConfigInt("size", ctx.Difficulty == Difficulty.Hard ? 4 : 3), 3, 5);
        var maxCage = ctx.Difficulty == Difficulty.Easy ? 2 : 3;
        int[][] grid = null!;
        List<Cage> cages = null!;
        // Prefer a puzzle with exactly one solution; validation is by the rules either way.
        for (var attempt = 0; attempt < 200; attempt++)
        {
            grid = LatinSquares.Random(size, ctx.Rng);
            cages = BuildCages(grid, size, maxCage, ctx.Rng);
            var captured = cages;
            if (LatinSquares.Count(size, null, g => captured.All(c => Satisfies(c, g))) == 1) break;
        }

        var cageGrid = new string[size][];
        var labelGrid = new string[size][];
        for (var r = 0; r < size; r++)
        {
            cageGrid[r] = new string[size];
            labelGrid[r] = Enumerable.Repeat("", size).ToArray();
        }
        var cagesJson = new JsonArray();
        for (var i = 0; i < cages.Count; i++)
        {
            var id = ((char)('A' + i)).ToString();
            var cage = cages[i];
            foreach (var (r, c) in cage.Cells) cageGrid[r][c] = id;
            var first = cage.Cells.OrderBy(x => x.R).ThenBy(x => x.C).First();
            labelGrid[first.R][first.C] = cage.Target + cage.Op;
            cagesJson.Add(new JsonObject
            {
                ["id"] = id,
                ["op"] = cage.Op,
                ["target"] = cage.Target,
                ["cells"] = new JsonArray(cage.Cells.Select(x => (JsonNode)new JsonArray(x.R, x.C)).ToArray())
            });
        }

        return new GeneratedRound(
            new JsonObject
            {
                ["size"] = size,
                ["cages"] = cagesJson,
                ["cageGrid"] = cageGrid.ToJson(),
                ["labelGrid"] = labelGrid.ToJson()
            },
            new JsonObject { ["grid"] = grid.ToJson() });
    }

    public ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission)
    {
        var size = puzzle["size"]!.GetValue<int>();
        var grid = EngineHelpers.ReadSubmittedGrid(submission, size);
        if (grid == null) return ValidationResult.Wrong($"Fill every cell with a number from 1 to {size}.");
        if (!EngineHelpers.IsLatin(grid)) return ValidationResult.Wrong("A number is repeated in a row or column.");
        foreach (var node in puzzle["cages"]!.AsArray())
        {
            var cage = new Cage(
                node!["cells"]!.AsArray().Select(c => (c![0]!.GetValue<int>(), c[1]!.GetValue<int>())).ToList(),
                node["op"]!.GetValue<string>(),
                node["target"]!.GetValue<int>());
            if (!Satisfies(cage, grid)) return ValidationResult.Wrong("One of the cages does not match its target.");
        }
        return ValidationResult.Correct;
    }

    private static bool Satisfies(Cage cage, int[][] g)
    {
        var values = cage.Cells.Select(x => g[x.R][x.C]).ToList();
        return cage.Op switch
        {
            "" => values[0] == cage.Target,
            "+" => values.Sum() == cage.Target,
            "x" => values.Aggregate(1, (a, b) => a * b) == cage.Target,
            "-" => values.Count == 2 && Math.Abs(values[0] - values[1]) == cage.Target,
            "/" => values.Count == 2 && Math.Max(values[0], values[1]) == cage.Target * Math.Min(values[0], values[1]),
            _ => false
        };
    }

    private static List<Cage> BuildCages(int[][] grid, int size, int maxCage, Random rng)
    {
        var owner = new int[size, size];
        for (var r = 0; r < size; r++) for (var c = 0; c < size; c++) owner[r, c] = -1;
        var order = (from r in Enumerable.Range(0, size) from c in Enumerable.Range(0, size) select (r, c)).ToList();
        rng.Shuffle(order);
        var groups = new List<List<(int R, int C)>>();
        (int, int)[] steps = [(0, 1), (1, 0), (0, -1), (-1, 0)];

        foreach (var start in order)
        {
            if (owner[start.r, start.c] != -1) continue;
            var cells = new List<(int R, int C)> { start };
            owner[start.r, start.c] = groups.Count;
            // Mostly pairs and triples; a few single "given" cells.
            var wanted = rng.Next(10) == 0 ? 1 : rng.Next(2, maxCage + 1);
            while (cells.Count < wanted)
            {
                var free = cells.SelectMany(x => steps.Select(s => (R: x.R + s.Item1, C: x.C + s.Item2)))
                    .Where(x => x.R >= 0 && x.C >= 0 && x.R < size && x.C < size && owner[x.R, x.C] == -1)
                    .Distinct().ToList();
                if (free.Count == 0) break;
                var next = free[rng.Next(free.Count)];
                owner[next.R, next.C] = groups.Count;
                cells.Add(next);
            }
            groups.Add(cells);
        }

        return groups.Select(cells =>
        {
            var v = cells.Select(x => grid[x.R][x.C]).ToList();
            if (v.Count == 1) return new Cage(cells, "", v[0]);
            var ops = new List<string> { "+", "x" };
            if (v.Count == 2)
            {
                ops.Add("-");
                if (Math.Max(v[0], v[1]) % Math.Min(v[0], v[1]) == 0) ops.Add("/");
            }
            var op = ops[rng.Next(ops.Count)];
            var target = op switch
            {
                "+" => v.Sum(),
                "x" => v.Aggregate(1, (a, b) => a * b),
                "-" => Math.Abs(v[0] - v[1]),
                _ => Math.Max(v[0], v[1]) / Math.Min(v[0], v[1])
            };
            return new Cage(cells, op, target);
        }).ToList();
    }
}
