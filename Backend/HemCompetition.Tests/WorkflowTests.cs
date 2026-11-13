using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Domain;
using HemCompetition.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HemCompetition.Tests;

public class WorkflowTests(TestApp app) : IClassFixture<TestApp>
{
    // ------------------------------------------------------------------ helpers

    /// <summary>Builds a correct submission from the server-held solution (which only the test can read).</summary>
    private async Task<object> CorrectAnswerAsync(Guid attemptId, int roundNo)
    {
        var (engine, solutionJson) = await app.DbAsync(async db =>
        {
            var r = await db.GameAttemptRounds.Include(x => x.Attempt).ThenInclude(a => a.Game).FirstAsync(x => x.AttemptId == attemptId && x.RoundNo == roundNo);
            return (r.Attempt.Game.EngineKey, r.Solution);
        });
        var s = JsonNode.Parse(solutionJson)!;
        return engine switch
        {
            "game24" => new { expression = s["example"]!.GetValue<string>() },
            "kenken" or "futoshiki" => new { grid = s["grid"] },
            "scramble" => new { words = s["rows"]!.AsArray().Select(r => r![0]!.GetValue<string>()), final = s["final"]![0]!.GetValue<string>() },
            "puzzle" => new
            {
                found = s["placements"]!.AsArray().Select(p =>
                {
                    var len = p!["word"]!.GetValue<string>().Length - 1;
                    int r = p["row"]!.GetValue<int>(), c = p["col"]!.GetValue<int>();
                    return new { start = new[] { r, c }, end = new[] { r + p["dr"]!.GetValue<int>() * len, c + p["dc"]!.GetValue<int>() * len } };
                })
            },
            _ => new { answer = s["answer"]!.GetValue<string>() }
        };
    }

    private async Task<int> PublishedQuizAsync(HttpClient admin, int questions = 6, int pick = 4, decimal negative = 0.25m)
    {
        var bank = (await admin.PostJson("/api/admin/quiz/categories", new { name = "Bank " + Guid.NewGuid().ToString("N")[..6] })).GetProperty("id").GetInt32();
        for (var i = 0; i < questions; i++)
            await admin.PostJson("/api/admin/quiz/questions", new
            {
                categoryId = bank, text = $"Question {i}: which option is RIGHT?", explanation = "Because it says so.", marks = 2,
                options = new[] { new { text = "RIGHT", isCorrect = true }, new { text = "wrong one", isCorrect = false }, new { text = "wrong two", isCorrect = false } }
            });
        var quiz = (await admin.PostJson("/api/admin/quiz/quizzes", new
        {
            title = "Quiz " + Guid.NewGuid().ToString("N")[..6], categoryId = bank, selectionMode = "Random", questionCount = pick,
            durationMinutes = 10, negativeMarks = negative, shuffleQuestions = true, shuffleOptions = true
        })).GetProperty("id").GetInt32();
        await admin.PostJson($"/api/admin/quiz/quizzes/{quiz}/publish");
        return quiz;
    }

    private static async Task ConsentAsync(HttpClient student, bool evidence = true, bool video = true) =>
        await student.PostJson("/api/proctoring/consent", new { acceptTerms = true, acceptEvidenceCapture = evidence, acceptVideoCapture = video });

    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xD9];

    /// <summary>Uploads evidence the way the browser does: with the one-event token from the report response.</summary>
    private async Task<HttpResponseMessage> SendEvidence(JsonElement outcome, byte[] bytes, string name, string type, Guid? eventId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/proctoring/events/{eventId ?? outcome.GetProperty("eventId").GetGuid()}/evidence") { Content = Upload(bytes, name, type) };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", outcome.GetProperty("uploadToken").GetString());
        return await app.Anonymous().SendAsync(request);
    }

    private static MultipartFormDataContent Upload(byte[] bytes, string name, string type)
    {
        var part = new ByteArrayContent(bytes);
        part.Headers.ContentType = new MediaTypeHeaderValue(type);
        return new MultipartFormDataContent { { part, "file", name } };
    }

    // ------------------------------------------------------------------ generated games

    [Theory]
    [InlineData("game24")]
    [InlineData("kenken")]
    [InlineData("futoshiki")]
    [InlineData("scramble")]
    [InlineData("puzzle")]
    [InlineData("word")]
    public async Task Every_game_can_be_played_end_to_end_and_is_scored_by_the_server(string game)
    {
        await app.LiveCompetitionAsync();
        var (student, studentId) = await app.StudentAsync();

        var start = await student.PostAsync($"/api/games/{game}/start", JsonContent.Create(new { }));
        var raw = await start.Content.ReadAsStringAsync();
        var attempt = await start.Json();
        var id = attempt.GetProperty("id").GetGuid();
        // Nothing that could be an answer key reaches the browser.
        Assert.DoesNotContain("solution", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("example", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("placements", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"answer\":", raw);

        // Starting again (double click, refresh) resumes the same attempt.
        Assert.Equal(id, (await student.PostJson($"/api/games/{game}/start")).GetProperty("id").GetGuid());

        // Round 1: a wrong answer keeps the round open.
        var wrong = await student.PostJson($"/api/games/attempts/{id}/submit", new { roundNo = 1, answer = new { expression = "1+1", grid = new[] { new[] { 1 } }, words = new[] { "X" }, final = "X", found = Array.Empty<object>(), answer = "X" } });
        Assert.False(wrong.GetProperty("correct").GetBoolean());
        Assert.Equal(1, wrong.GetProperty("attempt").GetProperty("currentRound").GetInt32());

        // Rounds 1-3 answered correctly.
        for (var round = 1; round <= 3; round++)
        {
            var ok = await student.PostJson($"/api/games/attempts/{id}/submit", new { roundNo = round, answer = await CorrectAnswerAsync(id, round) });
            Assert.True(ok.GetProperty("correct").GetBoolean(), ok.ToString());
        }
        // A replayed submission for a closed round is refused, so a round can never score twice.
        var replay = await student.PostAsJsonAsync($"/api/games/attempts/{id}/submit", new { roundNo = 3, answer = await CorrectAnswerAsync(id, 3) });
        Assert.Equal(HttpStatusCode.Conflict, replay.StatusCode);

        // Round 4 skipped with "Next", then the game is left with "Skip".
        Assert.Equal(5, (await student.PostJson($"/api/games/attempts/{id}/next", new { roundNo = 4 })).GetProperty("currentRound").GetInt32());
        Assert.Equal("Quit", (await student.PostJson($"/api/games/attempts/{id}/quit")).GetProperty("status").GetString());
        Assert.Equal(HttpStatusCode.Conflict, (await student.PostAsJsonAsync($"/api/games/attempts/{id}/submit", new { roundNo = 5, answer = await CorrectAnswerAsync(id, 5) })).StatusCode);

        var stored = await app.DbAsync(db => db.GameAttempts.Include(a => a.Rounds).FirstAsync(a => a.Id == id));
        Assert.Equal((30m, 50m, 3), (stored.Score, stored.MaxScore, stored.CorrectCount));
        Assert.Equal(1, stored.Rounds.First(r => r.RoundNo == 1).WrongSubmissions);
        Assert.All(stored.Rounds, r => Assert.NotEqual(RoundStatus.Pending, r.Status));

        // Another student cannot see or touch this attempt.
        var (other, _) = await app.StudentAsync();
        Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync($"/api/games/attempts/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await other.PostAsJsonAsync($"/api/games/attempts/{id}/submit", new { roundNo = 5, answer = new { } })).StatusCode);

        var list = await student.GetJson("/api/games");
        var entry = list.EnumerateArray().SelectMany(c => c.GetProperty("games").EnumerateArray()).First(g => g.GetProperty("key").GetString() == game);
        Assert.Equal("finished", entry.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Games_cannot_be_started_outside_the_competition_window()
    {
        await app.DbAsync(db => db.CompetitionSettings.ExecuteUpdateAsync(u => u.SetProperty(c => c.IsActive, false)));
        var (student, _) = await app.StudentAsync();
        var response = await student.PostAsJsonAsync("/api/games/game24/start", new { });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("NO_COMPETITION", await response.Code());
    }

    [Fact]
    public async Task Concurrent_starts_create_exactly_one_attempt()
    {
        await app.LiveCompetitionAsync();
        var (student, studentId) = await app.StudentAsync();
        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => student.PostAsJsonAsync("/api/games/kenken/start", new { })));
        Assert.All(results, r => Assert.True(r.IsSuccessStatusCode));
        Assert.Equal(1, await app.DbAsync(db => db.GameAttempts.CountAsync(a => a.StudentId == studentId)));
    }

    // ------------------------------------------------------------------ Mega Quiz

    [Fact]
    public async Task Quiz_is_taken_scored_and_reported_without_ever_exposing_the_key()
    {
        var competition = await app.LiveCompetitionAsync(showResults: true);
        var admin = await app.AdminAsync("Quiz Manager");
        var quizId = await PublishedQuizAsync(admin);
        var (student, studentId) = await app.StudentAsync();

        var listed = await student.GetJson("/api/quiz");
        Assert.Contains(listed.EnumerateArray(), q => q.GetProperty("id").GetInt32() == quizId);

        var start = await student.PostAsync($"/api/quiz/{quizId}/start", JsonContent.Create(new { }));
        var raw = await start.Content.ReadAsStringAsync();
        var paper = await start.Json();
        Assert.DoesNotContain("correct", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("explanation", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Because it says so", raw);
        var attemptId = paper.GetProperty("id").GetGuid();
        var questions = paper.GetProperty("questions").EnumerateArray().ToList();
        Assert.Equal(4, questions.Count);

        int Option(JsonElement q, string text) => q.GetProperty("options").EnumerateArray().First(o => o.GetProperty("text").GetString() == text).GetProperty("id").GetInt32();
        // Q1 right, Q2 right, Q3 wrong, Q4 left blank; Q1 is first answered wrongly and then changed.
        await student.PutJson($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 1, optionId = Option(questions[0], "wrong one") });
        await student.PutJson($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 1, optionId = Option(questions[0], "RIGHT") });
        await student.PutJson($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 2, optionId = Option(questions[1], "RIGHT") });
        await student.PutJson($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 3, optionId = Option(questions[2], "wrong two") });
        // An option from a different question is not accepted.
        var foreign = await app.DbAsync(db => db.QuestionOptions.OrderBy(o => o.Id).Select(o => o.Id).FirstAsync());
        Assert.Equal(HttpStatusCode.BadRequest, (await student.PutAsJsonAsync($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 4, optionId = foreign - 100000 })).StatusCode);

        // A refresh mid-quiz returns the same paper with saved answers.
        var resumed = await student.PostJson($"/api/quiz/{quizId}/start");
        Assert.Equal(attemptId, resumed.GetProperty("id").GetGuid());
        Assert.Equal(Option(questions[0], "RIGHT"), resumed.GetProperty("questions")[0].GetProperty("selectedOptionId").GetInt32());

        var result = await student.PostJson($"/api/quiz/attempts/{attemptId}/submit");
        Assert.Equal(3.75m, result.GetProperty("score").GetDecimal());   // 2 + 2 - 0.25
        Assert.Equal(8m, result.GetProperty("maxScore").GetDecimal());
        Assert.Equal((2, 1, 1), (result.GetProperty("correct").GetInt32(), result.GetProperty("incorrect").GetInt32(), result.GetProperty("unanswered").GetInt32()));
        // While the competition is running the answer key stays hidden even though scores are shown.
        Assert.Equal(JsonValueKind.Null, result.GetProperty("review").ValueKind);

        // Submitting twice changes nothing, and answers can no longer be edited.
        Assert.Equal(3.75m, (await student.PostJson($"/api/quiz/attempts/{attemptId}/submit")).GetProperty("score").GetDecimal());
        Assert.Equal(HttpStatusCode.Conflict, (await student.PutAsJsonAsync($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 4, optionId = Option(questions[3], "RIGHT") })).StatusCode);

        // Editing the question afterwards (even swapping the right answer) does not change the recorded result.
        var q1Id = await app.DbAsync(db => db.QuizAttemptQuestions.Where(q => q.AttemptId == attemptId && q.OrderNo == 1).Select(q => q.QuestionId).FirstAsync());
        var bank = await app.DbAsync(db => db.Questions.Where(q => q.Id == q1Id).Select(q => q.CategoryId).FirstAsync());
        await admin.PutJson($"/api/admin/quiz/questions/{q1Id}", new
        {
            categoryId = bank, text = "Rewritten", marks = 5,
            options = new[] { new { text = "RIGHT", isCorrect = false }, new { text = "now this", isCorrect = true } }
        });
        var super = await app.AdminAsync();
        var detail = await super.GetJson($"/api/admin/quiz-attempts/{attemptId}");
        Assert.Equal(3.75m, detail.GetProperty("score").GetDecimal());
        Assert.StartsWith("Question", detail.GetProperty("questions")[0].GetProperty("text").GetString());
        Assert.True(detail.GetProperty("questions")[0].GetProperty("isCorrect").GetBoolean());

        // After the competition closes the student may review answers.
        await app.DbAsync(db => db.CompetitionSettings.Where(c => c.Id == competition).ExecuteUpdateAsync(u => u.SetProperty(c => c.EndDateTimeUtc, DateTime.UtcNow.AddSeconds(-1))));
        var review = await student.GetJson($"/api/quiz/attempts/{attemptId}/result");
        Assert.Equal(4, review.GetProperty("review").GetArrayLength());
    }

    [Fact]
    public async Task Results_stay_hidden_when_the_competition_does_not_publish_them()
    {
        await app.LiveCompetitionAsync(showResults: false);
        var quizId = await PublishedQuizAsync(await app.AdminAsync());
        var (student, _) = await app.StudentAsync();
        var attemptId = (await student.PostJson($"/api/quiz/{quizId}/start")).GetProperty("id").GetGuid();
        var result = await student.PostJson($"/api/quiz/attempts/{attemptId}/submit");
        Assert.False(result.GetProperty("resultsVisible").GetBoolean());
        Assert.Equal(JsonValueKind.Null, result.GetProperty("score").ValueKind);
        var history = await student.GetJson("/api/student/history");
        Assert.All(history.EnumerateArray(), h => Assert.Equal(JsonValueKind.Null, h.GetProperty("score").ValueKind));
        Assert.False((await student.GetJson("/api/student/leaderboard")).GetProperty("visible").GetBoolean());
    }

    [Fact]
    public async Task Unpublished_and_empty_quizzes_are_not_available_to_students()
    {
        await app.LiveCompetitionAsync();
        var admin = await app.AdminAsync();
        var quizId = await PublishedQuizAsync(admin);
        await admin.PostJson($"/api/admin/quiz/quizzes/{quizId}/unpublish");
        var (student, _) = await app.StudentAsync();
        Assert.Equal(HttpStatusCode.NotFound, (await student.PostAsJsonAsync($"/api/quiz/{quizId}/start", new { })).StatusCode);
        Assert.DoesNotContain((await student.GetJson("/api/quiz")).EnumerateArray(), q => q.GetProperty("id").GetInt32() == quizId);

        // A quiz without enough usable questions cannot be published at all.
        var seeded = await app.DbAsync(db => db.Quizzes.Where(q => q.Title == "HEM Mega Quiz 1").Select(q => q.Id).FirstAsync());
        var publish = await admin.PostAsJsonAsync($"/api/admin/quiz/quizzes/{seeded}/publish", new { });
        Assert.Equal(HttpStatusCode.Conflict, publish.StatusCode);
        Assert.Equal("QUIZ_NOT_READY", await publish.Code());
    }

    [Fact]
    public async Task Expired_quiz_is_closed_and_scored_by_the_maintenance_job()
    {
        await app.LiveCompetitionAsync();
        var quizId = await PublishedQuizAsync(await app.AdminAsync(), negative: 0);
        var (student, _) = await app.StudentAsync();
        var paper = await student.PostJson($"/api/quiz/{quizId}/start");
        var attemptId = paper.GetProperty("id").GetGuid();
        var right = paper.GetProperty("questions")[0].GetProperty("options").EnumerateArray().First(o => o.GetProperty("text").GetString() == "RIGHT").GetProperty("id").GetInt32();
        await student.PutJson($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 1, optionId = right });

        await app.DbAsync(db => db.QuizAttempts.Where(a => a.Id == attemptId).ExecuteUpdateAsync(u => u.SetProperty(a => a.ExpiresAtUtc, DateTime.UtcNow.AddMinutes(-1))));
        // Late answers are refused...
        Assert.Equal(HttpStatusCode.Conflict, (await student.PutAsJsonAsync($"/api/quiz/attempts/{attemptId}/answer", new { orderNo = 2, optionId = right })).StatusCode);
        using (var scope = app.Factory.Services.CreateScope())
            await MaintenanceService.RunAsync(scope.ServiceProvider, hourly: true);
        var stored = await app.DbAsync(db => db.QuizAttempts.FirstAsync(a => a.Id == attemptId));
        // ...and what was answered in time still counts.
        Assert.Equal((AttemptStatus.Expired, 2m), (stored.Status, stored.Score));
    }

    [Fact]
    public async Task Questions_can_be_imported_exported_and_previewed()
    {
        var admin = await app.AdminAsync("Quiz Manager");
        var csv = "category,question,option_a,option_b,option_c,option_d,correct,explanation,difficulty,marks\n" +
                  "Import Bank,\"Capital of India, officially?\",Mumbai,New Delhi,Kolkata,Chennai,B,It is New Delhi,Easy,2\n" +
                  "Import Bank,2 + 2 = ?,3,4,5,,2,,Medium,1\n" +
                  "Import Bank,Largest planet?,Mars,Venus,Jupiter,Saturn,Jupiter,,Hard,1\n";
        var imported = await (await admin.PostAsync("/api/admin/quiz/questions/import", Upload(System.Text.Encoding.UTF8.GetBytes(csv), "q.csv", "text/csv"))).Json();
        Assert.Equal(3, imported.GetProperty("imported").GetInt32());
        // Importing the same file again adds nothing.
        var again = await (await admin.PostAsync("/api/admin/quiz/questions/import", Upload(System.Text.Encoding.UTF8.GetBytes(csv), "q.csv", "text/csv"))).Json();
        Assert.Equal((0, 3), (again.GetProperty("imported").GetInt32(), again.GetProperty("skipped").GetInt32()));

        // A bad row rejects the whole file with a row-level message.
        var bad = await admin.PostAsync("/api/admin/quiz/questions/import", Upload("category,question,option_a,option_b,correct\nImport Bank,Fine?,yes,no,A\nImport Bank,Broken?,yes,no,Z\n"u8.ToArray(), "q.csv", "text/csv"));
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);
        Assert.Contains("Row 3", await bad.Content.ReadAsStringAsync());
        Assert.Equal(3, await app.DbAsync(db => db.Questions.CountAsync(q => q.Category.Name == "Import Bank")));

        var correct = await app.DbAsync(db => db.QuestionOptions.Where(o => o.IsCorrect && o.Question.Category.Name == "Import Bank").Select(o => o.Text).OrderBy(t => t).ToListAsync());
        Assert.Equal(["4", "Jupiter", "New Delhi"], correct);

        var export = await (await admin.GetAsync("/api/admin/quiz/questions/export")).Content.ReadAsStringAsync();
        Assert.Contains("\"Capital of India, officially?\"", export);

        var bank = await app.DbAsync(db => db.QuizCategories.Where(c => c.Name == "Import Bank").Select(c => c.Id).FirstAsync());
        var quiz = (await admin.PostJson("/api/admin/quiz/quizzes", new { title = "Imported quiz", categoryId = bank, selectionMode = "Random", questionCount = 2, durationMinutes = 5 })).GetProperty("id").GetInt32();
        var preview = await admin.GetJson($"/api/admin/quiz/quizzes/{quiz}/preview");
        Assert.Equal(2, preview.GetProperty("questions").GetArrayLength());
        Assert.All(preview.GetProperty("questions").EnumerateArray(), q => Assert.Single(q.GetProperty("options").EnumerateArray(), o => o.GetProperty("isCorrect").GetBoolean()));
        // Previewing does not create attempts.
        Assert.Equal(0, await app.DbAsync(db => db.QuizAttempts.CountAsync(a => a.QuizId == quiz)));

        // Validation: exactly one correct option.
        var invalid = await admin.PostAsJsonAsync("/api/admin/quiz/questions", new { categoryId = bank, text = "Two right?", options = new[] { new { text = "a", isCorrect = true }, new { text = "b", isCorrect = true } } });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    // ------------------------------------------------------------------ proctoring, bans, appeals

    [Fact]
    public async Task Violations_escalate_by_policy_and_a_suspended_student_can_appeal()
    {
        await app.LiveCompetitionAsync();
        var (student, studentId) = await app.StudentAsync();
        await ConsentAsync(student);
        var attemptId = (await student.PostJson("/api/games/game24/start")).GetProperty("id").GetGuid();

        async Task<JsonElement> Report(string rule)
        {
            // Step past the debounce window so each report is a separate incident.
            await app.DbAsync(db => db.ViolationEvents.Where(v => v.StudentId == studentId)
                .ExecuteUpdateAsync(u => u.SetProperty(v => v.OccurredAtUtc, v => v.OccurredAtUtc.AddSeconds(-10))));
            return await student.PostJson("/api/proctoring/events", new { ruleCode = rule, message = "test" });
        }

        // MULTIPLE_FACES default policy: flag at 1, terminate at 2, suspend at 3.
        var first = await Report("MULTIPLE_FACES");
        Assert.Equal("Flagged", first.GetProperty("action").GetString());
        Assert.True(first.GetProperty("captureImage").GetBoolean());
        Assert.True(await app.DbAsync(db => db.GameAttempts.Where(a => a.Id == attemptId).Select(a => a.IsFlagged).FirstAsync()));

        // A burst of duplicates inside the debounce window is one incident.
        var duplicate = await student.PostJson("/api/proctoring/events", new { ruleCode = "MULTIPLE_FACES" });
        Assert.False(duplicate.GetProperty("recorded").GetBoolean());

        // An unknown rule is ignored rather than trusted.
        Assert.False((await student.PostJson("/api/proctoring/events", new { ruleCode = "MADE_UP" })).GetProperty("recorded").GetBoolean());

        var second = await Report("MULTIPLE_FACES");
        Assert.Equal("Terminated", second.GetProperty("action").GetString());
        Assert.Equal(AttemptStatus.Terminated, await app.DbAsync(db => db.GameAttempts.Where(a => a.Id == attemptId).Select(a => a.Status).FirstAsync()));

        var third = await Report("MULTIPLE_FACES");
        Assert.Equal("Suspended", third.GetProperty("action").GetString());
        // Evidence for the event that caused the suspension can still be delivered although the session is over.
        (await SendEvidence(third, Jpeg, "s.jpg", "image/jpeg")).EnsureSuccessStatusCode();
        // The session is dead immediately.
        Assert.Equal(HttpStatusCode.Unauthorized, (await student.GetAsync("/api/student/dashboard")).StatusCode);

        var ban = await app.DbAsync(db => db.StudentBans.SingleAsync(b => b.StudentId == studentId));
        Assert.Equal((BanType.Suspension, BanSource.Automatic), (ban.Type, ban.Source)); // never an automatic permanent ban

        // Login now explains the restriction and offers an appeal.
        var login = await app.Anonymous().PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{studentId}", TestApp.StudentPassword));
        Assert.Equal(HttpStatusCode.Forbidden, login.StatusCode);
        var body = await login.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("BANNED", body.GetProperty("code").GetString());
        Assert.False(body.TryGetProperty("token", out _));

        var appealClient = app.Anonymous();
        appealClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.GetProperty("appealToken").GetString());
        // The appeal token opens the appeal endpoints and nothing else.
        Assert.Equal(HttpStatusCode.Forbidden, (await appealClient.GetAsync("/api/games")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await appealClient.GetAsync("/api/student/dashboard")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await appealClient.GetAsync("/api/admin/appeals")).StatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, (await appealClient.PostAsJsonAsync("/api/appeals", new { message = "too short" })).StatusCode);
        var appealId = (await appealClient.PostJson("/api/appeals", new { message = "My younger brother walked behind me twice. I was working alone." })).GetProperty("id").GetGuid();
        var duplicateAppeal = await appealClient.PostAsJsonAsync("/api/appeals", new { message = "Please look at this again, it was a mistake." });
        Assert.Equal("APPEAL_PENDING", await duplicateAppeal.Code());

        // Filing an appeal does not restore access.
        Assert.Equal(HttpStatusCode.Forbidden, (await app.Anonymous().PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{studentId}", TestApp.StudentPassword))).StatusCode);
        Assert.Equal("Pending", (await appealClient.GetJson("/api/appeals/mine")).GetProperty("appeals")[0].GetProperty("status").GetString());

        // Reviewer sees history and evidence links, adds a note, and must give a reason.
        var reviewer = await app.AdminAsync("Reviewer");
        var detail = await reviewer.GetJson($"/api/admin/appeals/{appealId}");
        Assert.Equal(3, detail.GetProperty("violations").GetArrayLength());
        Assert.Single(detail.GetProperty("gameAttempts").EnumerateArray());
        await reviewer.PostJson($"/api/admin/appeals/{appealId}/notes", new { note = "Checked the snapshots." });
        Assert.Equal(HttpStatusCode.BadRequest, (await reviewer.PostAsJsonAsync($"/api/admin/appeals/{appealId}/decision", new { decision = "reject", reason = "" })).StatusCode);

        var decision = await reviewer.PostJson($"/api/admin/appeals/{appealId}/decision", new { decision = "approve", reason = "Sibling in background, no assistance given." });
        Assert.Equal("Approved", decision.GetProperty("status").GetString());
        Assert.Equal(HttpStatusCode.Conflict, (await reviewer.PostAsJsonAsync($"/api/admin/appeals/{appealId}/decision", new { decision = "reject", reason = "Changed my mind." })).StatusCode);

        // Access is back, the terminated attempt can be reset for a retake, and everything is audited.
        var back = await app.LoginStudentAsync($"student{studentId}");
        var manager = await app.AdminAsync();
        await manager.PostJson($"/api/admin/game-attempts/{attemptId}/reset", new { reason = "Appeal upheld" });
        Assert.Equal("InProgress", (await back.PostJson("/api/games/game24/start")).GetProperty("status").GetString());
        var actions = await app.DbAsync(db => db.AuditLogs.Select(a => a.Action).ToListAsync());
        Assert.Contains("appeal.approve", actions);
        Assert.Contains("appeal.note_added", actions);
        Assert.Contains("game_attempt.reset", actions);
        Assert.Contains((await back.GetJson("/api/student/notifications")).EnumerateArray(), n => n.GetProperty("category").GetString() == "appeal");
    }

    [Fact]
    public async Task Rejected_appeal_keeps_the_ban_and_records_the_reason()
    {
        var admin = await app.AdminAsync();
        var studentId = await app.CreateStudentAsync();
        await admin.PostJson("/api/admin/bans", new { studentId, type = "Ban", reason = "Shared answers in a group chat." });

        var login = await (await app.Anonymous().PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{studentId}", TestApp.StudentPassword))).Content.ReadFromJsonAsync<JsonElement>();
        var appealClient = app.Anonymous();
        appealClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.GetProperty("appealToken").GetString());
        var appealId = (await appealClient.PostJson("/api/appeals", new { message = "I did not share anything with anyone at all." })).GetProperty("id").GetGuid();

        await admin.PostJson($"/api/admin/appeals/{appealId}/decision", new { decision = "reject", reason = "Screenshots show the messages." });
        var mine = await appealClient.GetJson("/api/appeals/mine");
        Assert.True(mine.GetProperty("restricted").GetBoolean());
        Assert.Equal("Screenshots show the messages.", mine.GetProperty("appeals")[0].GetProperty("decisionReason").GetString());
        Assert.Equal(HttpStatusCode.Forbidden, (await app.Anonymous().PostAsJsonAsync("/api/auth/login", app.LoginBody($"student{studentId}", TestApp.StudentPassword))).StatusCode);

        // A second appeal is allowed (limit 2), then converted to a dated suspension.
        var second = (await appealClient.PostJson("/api/appeals", new { message = "Please reconsider, it was my first competition." })).GetProperty("id").GetGuid();
        await admin.PostJson($"/api/admin/appeals/{second}/decision", new { decision = "suspend", reason = "Reduced on review.", suspendUntilUtc = DateTime.UtcNow.AddDays(2) });
        var ban = await app.DbAsync(db => db.StudentBans.SingleAsync(b => b.StudentId == studentId));
        Assert.Equal(BanType.Suspension, ban.Type);
        Assert.NotNull(ban.EndsAtUtc);
        // Limit reached.
        Assert.Equal("APPEAL_LIMIT", await (await appealClient.PostAsJsonAsync("/api/appeals", new { message = "One more time please, I really mean it." })).Code());
    }

    [Fact]
    public async Task Evidence_needs_consent_is_type_checked_and_is_only_visible_to_authorised_admins()
    {
        await app.LiveCompetitionAsync();
        var (student, studentId) = await app.StudentAsync();

        // Without consent to evidence capture nothing is requested and uploads are refused.
        await ConsentAsync(student, evidence: false);
        var noConsent = await student.PostJson("/api/proctoring/events", new { ruleCode = "NO_FACE" });
        Assert.False(noConsent.GetProperty("captureImage").GetBoolean());
        Assert.Equal(JsonValueKind.Null, noConsent.GetProperty("uploadToken").ValueKind);
        // The ordinary session token cannot upload evidence at all.
        Assert.Equal(HttpStatusCode.Forbidden, (await student.PostAsync($"/api/proctoring/events/{noConsent.GetProperty("eventId").GetGuid()}/evidence", Upload(Jpeg, "a.jpg", "image/jpeg"))).StatusCode);

        // With image consent but not video consent.
        await ConsentAsync(student, evidence: true, video: false);
        await app.DbAsync(db => db.ViolationEvents.Where(v => v.StudentId == studentId).ExecuteUpdateAsync(u => u.SetProperty(v => v.OccurredAtUtc, v => v.OccurredAtUtc.AddSeconds(-10))));
        var evt = await student.PostJson("/api/proctoring/events", new { ruleCode = "NO_FACE" });
        Assert.True(evt.GetProperty("captureImage").GetBoolean());
        Assert.False(evt.GetProperty("captureVideo").GetBoolean());
        var eventId = evt.GetProperty("eventId").GetGuid();

        var evidenceId = (await (await SendEvidence(evt, Jpeg, "a.jpg", "image/jpeg")).Json()).GetProperty("id").GetGuid();
        // An HTML file renamed to .jpg is rejected by content, and a real video is rejected without video consent.
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, (await SendEvidence(evt, "<html><script>alert(1)</script>"u8.ToArray(), "a.jpg", "image/jpeg")).StatusCode);
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, (await SendEvidence(evt, [0x1A, 0x45, 0xDF, 0xA3, 1, 2, 3, 4, 5, 6, 7, 8], "a.webm", "video/webm")).StatusCode);

        // The upload token is bound to its own event and opens nothing else.
        Assert.Equal(HttpStatusCode.Forbidden, (await SendEvidence(evt, Jpeg, "a.jpg", "image/jpeg", noConsent.GetProperty("eventId").GetGuid())).StatusCode);
        var tokenOnly = app.Anonymous();
        tokenOnly.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", evt.GetProperty("uploadToken").GetString());
        Assert.Equal(HttpStatusCode.Forbidden, (await tokenOnly.GetAsync("/api/games")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await tokenOnly.PostAsJsonAsync("/api/proctoring/events", new { ruleCode = "NO_FACE" })).StatusCode);

        var file = await app.DbAsync(db => db.EvidenceFiles.SingleAsync(e => e.Id == evidenceId));
        Assert.Equal(("image/jpeg", (long)Jpeg.Length), (file.ContentType, file.SizeBytes));
        Assert.Equal(64, file.Sha256.Length);
        Assert.True(file.RetainUntilUtc > DateTime.UtcNow.AddDays(80));
        Assert.True(File.Exists(Path.Combine(app.StorageRoot, file.StorageKey)));
        // Stored outside the web root: not reachable as a static file.
        Assert.NotEqual("image/jpeg", (await app.Anonymous().GetAsync("/" + file.StorageKey)).Content.Headers.ContentType?.MediaType);

        // Students and admins without the permission cannot read it; a reviewer can, and the view is audited.
        Assert.Equal(HttpStatusCode.Forbidden, (await student.GetAsync($"/api/admin/proctoring/evidence/{evidenceId}/content")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await (await app.AdminAsync("Quiz Manager")).GetAsync($"/api/admin/proctoring/evidence/{evidenceId}/content")).StatusCode);
        var reviewer = await app.AdminAsync("Reviewer");
        var content = await reviewer.GetAsync($"/api/admin/proctoring/evidence/{evidenceId}/content");
        Assert.Equal(Jpeg, await content.Content.ReadAsByteArrayAsync());
        Assert.True(await app.DbAsync(db => db.AuditLogs.AnyAsync(a => a.Action == "evidence.viewed" && a.EntityId == evidenceId.ToString())));
        // A reviewer may view but not delete.
        Assert.Equal(HttpStatusCode.Forbidden, (await reviewer.DeleteAsync($"/api/admin/proctoring/evidence/{evidenceId}")).StatusCode);

        // Retention: once past its date the file is removed and the row marked.
        await app.DbAsync(db => db.EvidenceFiles.Where(e => e.Id == evidenceId).ExecuteUpdateAsync(u => u.SetProperty(e => e.RetainUntilUtc, DateTime.UtcNow.AddDays(-1))));
        using (var scope = app.Factory.Services.CreateScope())
            await MaintenanceService.RunAsync(scope.ServiceProvider, hourly: true);
        Assert.False(File.Exists(Path.Combine(app.StorageRoot, file.StorageKey)));
        Assert.Equal(HttpStatusCode.NotFound, (await reviewer.GetAsync($"/api/admin/proctoring/evidence/{evidenceId}/content")).StatusCode);
    }

    [Fact]
    public async Task Dismissed_violations_do_not_count_and_minor_rules_never_suspend_by_default()
    {
        await app.LiveCompetitionAsync();
        var (student, studentId) = await app.StudentAsync();
        await ConsentAsync(student);
        var reviewer = await app.AdminAsync("Reviewer");

        // A lost connection is recorded but has no consequences however often it happens.
        for (var i = 0; i < 6; i++)
        {
            await app.DbAsync(db => db.ViolationEvents.Where(v => v.StudentId == studentId).ExecuteUpdateAsync(u => u.SetProperty(v => v.OccurredAtUtc, v => v.OccurredAtUtc.AddSeconds(-10))));
            Assert.Equal("Warning", (await student.PostJson("/api/proctoring/events", new { ruleCode = "NETWORK_LOST" })).GetProperty("action").GetString());
        }
        (await student.GetAsync("/api/student/dashboard")).EnsureSuccessStatusCode();

        var first = await student.PostJson("/api/proctoring/events", new { ruleCode = "NO_FACE" });
        Assert.Equal(1, first.GetProperty("occurrence").GetInt32());
        await reviewer.PostJson($"/api/admin/proctoring/violations/{first.GetProperty("eventId").GetGuid()}/review", new { status = "Dismissed", note = "Camera glare." });
        await app.DbAsync(db => db.ViolationEvents.Where(v => v.StudentId == studentId).ExecuteUpdateAsync(u => u.SetProperty(v => v.OccurredAtUtc, v => v.OccurredAtUtc.AddSeconds(-10))));
        Assert.Equal(1, (await student.PostJson("/api/proctoring/events", new { ruleCode = "NO_FACE" })).GetProperty("occurrence").GetInt32());
    }

    // ------------------------------------------------------------------ admin: results, students, settings

    [Fact]
    public async Task Leaderboard_reports_and_student_management_work_together()
    {
        var competition = await app.LiveCompetitionAsync(showResults: true, showLeaderboard: true);
        var admin = await app.AdminAsync();

        // A leaderboard with a single participant still shows their total.
        var (solo, _) = await app.StudentAsync();
        var soloAttempt = (await solo.PostJson("/api/games/word/start")).GetProperty("id").GetGuid();
        await solo.PostJson($"/api/games/attempts/{soloAttempt}/submit", new { roundNo = 1, answer = await CorrectAnswerAsync(soloAttempt, 1) });
        await solo.PostJson($"/api/games/attempts/{soloAttempt}/quit");
        Assert.Equal(10m, (await solo.GetJson("/api/student/leaderboard")).GetProperty("me").GetProperty("totalScore").GetDecimal());

        // Import two students, play one game each with different outcomes.
        var csv = "username,password,first_name,last_name,city\nimp_alpha,Passw0rd!one,Alpha,One,Pune\nimp_beta,Passw0rd!two,Beta,Two,Surat\n";
        var imported = await (await admin.PostAsync("/api/admin/students/import", Upload(System.Text.Encoding.UTF8.GetBytes(csv), "s.csv", "text/csv"))).Json();
        Assert.Equal(2, imported.GetProperty("imported").GetInt32());

        var alpha = await app.LoginStudentAsync("imp_alpha", "Passw0rd!one");
        var beta = await app.LoginStudentAsync("imp_beta", "Passw0rd!two");
        var a = (await alpha.PostJson("/api/games/word/start")).GetProperty("id").GetGuid();
        for (var r = 1; r <= 5; r++) await alpha.PostJson($"/api/games/attempts/{a}/submit", new { roundNo = r, answer = await CorrectAnswerAsync(a, r) });
        var b = (await beta.PostJson("/api/games/word/start")).GetProperty("id").GetGuid();
        await beta.PostJson($"/api/games/attempts/{b}/submit", new { roundNo = 1, answer = await CorrectAnswerAsync(b, 1) });
        await beta.PostJson($"/api/games/attempts/{b}/quit");

        var board = await admin.GetJson($"/api/admin/results/leaderboard?competitionId={competition}&pageSize=200");
        var rows = board.GetProperty("items").EnumerateArray().ToList();
        var alphaRow = rows.First(r => r.GetProperty("name").GetString() == "Alpha One");
        var betaRow = rows.First(r => r.GetProperty("name").GetString() == "Beta Two");
        Assert.Equal(50m, alphaRow.GetProperty("totalScore").GetDecimal());
        Assert.Equal(10m, betaRow.GetProperty("totalScore").GetDecimal());
        Assert.True(alphaRow.GetProperty("rank").GetInt32() < betaRow.GetProperty("rank").GetInt32());

        var mine = await alpha.GetJson("/api/student/leaderboard");
        Assert.True(mine.GetProperty("visible").GetBoolean());
        Assert.Equal(50m, mine.GetProperty("me").GetProperty("totalScore").GetDecimal());
        var history = await alpha.GetJson("/api/student/history");
        Assert.Equal(50m, history[0].GetProperty("score").GetDecimal());

        foreach (var report in new[] { "leaderboard", "game-attempts", "quiz-attempts", "violations", "participation" })
        {
            var response = await admin.GetAsync($"/api/admin/reports/{report}?competitionId={competition}");
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType!.MediaType);
        }
        Assert.Contains("Alpha One", await (await admin.GetAsync($"/api/admin/reports/leaderboard?competitionId={competition}")).Content.ReadAsStringAsync());
        (await admin.GetAsync($"/api/admin/results/summary?competitionId={competition}")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/dashboard")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/proctoring/live")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/game-attempts?search=imp_alpha")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/quiz-attempts")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/bans?state=all")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/games")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/games/words?search=tiger")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/competitions")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin/roles")).EnsureSuccessStatusCode();
        (await admin.GetAsync("/api/admin-sessions")).EnsureSuccessStatusCode();

        // The admin attempt view (and only it) contains the solutions.
        var detail = await admin.GetJson($"/api/admin/game-attempts/{a}");
        Assert.Equal(5, detail.GetProperty("rounds").GetArrayLength());
        Assert.True(detail.GetProperty("rounds")[0].GetProperty("solution").TryGetProperty("answer", out _));

        // Deactivating a student ends their session at once; resetting a password signs them out too.
        var alphaId = await app.DbAsync(db => db.AuthUsers.Where(u => u.Username == "imp_alpha").Select(u => u.UserId).FirstAsync());
        await admin.PutJson($"/api/admin/students/{alphaId}", new { username = "imp_alpha", firstName = "Alpha", lastName = "One", isActive = false });
        Assert.Equal(HttpStatusCode.Unauthorized, (await alpha.GetAsync("/api/student/dashboard")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await app.Anonymous().PostAsJsonAsync("/api/auth/login", app.LoginBody("imp_alpha", "Passw0rd!one"))).StatusCode);

        var student = await admin.GetJson($"/api/admin/students/{alphaId}");
        Assert.Single(student.GetProperty("gameAttempts").EnumerateArray());
        (await admin.GetAsync("/api/admin/students?search=imp_&status=inactive")).EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Settings_rules_and_game_configuration_take_effect()
    {
        await app.LiveCompetitionAsync();
        var admin = await app.AdminAsync();

        // Game configuration: 2 rounds of 4x4 KenKen worth 7 points each.
        var games = await admin.GetJson("/api/admin/games");
        var kenken = games.EnumerateArray().SelectMany(c => c.GetProperty("games").EnumerateArray()).First(g => g.GetProperty("key").GetString() == "kenken");
        var gameId = kenken.GetProperty("id").GetInt32();
        await admin.PutJson($"/api/admin/games/{gameId}", new { name = "KenKen", difficulty = "Hard", rounds = 2, pointsPerRound = 7, isActive = true, config = new { size = 4 } });
        var sample = await admin.PostJson($"/api/admin/games/{gameId}/sample");
        Assert.Equal(4, sample.GetProperty("puzzle").GetProperty("size").GetInt32());

        var (student, studentId) = await app.StudentAsync();
        var attempt = await student.PostJson("/api/games/kenken/start");
        Assert.Equal(2, attempt.GetProperty("totalRounds").GetInt32());
        Assert.Equal(4, attempt.GetProperty("round").GetProperty("puzzle").GetProperty("size").GetInt32());
        Assert.Equal(14m, await app.DbAsync(db => db.GameAttempts.Where(a => a.StudentId == studentId).Select(a => a.MaxScore).FirstAsync()));
        await admin.PutJson($"/api/admin/games/{gameId}", new { name = "KenKen", difficulty = "Medium", rounds = 5, pointsPerRound = 10, isActive = true, config = new { } });

        // Tightening a rule changes what the next report does.
        var rules = await admin.GetJson("/api/admin/proctoring/rules");
        var rule = rules.EnumerateArray().First(r => r.GetProperty("code").GetString() == "COPY_PASTE");
        await admin.PutJson($"/api/admin/proctoring/rules/{rule.GetProperty("id").GetInt32()}", new { name = "Copy or paste", severity = "Serious", isEnabled = true, flagAfter = 1, terminateAfter = 1, suspendAfter = 0 });
        await ConsentAsync(student);
        Assert.Equal("Terminated", (await student.PostJson("/api/proctoring/events", new { ruleCode = "COPY_PASTE" })).GetProperty("action").GetString());
        await admin.PutJson($"/api/admin/proctoring/rules/{rule.GetProperty("id").GetInt32()}", new { name = "Copy, cut or paste attempt", severity = "Minor", isEnabled = true, flagAfter = 5, terminateAfter = 0, suspendAfter = 0 });

        // Settings are validated and applied; changing the consent version asks everyone to accept again.
        var settings = await admin.GetJson("/api/admin/settings");
        Assert.True((await student.GetJson("/api/proctoring/status")).GetProperty("termsAccepted").GetBoolean());
        var node = JsonNode.Parse(settings.GetRawText())!;
        node["consentVersion"] = "9.9";
        node["evidenceRetentionDays"] = 99999;
        var saved = await admin.PutJson("/api/admin/settings", node);
        Assert.Equal(730, saved.GetProperty("evidenceRetentionDays").GetInt32());
        Assert.False((await student.GetJson("/api/proctoring/status")).GetProperty("termsAccepted").GetBoolean());
        node["consentVersion"] = "2.0"; node["evidenceRetentionDays"] = 90;
        await admin.PutJson("/api/admin/settings", node);

        // Custom role with a single permission.
        var roleId = (await admin.PostJson("/api/admin/roles", new { name = "Word Editor", permissions = new[] { "games.view", "games.manage", "not.a.permission" } })).GetProperty("id").GetInt32();
        var role = (await admin.GetJson("/api/admin/roles")).EnumerateArray().First(r => r.GetProperty("id").GetInt32() == roleId);
        Assert.Equal(2, role.GetProperty("permissions").GetArrayLength());
        Assert.Equal(HttpStatusCode.Conflict, (await admin.DeleteAsync($"/api/admin/roles/{(await admin.GetJson("/api/admin/roles"))[0].GetProperty("id").GetInt32()}")).StatusCode);

        // Word bank maintenance.
        var wordId = (await admin.PostJson("/api/admin/games/words", new { word = "zenith", clue = "The highest point", topic = "Test" })).GetProperty("id").GetInt32();
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync("/api/admin/games/words", new { word = "ZENITH" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.PostAsJsonAsync("/api/admin/games/words", new { word = "two words" })).StatusCode);
        (await admin.DeleteAsync($"/api/admin/games/words/{wordId}")).EnsureSuccessStatusCode();

        // Competition scheduling validation.
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.PostAsJsonAsync("/api/admin/competitions", new { competitionName = "Backwards", startDateTimeUtc = DateTime.UtcNow, endDateTimeUtc = DateTime.UtcNow.AddHours(-1) })).StatusCode);
    }
}
