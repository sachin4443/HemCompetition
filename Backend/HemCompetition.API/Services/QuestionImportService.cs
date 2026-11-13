using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services;

public sealed record ImportResult(int Imported, int Skipped, List<string> Errors);

/// <summary>Bulk import of questions from CSV/Excel. Nothing is saved unless every row is valid.</summary>
public sealed class QuestionImportService(AppDbContext db)
{
    public const string Template =
        "category,question,option_a,option_b,option_c,option_d,correct,explanation,difficulty,marks\n" +
        "General Knowledge,What is the capital of India?,Mumbai,New Delhi,Kolkata,Chennai,B,New Delhi is the national capital.,Easy,1\n";

    public async Task<ImportResult> ImportAsync(TabularFile.Table table, int? defaultCategoryId)
    {
        int cCategory = table.Column("category", "bank", "questionbank"),
            cText = table.Column("question", "questiontext", "text", "questionname"),
            cCorrect = table.Column("correct", "answer", "correctoption", "correctanswer"),
            cExplanation = table.Column("explanation", "answerdescription"),
            cDifficulty = table.Column("difficulty", "complexitylevel", "level"),
            cMarks = table.Column("marks", "mark");
        var optionColumns = "abcdef".Select((letter, i) => table.Column($"option{letter}", $"option{i + 1}", $"opt{letter}")).Where(i => i >= 0).ToList();

        var errors = new List<string>();
        if (cText < 0) errors.Add("Missing column: question.");
        if (cCorrect < 0) errors.Add("Missing column: correct.");
        if (optionColumns.Count < 2) errors.Add("At least two option columns (option_a, option_b, ...) are required.");
        if (cCategory < 0 && defaultCategoryId == null) errors.Add("Add a category column or choose a question bank to import into.");
        if (errors.Count > 0) return new ImportResult(0, 0, errors);

        var categories = await db.QuizCategories.ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
        var defaultCategory = defaultCategoryId == null ? null : categories.Values.FirstOrDefault(c => c.Id == defaultCategoryId)
            ?? throw ApiException.BadRequest("The selected question bank does not exist.");
        var existing = (await db.Questions.Where(q => !q.IsDeleted).Select(q => new { q.CategoryId, q.Text }).ToListAsync())
            .Select(q => (q.CategoryId, Key(q.Text))).ToHashSet();

        var questions = new List<Question>();
        var skipped = 0;
        var now = DateTime.UtcNow;
        for (var i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            var line = i + 2;
            var text = TabularFile.Table.Cell(row, cText);
            if (text.Length == 0) { errors.Add($"Row {line}: the question is empty."); continue; }
            if (text.Length > 4000) { errors.Add($"Row {line}: the question is longer than 4000 characters."); continue; }

            var options = optionColumns.Select(c => TabularFile.Table.Cell(row, c)).ToList();
            while (options.Count > 0 && options[^1].Length == 0) options.RemoveAt(options.Count - 1);
            if (options.Count < 2 || options.Any(o => o.Length == 0)) { errors.Add($"Row {line}: at least two options are needed, with no gaps."); continue; }
            if (options.Any(o => o.Length > 1000)) { errors.Add($"Row {line}: an option is longer than 1000 characters."); continue; }

            // "B", "2", or the option text itself
            var correctRaw = TabularFile.Table.Cell(row, cCorrect);
            var correct = correctRaw.Length == 1 && char.IsLetter(correctRaw[0]) ? char.ToUpperInvariant(correctRaw[0]) - 'A'
                : int.TryParse(correctRaw, out var n) ? n - 1
                : options.FindIndex(o => o.Equals(correctRaw, StringComparison.OrdinalIgnoreCase));
            if (correct < 0 || correct >= options.Count) { errors.Add($"Row {line}: 'correct' must be the letter, number or text of one of the options."); continue; }

            var difficulty = Difficulty.Medium;
            var difficultyRaw = TabularFile.Table.Cell(row, cDifficulty);
            if (difficultyRaw.Length > 0 && !Enum.TryParse(difficultyRaw, true, out difficulty)) { errors.Add($"Row {line}: difficulty must be Easy, Medium or Hard."); continue; }

            decimal marks = 1;
            var marksRaw = TabularFile.Table.Cell(row, cMarks);
            if (marksRaw.Length > 0 && (!decimal.TryParse(marksRaw, System.Globalization.CultureInfo.InvariantCulture, out marks) || marks <= 0 || marks > 100))
            { errors.Add($"Row {line}: marks must be a number between 0 and 100."); continue; }

            var category = defaultCategory;
            var categoryName = TabularFile.Table.Cell(row, cCategory);
            if (categoryName.Length > 0 && !categories.TryGetValue(categoryName, out category))
            {
                if (categoryName.Length > 150) { errors.Add($"Row {line}: the category name is too long."); continue; }
                category = new QuizCategory { Name = categoryName, CreatedAtUtc = now };
                categories[categoryName] = category;
                db.QuizCategories.Add(category);
            }
            if (category == null) { errors.Add($"Row {line}: no category."); continue; }

            // Re-importing the same file does not create duplicates.
            if (category.Id != 0 && !existing.Add((category.Id, Key(text)))) { skipped++; continue; }

            var explanation = TabularFile.Table.Cell(row, cExplanation);
            questions.Add(new Question
            {
                Category = category, Text = text, Explanation = explanation.Length == 0 ? null : explanation[..Math.Min(explanation.Length, 4000)],
                Difficulty = difficulty, Marks = marks, IsActive = true, CreatedAtUtc = now,
                Options = options.Select((o, idx) => new QuestionOption { Text = o, IsCorrect = idx == correct, SortOrder = idx }).ToList()
            });
        }

        if (errors.Count > 0)
        {
            db.ChangeTracker.Clear();
            return new ImportResult(0, skipped, errors.Take(50).ToList());
        }
        db.Questions.AddRange(questions);
        await db.SaveChangesAsync();
        return new ImportResult(questions.Count, skipped, errors);
    }

    private static string Key(string text) => new(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}
