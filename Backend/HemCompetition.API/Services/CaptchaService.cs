using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace HemCompetition.API.Services;

public sealed record CaptchaChallenge(string Id, string Code, string Svg);

/// <summary>
/// Self-hosted login captcha: a five-digit code drawn as a noisy dot-matrix SVG (no text in the
/// markup, no third-party service). Each challenge lives for three minutes and can be used once.
/// It slows down scripted sign-in attempts; the account lockout and rate limits remain the real
/// defence against password guessing.
/// </summary>
public sealed class CaptchaService(IMemoryCache cache)
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(3);
    private const int Length = 5;

    // 5x7 dot-matrix digits
    private static readonly string[][] Glyphs =
    [
        ["01110", "10001", "10011", "10101", "11001", "10001", "01110"],
        ["00100", "01100", "00100", "00100", "00100", "00100", "01110"],
        ["01110", "10001", "00001", "00010", "00100", "01000", "11111"],
        ["11111", "00010", "00100", "00010", "00001", "10001", "01110"],
        ["00010", "00110", "01010", "10010", "11111", "00010", "00010"],
        ["11111", "10000", "11110", "00001", "00001", "10001", "01110"],
        ["00110", "01000", "10000", "11110", "10001", "10001", "01110"],
        ["11111", "00001", "00010", "00100", "01000", "01000", "01000"],
        ["01110", "10001", "10001", "01110", "10001", "10001", "01110"],
        ["01110", "10001", "10001", "01111", "00001", "00010", "01100"],
    ];

    private static string Key(string id) => "captcha:" + id;

    public CaptchaChallenge Create()
    {
        var code = string.Concat(Enumerable.Range(0, Length).Select(_ => RandomNumberGenerator.GetInt32(10).ToString()));
        var id = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
        cache.Set(Key(id), code, Lifetime);
        return new CaptchaChallenge(id, code, Render(code));
    }

    /// <summary>True when the answer matches. A wrong answer uses the challenge up.</summary>
    public bool Check(string? id, string? answer)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length > 64 || !cache.TryGetValue(Key(id), out string? code) || code == null) return false;
        var ok = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(code), Encoding.UTF8.GetBytes((answer ?? "").Trim()));
        if (!ok) cache.Remove(Key(id));
        return ok;
    }

    /// <summary>Marks a correctly answered challenge as used.</summary>
    public void Consume(string? id)
    {
        if (!string.IsNullOrEmpty(id)) cache.Remove(Key(id));
    }

    private static string Render(string code)
    {
        const int width = 220, height = 70;
        var rng = Random.Shared;
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.Append(inv, $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {width} {height}\" width=\"{width}\" height=\"{height}\" role=\"img\">");
        sb.Append(inv, $"<rect width=\"{width}\" height=\"{height}\" fill=\"#f1f5f9\"/>");

        var dots = new List<(double X, double Y, double R, string Fill)>();
        for (var i = 0; i < code.Length; i++)
        {
            var glyph = Glyphs[code[i] - '0'];
            var cell = 5.6 + rng.NextDouble() * 1.2;
            var originX = 14 + i * 40 + rng.NextDouble() * 6;
            var originY = 10 + rng.NextDouble() * 10;
            var skew = (rng.NextDouble() - 0.5) * 0.5;
            var colour = $"#{rng.Next(0, 50):x2}{rng.Next(20, 80):x2}{rng.Next(40, 110):x2}";
            for (var r = 0; r < 7; r++)
                for (var c = 0; c < 5; c++)
                    if (glyph[r][c] == '1')
                        dots.Add((originX + c * cell + (r - 3) * skew * cell + (rng.NextDouble() - 0.5) * 1.6,
                            originY + r * cell + (rng.NextDouble() - 0.5) * 1.6, cell * (0.5 + rng.NextDouble() * 0.12), colour));
        }
        // Decoy dots in the same style, and they are shuffled in with the real ones.
        for (var i = 0; i < 40; i++)
            dots.Add((rng.NextDouble() * width, rng.NextDouble() * height, 0.8 + rng.NextDouble() * 1.2,
                $"#{rng.Next(150, 210):x2}{rng.Next(160, 215):x2}{rng.Next(170, 225):x2}"));
        foreach (var d in dots.OrderBy(_ => rng.Next()))
            sb.Append(inv, $"<circle cx=\"{d.X:0.0}\" cy=\"{d.Y:0.0}\" r=\"{d.R:0.0}\" fill=\"{d.Fill}\"/>");
        for (var i = 0; i < 3; i++)
            sb.Append(inv, $"<path d=\"M0 {rng.Next(height)} C{rng.Next(width)} {rng.Next(height)}, {rng.Next(width)} {rng.Next(height)}, {width} {rng.Next(height)}\" stroke=\"#64748b\" stroke-opacity=\".35\" stroke-width=\"1.2\" fill=\"none\"/>");
        sb.Append("</svg>");
        return sb.ToString();
    }
}
