using System.ComponentModel.DataAnnotations;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers.Admin;

public sealed class CategoryRequest
{
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [StringLength(1000)] public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class OptionRequest
{
    [Required, StringLength(1000)] public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
}

public sealed class QuestionRequest
{
    public int CategoryId { get; set; }
    [Required, StringLength(4000)] public string Text { get; set; } = "";
    [StringLength(4000)] public string? Explanation { get; set; }
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    [Range(0.25, 100)] public decimal Marks { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    [Required, MinLength(2), MaxLength(8)] public List<OptionRequest> Options { get; set; } = new();
}

public sealed class QuizRequest
{
    [Required, StringLength(200)] public string Title { get; set; } = "";
    [StringLength(2000)] public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public QuizSelectionMode SelectionMode { get; set; } = QuizSelectionMode.Random;
    public Difficulty? Difficulty { get; set; }
    [Range(1, 500)] public int QuestionCount { get; set; } = 10;
    [Range(1, 600)] public int DurationMinutes { get; set; } = 10;
    [Range(0.25, 100)] public decimal? MarksPerQuestion { get; set; }
    [Range(0, 100)] public decimal NegativeMarks { get; set; }
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
    public int SortOrder { get; set; }
}

/// <summary>
/// Question bank and quiz management. These are the only endpoints that ever return answer keys,
/// and all of them require an admin token with a quiz permission.
/// </summary>
[ApiController]
[Route("api/admin/quiz")]
[Authorize(Policy = Policies.Admin)]
public class AdminQuizController(
    AppDbContext db, AuditService audit, QuizAttemptService quizService, QuestionImportService importer, IFileStorage storage) : ControllerBase
{
    // ----------------------------------------------------------------------------- categories

    [HttpGet("categories")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Categories() =>
        Ok(await db.QuizCategories.AsNoTracking().OrderBy(c => c.Name).Select(c => new
        {
            id = c.Id, name = c.Name, description = c.Description, isActive = c.IsActive,
            questions = db.Questions.Count(q => q.CategoryId == c.Id && !q.IsDeleted),
            activeQuestions = db.Questions.Count(q => q.CategoryId == c.Id && !q.IsDeleted && q.IsActive)
        }).ToListAsync());

    [HttpPost("categories")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest model)
    {
        var name = model.Name.Trim();
        if (await db.QuizCategories.AnyAsync(c => c.Name == name)) throw ApiException.Conflict("A question bank with that name already exists.");
        var c = new QuizCategory { Name = name, Description = model.Description?.Trim(), IsActive = model.IsActive, CreatedAtUtc = DateTime.UtcNow };
        db.QuizCategories.Add(c);
        audit.Add("quiz_category.created", "quiz_category", name);
        await db.SaveChangesAsync();
        return Ok(new { id = c.Id });
    }

    [HttpPut("categories/{id:int}")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest model)
    {
        var c = await db.QuizCategories.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        var name = model.Name.Trim();
        if (name != c.Name && await db.QuizCategories.AnyAsync(x => x.Name == name)) throw ApiException.Conflict("A question bank with that name already exists.");
        c.Name = name;
        c.Description = model.Description?.Trim();
        c.IsActive = model.IsActive;
        audit.Add("quiz_category.updated", "quiz_category", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpDelete("categories/{id:int}")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var c = await db.QuizCategories.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        if (await db.Questions.AnyAsync(q => q.CategoryId == id && !q.IsDeleted) || await db.Quizzes.AnyAsync(q => q.CategoryId == id && !q.IsDeleted))
            throw ApiException.Conflict("This question bank still has questions or is used by a quiz.");
        // Soft-deleted questions and quizzes still reference the row, so retire it instead of deleting.
        if (await db.Questions.AnyAsync(q => q.CategoryId == id) || await db.Quizzes.AnyAsync(q => q.CategoryId == id))
        {
            c.IsActive = false;
            c.Name = $"{c.Name} (deleted {DateTime.UtcNow:yyyyMMddHHmmss})";
        }
        else db.QuizCategories.Remove(c);
        audit.Add("quiz_category.deleted", "quiz_category", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    // ----------------------------------------------------------------------------- questions

    [HttpGet("questions")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Questions([FromQuery] PageQuery page, [FromQuery] int? categoryId, [FromQuery] Difficulty? difficulty, [FromQuery] bool? active)
    {
        var q = db.Questions.AsNoTracking().Where(x => !x.IsDeleted);
        if (categoryId != null) q = q.Where(x => x.CategoryId == categoryId);
        if (difficulty != null) q = q.Where(x => x.Difficulty == difficulty);
        if (active != null) q = q.Where(x => x.IsActive == active);
        if (!string.IsNullOrWhiteSpace(page.Search)) q = q.Where(x => EF.Functions.ILike(x.Text, $"%{page.Search.Trim()}%"));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(x => x.Id).Skip(page.Skip).Take(page.PageSize).Select(x => new
        {
            id = x.Id, categoryId = x.CategoryId, category = x.Category.Name, text = x.Text, explanation = x.Explanation,
            difficulty = x.Difficulty.ToString(), marks = x.Marks, isActive = x.IsActive, hasImage = x.ImageKey != null,
            options = x.Options.OrderBy(o => o.SortOrder).ThenBy(o => o.Id).Select(o => new { id = o.Id, text = o.Text, isCorrect = o.IsCorrect }).ToList()
        }).AsSplitQuery().ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    [HttpPost("questions")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> CreateQuestion([FromBody] QuestionRequest model)
    {
        await ValidateAsync(model);
        var question = new Question { CreatedAtUtc = DateTime.UtcNow };
        Apply(question, model);
        db.Questions.Add(question);
        audit.Add("question.created", "question", null, new { model.CategoryId });
        await db.SaveChangesAsync();
        return Ok(new { id = question.Id });
    }

    [HttpPut("questions/{id:int}")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionRequest model)
    {
        await ValidateAsync(model);
        var question = await db.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Question not found.");
        // Options are replaced as a set. Past attempts are unaffected: they hold their own snapshot.
        db.QuestionOptions.RemoveRange(question.Options);
        question.Options.Clear();
        Apply(question, model);
        question.UpdatedAtUtc = DateTime.UtcNow;
        await BumpQuizVersionsAsync(id);
        audit.Add("question.updated", "question", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpDelete("questions/{id:int}")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await db.Questions.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Question not found.");
        question.IsDeleted = true;
        question.IsActive = false;
        question.UpdatedAtUtc = DateTime.UtcNow;
        await BumpQuizVersionsAsync(id);
        await db.QuizQuestions.Where(x => x.QuestionId == id).ExecuteDeleteAsync();
        audit.Add("question.deleted", "question", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpPut("questions/{id:int}/active")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> SetQuestionActive(int id, [FromQuery] bool value)
    {
        var question = await db.Questions.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Question not found.");
        question.IsActive = value;
        question.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add(value ? "question.enabled" : "question.disabled", "question", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpPost("questions/{id:int}/image")]
    [RequirePermission(Permissions.QuizManage)]
    [RequestSizeLimit(3_000_000)]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var question = await db.Questions.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Question not found.");
        var (content, contentType) = await FileSniffer.ReadVerifiedAsync(file, 2 * 1024 * 1024, ["image/jpeg", "image/png", "image/webp"]);
        await using var _ = content;
        // A new key per upload: attempts that already snapshotted the old image keep showing it.
        var key = $"questions/{id}/{Guid.NewGuid():N}{FileSniffer.Extension(contentType)}";
        await storage.SaveAsync(key, content);
        question.ImageKey = key;
        question.ImageContentType = contentType;
        question.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("question.image_uploaded", "question", id);
        await db.SaveChangesAsync();
        return Ok(new { id, hasImage = true });
    }

    [HttpDelete("questions/{id:int}/image")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> RemoveImage(int id)
    {
        var question = await db.Questions.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Question not found.");
        question.ImageKey = null;
        question.ImageContentType = null;
        audit.Add("question.image_removed", "question", id);
        await db.SaveChangesAsync();
        return Ok(new { id, hasImage = false });
    }

    [HttpGet("questions/{id:int}/image")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Image(int id)
    {
        var q = await db.Questions.AsNoTracking().Where(x => x.Id == id).Select(x => new { x.ImageKey, x.ImageContentType }).FirstOrDefaultAsync();
        var stream = q?.ImageKey == null ? null : await storage.OpenAsync(q.ImageKey);
        return stream == null ? NotFound() : File(stream, q!.ImageContentType ?? "application/octet-stream");
    }

    [HttpGet("questions/template")]
    [RequirePermission(Permissions.QuizView)]
    public IActionResult Template() =>
        File(System.Text.Encoding.UTF8.GetBytes(QuestionImportService.Template), "text/csv", "question-import-template.csv");

    [HttpPost("questions/import")]
    [RequirePermission(Permissions.QuizManage)]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> Import(IFormFile file, [FromQuery] int? categoryId)
    {
        var result = await importer.ImportAsync(await TabularFile.ReadAsync(file), categoryId);
        if (result.Errors.Count > 0)
            return BadRequest(new { message = "Nothing was imported. Fix these rows and try again.", errors = result.Errors });
        await audit.LogAsync("question.imported", "question", null, new { result.Imported, result.Skipped, categoryId });
        return Ok(new { imported = result.Imported, skipped = result.Skipped });
    }

    [HttpGet("questions/export")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Export([FromQuery] int? categoryId)
    {
        var questions = await db.Questions.AsNoTracking().Where(q => !q.IsDeleted && (categoryId == null || q.CategoryId == categoryId))
            .Include(q => q.Category).Include(q => q.Options).OrderBy(q => q.CategoryId).ThenBy(q => q.Id).AsSplitQuery().ToListAsync();
        var maxOptions = Math.Max(4, questions.Count == 0 ? 0 : questions.Max(q => q.Options.Count));
        var headers = new List<string> { "category", "question" };
        headers.AddRange(Enumerable.Range(0, maxOptions).Select(i => $"option_{(char)('a' + i)}"));
        headers.AddRange(["correct", "explanation", "difficulty", "marks", "active"]);
        var rows = questions.Select(q =>
        {
            var options = q.Options.OrderBy(o => o.SortOrder).ThenBy(o => o.Id).ToList();
            var row = new List<object?> { q.Category.Name, q.Text };
            row.AddRange(Enumerable.Range(0, maxOptions).Select(i => (object?)(i < options.Count ? options[i].Text : "")));
            var correct = options.FindIndex(o => o.IsCorrect);
            row.AddRange([correct < 0 ? "" : ((char)('A' + correct)).ToString(), q.Explanation, q.Difficulty.ToString(), q.Marks, q.IsActive]);
            return row;
        });
        await audit.LogAsync("question.exported", "question", null, new { categoryId, count = questions.Count });
        return File(TabularFile.ToCsv(headers, rows), "text/csv", "questions.csv");
    }

    // ----------------------------------------------------------------------------- quizzes

    [HttpGet("quizzes")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Quizzes()
    {
        var list = await db.Quizzes.AsNoTracking().Where(q => !q.IsDeleted).OrderBy(q => q.SortOrder).ThenBy(q => q.Id)
            .Select(q => new { Quiz = q, Category = q.Category!.Name, Attached = q.Questions.Count, Attempts = db.QuizAttempts.Count(a => a.QuizId == q.Id) })
            .ToListAsync();
        var result = new List<object>();
        foreach (var x in list)
            result.Add(Shape(x.Quiz, x.Category, x.Attached, x.Attempts, await quizService.Pool(x.Quiz).CountAsync()));
        return Ok(result);
    }

    [HttpGet("quizzes/{id:int}")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Quiz(int id)
    {
        var quiz = await db.Quizzes.AsNoTracking().Include(q => q.Category).FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        var attached = await db.QuizQuestions.AsNoTracking().Where(x => x.QuizId == id).OrderBy(x => x.SortOrder)
            .Select(x => new { id = x.QuestionId, text = x.Question.Text, category = x.Question.Category.Name, difficulty = x.Question.Difficulty.ToString(), isActive = x.Question.IsActive }).ToListAsync();
        return Ok(new
        {
            quiz = Shape(quiz, quiz.Category?.Name, attached.Count, await db.QuizAttempts.CountAsync(a => a.QuizId == id), await quizService.Pool(quiz).CountAsync()),
            questions = attached
        });
    }

    [HttpPost("quizzes")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizRequest model)
    {
        await ValidateAsync(model);
        var quiz = new Quiz { CreatedAtUtc = DateTime.UtcNow };
        Apply(quiz, model);
        db.Quizzes.Add(quiz);
        audit.Add("quiz.created", "quiz", null, new { model.Title });
        await db.SaveChangesAsync();
        return Ok(new { id = quiz.Id });
    }

    [HttpPut("quizzes/{id:int}")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> UpdateQuiz(int id, [FromBody] QuizRequest model)
    {
        await ValidateAsync(model);
        var quiz = await db.Quizzes.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        Apply(quiz, model);
        quiz.Version++;
        quiz.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("quiz.updated", "quiz", id, new { quiz.Version });
        await db.SaveChangesAsync();
        return Ok(new { id, version = quiz.Version });
    }

    /// <summary>Replaces the quiz's explicit question list. An empty list means "use the whole question bank".</summary>
    [HttpPut("quizzes/{id:int}/questions")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> SetQuestions(int id, [FromBody] List<int> questionIds)
    {
        var quiz = await db.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        var ids = questionIds.Distinct().ToList();
        if (ids.Count > 500) throw ApiException.BadRequest("A quiz can hold at most 500 questions.");
        var valid = await db.Questions.Where(q => ids.Contains(q.Id) && !q.IsDeleted).Select(q => q.Id).ToListAsync();
        if (valid.Count != ids.Count) throw ApiException.BadRequest("Some of the selected questions no longer exist.");
        db.QuizQuestions.RemoveRange(quiz.Questions);
        quiz.Questions = ids.Select((q, i) => new QuizQuestion { QuizId = id, QuestionId = q, SortOrder = i }).ToList();
        quiz.Version++;
        quiz.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("quiz.questions_changed", "quiz", id, new { count = ids.Count });
        await db.SaveChangesAsync();
        return Ok(new { id, count = ids.Count });
    }

    /// <summary>A sample paper exactly as the selection rules would produce it, with the answer key.</summary>
    [HttpGet("quizzes/{id:int}/preview")]
    [RequirePermission(Permissions.QuizView)]
    public async Task<IActionResult> Preview(int id)
    {
        var quiz = await db.Quizzes.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        var paper = await quizService.BuildPaperAsync(quiz, new Random());
        return Ok(new
        {
            title = quiz.Title, durationMinutes = quiz.DurationMinutes, negativeMarks = quiz.NegativeMarks,
            maxScore = paper.Sum(p => p.Snapshot.Marks),
            questions = paper.Select((p, i) => new
            {
                orderNo = i + 1, questionId = p.Question.Id, text = p.Snapshot.Text, marks = p.Snapshot.Marks, difficulty = p.Snapshot.Difficulty,
                hasImage = p.Snapshot.ImageKey != null, explanation = p.Snapshot.Explanation,
                options = p.Snapshot.Options.Select(o => new { id = o.Id, text = o.Text, isCorrect = o.Id == p.Snapshot.CorrectOptionId })
            })
        });
    }

    [HttpPost("quizzes/{id:int}/publish")]
    [RequirePermission(Permissions.QuizPublish)]
    public async Task<IActionResult> Publish(int id)
    {
        var quiz = await db.Quizzes.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        var available = await quizService.Pool(quiz).CountAsync();
        var needed = quiz.SelectionMode == QuizSelectionMode.Random ? quiz.QuestionCount : 1;
        if (available < needed)
            throw ApiException.Conflict($"This quiz needs {needed} usable question(s) but only {available} are available. " +
                                        "A usable question is active and has at least two options with exactly one marked correct.", "QUIZ_NOT_READY");
        quiz.IsPublished = true;
        quiz.PublishedAtUtc = DateTime.UtcNow;
        quiz.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("quiz.published", "quiz", id, new { quiz.Version });
        await db.SaveChangesAsync();
        return Ok(new { id, isPublished = true });
    }

    [HttpPost("quizzes/{id:int}/unpublish")]
    [RequirePermission(Permissions.QuizPublish)]
    public async Task<IActionResult> Unpublish(int id)
    {
        var quiz = await db.Quizzes.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        quiz.IsPublished = false;
        quiz.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("quiz.unpublished", "quiz", id);
        await db.SaveChangesAsync();
        return Ok(new { id, isPublished = false });
    }

    [HttpDelete("quizzes/{id:int}")]
    [RequirePermission(Permissions.QuizManage)]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        var quiz = await db.Quizzes.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted) ?? throw ApiException.NotFound("Quiz not found.");
        // Soft delete: attempts and results that reference the quiz stay intact.
        quiz.IsDeleted = true;
        quiz.IsPublished = false;
        quiz.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("quiz.deleted", "quiz", id, new { quiz.Title });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    // ----------------------------------------------------------------------------- helpers

    private static object Shape(Quiz q, string? category, int attached, int attempts, int usable) => new
    {
        id = q.Id, title = q.Title, description = q.Description, categoryId = q.CategoryId, category,
        selectionMode = q.SelectionMode.ToString(), difficulty = q.Difficulty?.ToString(), questionCount = q.QuestionCount,
        durationMinutes = q.DurationMinutes, marksPerQuestion = q.MarksPerQuestion, negativeMarks = q.NegativeMarks,
        shuffleQuestions = q.ShuffleQuestions, shuffleOptions = q.ShuffleOptions, isPublished = q.IsPublished,
        publishedAtUtc = q.PublishedAtUtc, version = q.Version, sortOrder = q.SortOrder,
        attachedQuestions = attached, usableQuestions = usable, attempts
    };

    private async Task ValidateAsync(QuestionRequest model)
    {
        if (!await db.QuizCategories.AnyAsync(c => c.Id == model.CategoryId)) throw ApiException.BadRequest("Choose a question bank.");
        if (model.Options.Any(o => string.IsNullOrWhiteSpace(o.Text))) throw ApiException.BadRequest("Options cannot be empty.");
        if (model.Options.Count(o => o.IsCorrect) != 1) throw ApiException.BadRequest("Mark exactly one option as the correct answer.");
        if (model.Options.Select(o => o.Text.Trim().ToLowerInvariant()).Distinct().Count() != model.Options.Count)
            throw ApiException.BadRequest("Two options have the same text.");
    }

    private async Task ValidateAsync(QuizRequest model)
    {
        if (model.CategoryId != null && !await db.QuizCategories.AnyAsync(c => c.Id == model.CategoryId))
            throw ApiException.BadRequest("The selected question bank does not exist.");
    }

    private static void Apply(Question q, QuestionRequest model)
    {
        q.CategoryId = model.CategoryId;
        q.Text = model.Text.Trim();
        q.Explanation = string.IsNullOrWhiteSpace(model.Explanation) ? null : model.Explanation.Trim();
        q.Difficulty = model.Difficulty;
        q.Marks = model.Marks;
        q.IsActive = model.IsActive;
        q.Options = model.Options.Select((o, i) => new QuestionOption { Text = o.Text.Trim(), IsCorrect = o.IsCorrect, SortOrder = i }).ToList();
    }

    private static void Apply(Quiz q, QuizRequest model)
    {
        q.Title = model.Title.Trim();
        q.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        q.CategoryId = model.CategoryId;
        q.SelectionMode = model.SelectionMode;
        q.Difficulty = model.Difficulty;
        q.QuestionCount = model.QuestionCount;
        q.DurationMinutes = model.DurationMinutes;
        q.MarksPerQuestion = model.MarksPerQuestion;
        q.NegativeMarks = model.NegativeMarks;
        q.ShuffleQuestions = model.ShuffleQuestions;
        q.ShuffleOptions = model.ShuffleOptions;
        q.SortOrder = model.SortOrder;
    }

    /// <summary>Any quiz that can draw this question gets a new version number.</summary>
    private async Task BumpQuizVersionsAsync(int questionId)
    {
        var categoryId = await db.Questions.Where(q => q.Id == questionId).Select(q => q.CategoryId).FirstAsync();
        await db.Quizzes.Where(q => !q.IsDeleted && (q.Questions.Any(x => x.QuestionId == questionId) || (q.CategoryId == categoryId && !q.Questions.Any())))
            .ExecuteUpdateAsync(u => u.SetProperty(q => q.Version, q => q.Version + 1));
    }
}
