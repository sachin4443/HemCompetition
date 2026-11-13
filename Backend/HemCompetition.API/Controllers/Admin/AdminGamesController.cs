using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Games;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers.Admin;

public sealed class GameUpdateRequest
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [StringLength(1000)] public string? Description { get; set; }
    public Difficulty Difficulty { get; set; }
    [Range(1, 50)] public int Rounds { get; set; }
    [Range(0, 1000)] public decimal PointsPerRound { get; set; }
    [Range(30, 86400)] public int? TimeLimitSeconds { get; set; }
    public JsonElement? Config { get; set; }
    public bool IsActive { get; set; }
}

public sealed class WordRequest
{
    [Required, RegularExpression("^[A-Za-z]{3,20}$", ErrorMessage = "A word must be 3-20 letters with no spaces.")] public string Word { get; set; } = "";
    [StringLength(500)] public string? Clue { get; set; }
    [StringLength(100)] public string? Topic { get; set; }
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public bool IsActive { get; set; } = true;
}

[ApiController]
[Route("api/admin/games")]
[Authorize(Policy = Policies.Admin)]
public class AdminGamesController(AppDbContext db, AuditService audit, GameEngineRegistry engines) : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permissions.GamesView)]
    public async Task<IActionResult> List()
    {
        var categories = await db.GameCategories.AsNoTracking().OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id, c.Key, c.Name, c.Description, c.IsActive,
                Games = c.Games.OrderBy(g => g.SortOrder).Select(g => new
                {
                    g.Id, g.Key, g.Name, g.Description, g.EngineKey, Difficulty = g.Difficulty.ToString(), g.Rounds, g.PointsPerRound,
                    g.TimeLimitSeconds, g.Config, g.IsActive,
                    Attempts = db.GameAttempts.Count(a => a.GameId == g.Id)
                }).ToList()
            }).ToListAsync();
        return Ok(categories.Select(c => new
        {
            id = c.Id, key = c.Key, name = c.Name, description = c.Description, isActive = c.IsActive,
            games = c.Games.Select(g => new
            {
                id = g.Id, key = g.Key, name = g.Name, description = g.Description, engineKey = g.EngineKey, difficulty = g.Difficulty,
                rounds = g.Rounds, pointsPerRound = g.PointsPerRound, timeLimitSeconds = g.TimeLimitSeconds,
                config = JsonNode.Parse(g.Config), isActive = g.IsActive, attempts = g.Attempts
            })
        }));
    }

    [HttpPut("{id:int}")]
    [RequirePermission(Permissions.GamesManage)]
    public async Task<IActionResult> Update(int id, [FromBody] GameUpdateRequest model)
    {
        var game = await db.Games.FirstOrDefaultAsync(g => g.Id == id) ?? throw ApiException.NotFound("Game not found.");
        if (model.Config is { } cfg)
        {
            if (cfg.ValueKind != JsonValueKind.Object || cfg.GetRawText().Length > 2000) throw ApiException.BadRequest("Config must be a small JSON object.");
            game.Config = cfg.GetRawText();
        }
        game.Name = model.Name.Trim();
        game.Description = model.Description?.Trim();
        game.Difficulty = model.Difficulty;
        game.Rounds = model.Rounds;
        game.PointsPerRound = model.PointsPerRound;
        game.TimeLimitSeconds = model.TimeLimitSeconds;
        game.IsActive = model.IsActive;
        game.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("game.updated", "game", id, new { model.Rounds, model.Difficulty, model.IsActive });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpPut("categories/{id:int}/active")]
    [RequirePermission(Permissions.GamesManage)]
    public async Task<IActionResult> SetCategoryActive(int id, [FromQuery] bool value)
    {
        var c = await db.GameCategories.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        c.IsActive = value;
        audit.Add("game_category.updated", "game_category", id, new { isActive = value });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    /// <summary>Generates one sample round, including its answer, so a configuration can be checked before students see it.</summary>
    [HttpPost("{id:int}/sample")]
    [RequirePermission(Permissions.GamesManage)]
    public async Task<IActionResult> Sample(int id)
    {
        var game = await db.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id) ?? throw ApiException.NotFound("Game not found.");
        var engine = engines.Get(game.EngineKey);
        var ctx = new GenerationContext
        {
            Rng = new Random(), Difficulty = game.Difficulty, Config = JsonNode.Parse(game.Config) as JsonObject ?? new JsonObject(),
            Words = engine.RequiresWords
                ? await db.WordBank.AsNoTracking().Where(w => w.IsActive).Select(w => new WordEntry(w.Word, w.Clue, w.Topic)).ToListAsync()
                : []
        };
        try
        {
            var round = engine.Generate(ctx);
            return Ok(new { engine = game.EngineKey, puzzle = round.Puzzle, solution = round.Solution });
        }
        catch (GameUnavailableException ex)
        {
            throw ApiException.Conflict(ex.Message, "GAME_UNAVAILABLE");
        }
    }

    // ----------------------------------------------------------------------------- word bank

    [HttpGet("words")]
    [RequirePermission(Permissions.GamesView)]
    public async Task<IActionResult> Words([FromQuery] PageQuery page)
    {
        var q = db.WordBank.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var like = $"%{page.Search.Trim()}%";
            q = q.Where(w => EF.Functions.ILike(w.Word, like) || EF.Functions.ILike(w.Clue!, like) || EF.Functions.ILike(w.Topic!, like));
        }
        var total = await q.CountAsync();
        var items = await q.OrderBy(w => w.Word).Skip(page.Skip).Take(page.PageSize)
            .Select(w => new { id = w.Id, word = w.Word, clue = w.Clue, topic = w.Topic, difficulty = w.Difficulty.ToString(), isActive = w.IsActive }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    [HttpPost("words")]
    [RequirePermission(Permissions.GamesManage)]
    public async Task<IActionResult> AddWord([FromBody] WordRequest model)
    {
        var word = model.Word.Trim().ToUpperInvariant();
        if (await db.WordBank.AnyAsync(w => w.Word == word)) throw ApiException.Conflict("That word is already in the bank.");
        var entry = new WordBankEntry { Word = word, CreatedAtUtc = DateTime.UtcNow };
        Apply(entry, model);
        db.WordBank.Add(entry);
        audit.Add("word.created", "word", word);
        await db.SaveChangesAsync();
        return Ok(new { id = entry.Id });
    }

    [HttpPut("words/{id:int}")]
    [RequirePermission(Permissions.GamesManage)]
    public async Task<IActionResult> UpdateWord(int id, [FromBody] WordRequest model)
    {
        var entry = await db.WordBank.FirstOrDefaultAsync(w => w.Id == id) ?? throw ApiException.NotFound();
        var word = model.Word.Trim().ToUpperInvariant();
        if (word != entry.Word && await db.WordBank.AnyAsync(w => w.Word == word)) throw ApiException.Conflict("That word is already in the bank.");
        entry.Word = word;
        Apply(entry, model);
        audit.Add("word.updated", "word", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpDelete("words/{id:int}")]
    [RequirePermission(Permissions.GamesManage)]
    public async Task<IActionResult> DeleteWord(int id)
    {
        var entry = await db.WordBank.FirstOrDefaultAsync(w => w.Id == id) ?? throw ApiException.NotFound();
        db.WordBank.Remove(entry);
        audit.Add("word.deleted", "word", entry.Word);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    /// <summary>Columns: word, clue, topic, difficulty. Existing words are updated.</summary>
    [HttpPost("words/import")]
    [RequirePermission(Permissions.GamesManage)]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> ImportWords(IFormFile file)
    {
        var table = await TabularFile.ReadAsync(file);
        int cWord = table.Column("word", "answer"), cClue = table.Column("clue", "question", "hint"),
            cTopic = table.Column("topic", "category"), cDiff = table.Column("difficulty", "level");
        if (cWord < 0) throw ApiException.BadRequest("The file needs a 'word' column.");
        var existing = await db.WordBank.ToDictionaryAsync(w => w.Word);
        var errors = new List<string>();
        int added = 0, updated = 0;
        for (var i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            var word = TabularFile.Table.Cell(row, cWord).ToUpperInvariant();
            if (word.Length is < 3 or > 20 || !word.All(char.IsAsciiLetterUpper)) { errors.Add($"Row {i + 2}: '{word}' must be 3-20 letters with no spaces."); continue; }
            var difficulty = Difficulty.Medium;
            var d = TabularFile.Table.Cell(row, cDiff);
            if (d.Length > 0 && !Enum.TryParse(d, true, out difficulty)) { errors.Add($"Row {i + 2}: difficulty must be Easy, Medium or Hard."); continue; }
            if (!existing.TryGetValue(word, out var entry))
            {
                existing[word] = entry = new WordBankEntry { Word = word, CreatedAtUtc = DateTime.UtcNow, IsActive = true };
                db.WordBank.Add(entry);
                added++;
            }
            else updated++;
            var clue = TabularFile.Table.Cell(row, cClue);
            var topic = TabularFile.Table.Cell(row, cTopic);
            if (clue.Length > 0) entry.Clue = clue[..Math.Min(clue.Length, 500)];
            if (topic.Length > 0) entry.Topic = topic[..Math.Min(topic.Length, 100)];
            entry.Difficulty = difficulty;
        }
        if (errors.Count > 0)
        {
            db.ChangeTracker.Clear();
            return BadRequest(new { message = "Nothing was imported. Fix these rows and try again.", errors = errors.Take(50) });
        }
        audit.Add("word.imported", "word", null, new { added, updated });
        await db.SaveChangesAsync();
        return Ok(new { imported = added, updated });
    }

    [HttpGet("words/export")]
    [RequirePermission(Permissions.GamesView)]
    public async Task<IActionResult> ExportWords()
    {
        var words = await db.WordBank.AsNoTracking().OrderBy(w => w.Word).ToListAsync();
        return File(TabularFile.ToCsv(["word", "clue", "topic", "difficulty", "active"],
            words.Select(w => new object?[] { w.Word, w.Clue, w.Topic, w.Difficulty.ToString(), w.IsActive })), "text/csv", "word-bank.csv");
    }

    private static void Apply(WordBankEntry entry, WordRequest model)
    {
        entry.Clue = string.IsNullOrWhiteSpace(model.Clue) ? null : model.Clue.Trim();
        entry.Topic = string.IsNullOrWhiteSpace(model.Topic) ? null : model.Topic.Trim();
        entry.Difficulty = model.Difficulty;
        entry.IsActive = model.IsActive;
    }
}
