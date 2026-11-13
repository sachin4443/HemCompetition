using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Domain;

namespace HemCompetition.API.Games;

/// <summary>Use four numbers once each with + - * / to make 24.</summary>
public sealed class Game24Engine : IGameEngine
{
    public const int Target = 24;
    public string Key => "game24";
    public bool RequiresWords => false;

    public GeneratedRound Generate(GenerationContext ctx)
    {
        var max = ctx.ConfigInt("maxNumber", ctx.Difficulty switch { Difficulty.Easy => 9, Difficulty.Medium => 10, _ => 13 });
        max = Math.Clamp(max, 4, 20);
        // Easy puzzles must be solvable without division.
        var allowDivision = ctx.Difficulty != Difficulty.Easy;
        for (var attempt = 0; attempt < 5000; attempt++)
        {
            var numbers = Enumerable.Range(0, 4).Select(_ => ctx.Rng.Next(1, max + 1)).ToArray();
            var example = Solve(numbers, allowDivision);
            if (example == null) continue;
            return new GeneratedRound(
                new JsonObject { ["numbers"] = new JsonArray(numbers.Select(n => (JsonNode)n).ToArray()), ["target"] = Target },
                new JsonObject { ["example"] = example });
        }
        throw new GameUnavailableException("Could not generate a Game 24 puzzle.");
    }

    public ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission)
    {
        var expression = submission.ValueKind == JsonValueKind.Object && submission.TryGetProperty("expression", out var e) &&
                         e.ValueKind == JsonValueKind.String ? e.GetString() : null;
        if (string.IsNullOrWhiteSpace(expression) || expression.Length > 80)
            return ValidationResult.Wrong("Enter an expression using all four numbers.");

        var parser = new ExpressionParser(expression);
        if (!parser.TryEvaluate(out var value, out var used))
            return ValidationResult.Wrong("That is not a valid expression.");

        var expected = puzzle["numbers"]!.AsArray().Select(n => n!.GetValue<int>()).OrderBy(n => n);
        if (!used.OrderBy(n => n).SequenceEqual(expected.Select(n => (long)n)))
            return ValidationResult.Wrong("Use each of the four numbers exactly once.");

        return value.Equals(new Fraction(Target, 1))
            ? ValidationResult.Correct
            : ValidationResult.Wrong("The expression does not equal 24.");
    }

    /// <summary>Returns one expression that makes 24, or null when the numbers cannot.</summary>
    public static string? Solve(int[] numbers, bool allowDivision = true)
    {
        var items = numbers.Select(n => (Value: new Fraction(n, 1), Text: n.ToString())).ToList();
        return Search(items, allowDivision);
    }

    private static string? Search(List<(Fraction Value, string Text)> items, bool allowDivision)
    {
        if (items.Count == 1) return items[0].Value.Equals(new Fraction(Target, 1)) ? items[0].Text : null;
        for (var i = 0; i < items.Count; i++)
        for (var j = 0; j < items.Count; j++)
        {
            if (i == j) continue;
            var (a, b) = (items[i], items[j]);
            var rest = items.Where((_, k) => k != i && k != j).ToList();
            var candidates = new List<(Fraction, string)>();
            if (i < j)
            {
                candidates.Add((a.Value + b.Value, $"({a.Text}+{b.Text})"));
                candidates.Add((a.Value * b.Value, $"({a.Text}*{b.Text})"));
            }
            candidates.Add((a.Value - b.Value, $"({a.Text}-{b.Text})"));
            if (allowDivision && !b.Value.IsZero) candidates.Add((a.Value / b.Value, $"({a.Text}/{b.Text})"));
            foreach (var c in candidates)
            {
                rest.Add(c);
                var found = Search(rest, allowDivision);
                rest.RemoveAt(rest.Count - 1);
                if (found != null) return found;
            }
        }
        return null;
    }

    public readonly struct Fraction : IEquatable<Fraction>
    {
        public long Num { get; }
        public long Den { get; }
        public Fraction(long num, long den)
        {
            if (den < 0) { num = -num; den = -den; }
            var g = Gcd(Math.Abs(num), den);
            Num = g == 0 ? 0 : num / g;
            Den = g == 0 ? 1 : den / g;
        }
        public bool IsZero => Num == 0;
        public static Fraction operator +(Fraction a, Fraction b) => new(checked(a.Num * b.Den + b.Num * a.Den), checked(a.Den * b.Den));
        public static Fraction operator -(Fraction a, Fraction b) => new(checked(a.Num * b.Den - b.Num * a.Den), checked(a.Den * b.Den));
        public static Fraction operator *(Fraction a, Fraction b) => new(checked(a.Num * b.Num), checked(a.Den * b.Den));
        public static Fraction operator /(Fraction a, Fraction b) => new(checked(a.Num * b.Den), checked(a.Den * b.Num));
        public bool Equals(Fraction other) => Num == other.Num && Den == other.Den;
        public override bool Equals(object? obj) => obj is Fraction f && Equals(f);
        public override int GetHashCode() => HashCode.Combine(Num, Den);
        private static long Gcd(long a, long b) { while (b != 0) (a, b) = (b, a % b); return a; }
    }

    /// <summary>
    /// Recursive-descent evaluator for digits, + - * / (also × ÷) and parentheses. Nothing is ever
    /// executed, so a submission cannot run code on the server.
    /// </summary>
    private sealed class ExpressionParser(string text)
    {
        private readonly string _text = text.Replace('×', '*').Replace('x', '*').Replace('X', '*').Replace('÷', '/').Replace('−', '-');
        private int _pos;
        private int _depth;
        private readonly List<long> _numbers = new();

        public bool TryEvaluate(out Fraction value, out List<long> numbers)
        {
            numbers = _numbers;
            value = default;
            try
            {
                value = Expression();
                SkipSpaces();
                return _pos == _text.Length;
            }
            catch (Exception ex) when (ex is FormatException or DivideByZeroException or OverflowException)
            {
                return false;
            }
        }

        private void SkipSpaces() { while (_pos < _text.Length && _text[_pos] == ' ') _pos++; }

        private Fraction Expression()
        {
            var left = Term();
            while (true)
            {
                SkipSpaces();
                if (_pos >= _text.Length || (_text[_pos] != '+' && _text[_pos] != '-')) return left;
                var op = _text[_pos++];
                var right = Term();
                left = op == '+' ? left + right : left - right;
            }
        }

        private Fraction Term()
        {
            var left = Factor();
            while (true)
            {
                SkipSpaces();
                if (_pos >= _text.Length || (_text[_pos] != '*' && _text[_pos] != '/')) return left;
                var op = _text[_pos++];
                var right = Factor();
                if (op == '/' && right.IsZero) throw new DivideByZeroException();
                left = op == '*' ? left * right : left / right;
            }
        }

        private Fraction Factor()
        {
            SkipSpaces();
            if (_pos >= _text.Length) throw new FormatException();
            if (_text[_pos] == '(')
            {
                if (++_depth > 16) throw new FormatException();
                _pos++;
                var inner = Expression();
                SkipSpaces();
                if (_pos >= _text.Length || _text[_pos] != ')') throw new FormatException();
                _pos++; _depth--;
                return inner;
            }
            var start = _pos;
            while (_pos < _text.Length && char.IsAsciiDigit(_text[_pos])) _pos++;
            if (start == _pos || _pos - start > 3) throw new FormatException();
            var n = long.Parse(_text.AsSpan(start, _pos - start));
            _numbers.Add(n);
            return new Fraction(n, 1);
        }
    }
}
