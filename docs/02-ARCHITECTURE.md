# Architecture

One ASP.NET Core application, one PostgreSQL database, one React single-page app (built into
`Backend/HemCompetition.API/wwwroot` and served by the API).

```
Browser (React 19, Vite, Bootstrap 5)
  student app            admin app (separate lazy-loaded bundle)
        |  /api  (JSON, Bearer access token in memory + HttpOnly refresh cookie)
ASP.NET Core 10
  Controllers ............ thin: validate input, call a service, shape the response
  Services ............... GameAttemptService, QuizAttemptService, ProctoringService, BanService,
                           LeaderboardService, SessionService, TokenService, SettingsService, AuditService
  Games .................. IGameEngine + six engines (pure logic, no database)
  Security ............... permission catalogue, RequirePermission filter, token types
  Infrastructure ......... file storage, CSV/XLSX reader, options, error type
  Data ................... AppDbContext, migrations, DatabaseInitializer, starter word list
PostgreSQL (EF Core + Npgsql)        File storage (App_Data/storage, outside wwwroot)
```

## Game engine

Every generated game implements one interface:

```csharp
public interface IGameEngine
{
    string Key { get; }
    bool RequiresWords { get; }
    GeneratedRound Generate(GenerationContext context);          // -> (Puzzle, Solution)
    ValidationResult Validate(JsonObject puzzle, JsonObject solution, JsonElement submission);
}
```

| Concept in the brief | Where it lives |
|---|---|
| Game Generator / Validator | `IGameEngine.Generate` / `Validate` (`Games/*.cs`) |
| Game Configuration | `games` row (difficulty, rounds, points, time limit, JSON options) |
| Game Session / Attempt | `game_attempts` + `game_attempt_rounds` (one pipeline: `GameAttemptService`) |
| Score Calculator | `ScoreCalculator` |
| Result Processor / Leaderboard | `LeaderboardService` (computed from attempts, never stored separately) |

`Puzzle` is what the student sees; `Solution` stays on the server. All rounds of an attempt are
generated when it starts and stored as JSONB snapshots, so a refresh resumes the same round and the
admin can later see exactly what was asked, what was right, and what was submitted.

**Adding a game:** write one class implementing `IGameEngine`, register it in `Program.cs`, insert
one row in `games`, and add a board component to `Frontend/src/games/boards.jsx`. Attempts, scoring,
history, leaderboard, admin screens and reports need no change.

| Game | Engine | Generation | Validation |
|---|---|---|---|
| Game 24 | `game24` | random numbers that a solver proves can make 24 | expression parsed and evaluated with exact fractions; each number used once (never `eval`) |
| KenKen | `kenken` | random Latin square cut into arithmetic cages, unique solution preferred | by the rules (rows, columns, every cage) |
| Futoshiki | `futoshiki` | random Latin square; clues added until exactly one solution remains | by the rules (givens, rows, columns, every sign) |
| Scramble | `scramble` | words drawn from the word bank; highlighted letters spell the final word | each row and the final word, accepting valid anagrams from the bank |
| Puzzle | `puzzle` | word-search grid built server-side | each word must lie on a straight line in the grid |
| Word Challenge | `word` | clue + length from the word bank | exact word |

## Mega Quiz

* `quiz_categories` are question banks; `questions` + `question_options` hold the content (exactly
  one correct option, enforced by a partial unique index).
* A `quizzes` row defines rules: bank, fixed or random selection, question count, duration, marks,
  negative marks, shuffling, published flag, version.
* Starting a quiz writes a `quiz_attempt_questions` row per question containing a **snapshot**
  (text, options, marks, answer key). Scoring reads only snapshots, so later edits to the bank never
  change a recorded result. The quiz version is stored on the attempt.
* Student responses never contain the key, explanations or correctness. Review (with answers) is
  released only when the competition allows results **and** has ended.
* The timer is enforced by the server (`expires_at_utc`); a background job closes and scores expired attempts.

## Authentication and sessions

* Passwords: ASP.NET Identity PBKDF2 (existing hashes keep working).
* Access token: JWT, 15 minutes, kept in JavaScript memory only. Refresh token: random 48 bytes in an
  `HttpOnly; SameSite=Strict` cookie scoped to the refresh path, stored hashed, rotated on every use;
  re-use of a rotated token revokes the whole family.
* Token types: `student`, `admin`, `appeal` (restricted student, appeal endpoints only), `evidence`
  (upload for one violation event only). Each endpoint accepts exactly one kind.
* Students: one live session; every request is checked against it; heartbeat every 30 s.
* Lockout after repeated failures, rate limits per address and per user, uniform login error.
* Student sign-in requires a captcha: a five-digit code drawn server-side as a dot-matrix SVG, valid
  for three minutes and for one attempt. It can be switched off under Admin → Settings.
* Admins: role → permissions in the database, checked on every request by `[RequirePermission]`.

## Proctoring

The browser observes and reports; the server decides.

1. Student reads the rules and gives explicit consent (rules; photo evidence; optional video).
2. Camera starts only after the student presses the button. Face detection (MediaPipe BlazeFace)
   runs on the device; no video is streamed. It counts faces only — no identification.
3. An observation (tab switch, fullscreen exit, no face, several faces, copy/paste, …) is POSTed to
   `/api/proctoring/events`. `ProctoringService` looks up the rule, counts prior occurrences that
   were not dismissed, and returns the action: warning → flag attempt → end attempt → suspend.
4. If the rule asks for evidence and the student consented, the response includes an upload token;
   the browser captures a snapshot (and a short clip) and uploads it. Files are type-checked by
   content, hashed, stored outside the web root, and deleted after the retention period.
5. Automatic action never exceeds a suspension. A ban is always an administrator's decision.
6. A restricted student sees the reason at sign-in and can request a review; reviewers see the
   history, evidence and attempts, add internal notes and decide. Everything is audit-logged.

## Request flow example — submitting a game answer

`POST /api/games/attempts/{id}/submit` → student token and live session verified → attempt row
locked (`SELECT … FOR UPDATE`) → ownership and "is this the open round" checked → engine validates
against the stored solution → round and score updated in the same transaction → response contains
correct/incorrect and the next puzzle, never a solution.
