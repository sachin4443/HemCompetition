# HEM Competition – Phase 1 Audit Report

Scope: every tracked file in the repository at commit `4eb9491`, the `Competition.pptx`
walkthrough, and the pgAdmin screenshot of the `HemCompetition` PostgreSQL database.
Everything below was verified against source code. Where something could **not** be
verified (because it lives only inside the MSSQL server), it is called out explicitly.

## 1. Existing architecture

| Layer | What is actually there |
|---|---|
| Backend | One ASP.NET Core project, `Backend/HemCompetition.API`, `net10.0`. Controllers talk to the database directly; only Futoshiki, KenKen and sessions have a service class. |
| Data access | Three different styles side by side: EF Core + Npgsql (`AuthDbContext`), EF Core + SQL Server (`Hemvirtues2627Context`, 363 scaffolded `DbSet`s), and Dapper + `SqlConnection` calling stored procedures. |
| Frontend | React 19 + Vite 8 + Bootstrap 5 + axios + sweetalert2 + MediaPipe face detector. Built into `Backend/…/wwwroot` and served by the API (SPA fallback). |
| Auth | JWT (HMAC-SHA256, 120 min), ASP.NET Identity `PasswordHasher` (PBKDF2). Students also have a server-side session row that is validated on every request. |
| Tests | None. |
| Migrations | None. All PostgreSQL tables were created by hand. |

### Databases

**PostgreSQL (`AuthConnection`)** – 7 tables in the screenshot:

| Table | Mapped in code | Purpose |
|---|---|---|
| `auth_users` | yes | Student accounts (≈16k rows), copied from MSSQL `Users`/`AspNetUsers`; `user_id` is the MSSQL user id |
| `auth_sessions` | yes | One active session per student, heartbeat timestamps |
| `competition_security` | yes | One row per student: consent, status `ACTIVE`/`DISQUALIFIED`, last violation |
| `competition_settings` | yes | Competition name and start/end window |
| `admin_users` | yes | Admin accounts |
| `ExceptionLogs` | yes | Unhandled exception log |
| `auth_users_import` | **no** | Staging table from the original user import; unused by the application |

**SQL Server (`DefaultConnection`)** – the whole legacy "hemvirtues" LMS database. The
scaffolded context exposes 363 tables/views; the competition uses about 20 of them, and
most of the logic through **19 stored procedures whose source is not in the repository**:

| Used by | Stored procedures / tables |
|---|---|
| Dashboards | `HF_SP_GetModules_Play_Status`, `HF_SP_GetNumberGame_Play_Status`, `HF_SP_GetWordGame_Play_Status`, `HF_SP_GetQuiz_Play_Status` |
| Mega Quiz | `HF_SP_CreateAndGetExamQuestions`, `sp_GetExamReviewDetails`, tables `Quiz`, `Examination`, `ExaminationRecords`, `QuestionOption` |
| Game 24 | `Sp_PlayGame_Logic`, `SP_InesetNextGames`, `SP_SkipGames` |
| Futoshiki | `Futoshiki_Competition_Master/Target/Solution`, `Futoshiki_User_Answer` (EF) |
| KenKen | `Ken_Comp_Master/Cage/Target/Solution/UserAnswer` (EF) |
| Puzzle | `puzzleSolvedCount`, `HF_SP_SavePuzzleTransaction`, `Puzzle_Comp_Master` |
| Word Challenge | `WordGameSolvedCount`, `HF_WordGamedata_Comp`, `HF_SP_SaveWordGameSolution` |
| Scramble | `ScrambleSolvedCount`, `SP_ScrambleWord_Comp`, `HF_SP_SaveScrambleSolution` |
| Reports | `HF_SP_COMP_QuizReports`, `HF_SP_Comp_UserGameSummaryForNumberGame`, `HF_SP_Comp_UserGameSummaryForWordGame`, `HF_SP_COMP_GameSummaryReports`, `HF_SP_Comp_DetailReport`, `HF_SP_Comp_CountReport` |

> **Not verifiable from the repository:** question selection rules, scoring, and report
> column definitions implemented inside those procedures, and the content itself (quiz
> questions, word lists, pre-built puzzles). The new implementation re-creates the
> behaviour visible from the API and UI; content must be imported (see migration plan).

## 2. Current features (as implemented, not as documented)

Student flow: login → "Quick Verification" camera check → accept terms → dashboard with
three sections → section dashboard → game. Matches the PPT, with these corrections:

* The third number game is **Futoshiki** (the image is named `phosukhy.png`; "Photosoky" in the brief).
* Five rounds per game (the `>= 5` checks in the word-game endpoints and `Take(5)` in number games).
* "Skip" ends the game permanently; "Next" moves on without return (PPT slide 6).
* `ResultPage` and `review-quiz` exist but students never see a score for games.
* Dashboard "games" list returned by `/dashboard/user-profile` is hard-coded (Quiz is always "Completed").

Admin: login, dashboard with user/session counts, session browser with CSV export, live
competition summary, CSV report downloads, and developer telemetry/diagnostics pages.
There is **no** question management, student management, violation review, or ban
handling anywhere in the admin UI.

## 3. Existing security mechanisms

Worth keeping (and kept):

* PBKDF2 password hashes (ASP.NET Identity v3 format) – existing students can keep their passwords.
* Single active session per student, heartbeat every 5 s, 150 s timeout, session validated per request.
* "Already logged in elsewhere" prompt at login.
* Duplicate-tab lock (`BroadcastChannel` + `localStorage` ownership heartbeat).
* Fullscreen enforcement, tab-visibility, blur, and blocked-key detection with a 10-warning limit.
* Camera gate before the dashboard; MediaPipe BlazeFace on-device detection; no-face / multiple-face
  countdown (10 s) then disqualification; consent version stored.
* Disqualified accounts are refused at login.
* Competition window check before login.

## 4. Bugs, vulnerabilities and technical issues

### Critical

| # | Issue | Where |
|---|---|---|
| C1 | **No authentication on any game, quiz or report endpoint.** `CompetitionController`, `Game24ApiController`, `FutoshikiApiController`, `KenkenApiController` and `ReportsController` have no `[Authorize]`. Anyone on the internet can download every report CSV (names, emails, scores). | controllers |
| C2 | **User id taken from URL/body.** `GetQuestions/{userId}`, `SaveAnswer {UserId}`, `status/{userId}` let any caller read or write another student's attempts. | number-game controllers, `CompetitionController` |
| C3 | **Answers shipped to the browser.** Scramble returns `answer` and `UnscrambleWord`; Word Challenge returns `WG_Answer`; Puzzle returns the word list and is validated only in JS; Game 24 is validated only in JS (`new Function`) and the server stores whatever it is sent. | `CompetitionController`, game components |
| C4 | **Quiz scoring is forgeable.** `submit-answer` and `get-exam-result` accept any `ExamID` with no ownership check; `get-exam-result` rewrites `MarksObtain` each time it is called; `review-quiz` returns any exam by id. | `CompetitionController` |
| C5 | **Anyone can log any student out.** `POST /api/auth/terminate-session {userId}` is anonymous and unconditional. | `AuthController` |
| C6 | **Any student can change the competition window.** `CompetitionSetting/save` is `[Authorize]` only – a student JWT passes. | `CompetitionSettingController` |
| C7 | **Secrets committed.** JWT signing key, SQL Server `sa` password, PostgreSQL password in `appsettings.json`; three admin accounts with the same hard-coded password re-seeded on every start. | `appsettings.json`, `AdminUserSeeder` |
| C8 | **Self-reported, instant, permanent disqualification.** The browser decides and calls `/disqualify`; the server trusts it, keeps no event history and no evidence, and there is no way back except editing the database. | `CompetitionSecurityController`, `CameraSecurityProvider` |

### High

* No login rate limiting or lockout (`access_failed_count` is incremented but never acted on; `lockout_end_date_utc` unused).
* Login reveals whether a username exists (404 "not eligible" vs 401).
* Admin JWT has no server-side revocation and bypasses session validation; admin pages have no route guard.
* `GlobalExceptionMiddleware` sits after session validation and endpoints return `ex.Message` to clients.
* Unauthenticated debug endpoints: `auth/auth-db-test` (returns a real user's profile), `competition/test-exception`.
* MediaPipe WASM is loaded from a public CDN at a different version (0.10.21) than the installed package (0.10.35).
* No CORS/security headers, no HTTPS-only/HSTS handling, tokens in `localStorage`.
* 10-warning termination only clears `localStorage` – the student logs straight back in and nothing is recorded.

### Medium / correctness

* Session validation writes to the database on every request (`LastActivityUtc`), plus a 5 s heartbeat per student – at 16k students that is thousands of writes per second for no benefit.
* Times are stored as JavaScript date strings; durations come from the client clock.
* KenKen loads cages and targets in a per-puzzle loop (N+1); cache keys are computed and never used.
* `Take(5)` number-game selection breaks as soon as a student has answered some but not all puzzles of a batch.
* Futoshiki grid parsing reads `Target6` for two different cells.
* Race: double-click submit can insert two answers (no unique constraint, no idempotency).

## 5. Unnecessary code

* `Models/` – 357 scaffolded SQL Server entities + 9.4k-line context; ~340 are unrelated LMS tables.
* Half of `DashboardController.cs` is a commented-out old version.
* Duplicates: `components/pdf/PdfViewerModal` vs `components/dashboard/PdfViewerModal`; `security/ExamSecurityProvider` vs `components/security/ExamSecurityProvider` (neither used); `components/dashboard/QuizDashboard` vs `pages/QuizDashboard`; `QuizGameComponent` (8-line stub); `authService.js`, `FullscreenManager.js` (unused).
* `wwwroot/` build output is committed (stale hashed bundles).
* `Puzzle.css`/`Puzzle1.css`, jQuery-style `wordfind*.js` loaded globally.
* Packages no longer needed after consolidation: `Microsoft.EntityFrameworkCore.SqlServer`, `Dapper`.
* Service classes live in `DTOs/` (`ExamBAL`, `ApiDiagnosticsService`, `SqlDiagnosticsService`).

## 6. Recommended architecture (summary – detail in `02-ARCHITECTURE.md`)

* One `AppDbContext` on PostgreSQL with EF Core migrations; SQL Server provider, Dapper and all scaffolded models removed.
* One game engine contract (`IGameEngine`: generate → public puzzle + server-held solution → validate) with six engines; one attempt pipeline, one set of tables, one set of endpoints for all six games.
* Mega Quiz as a first-class module: question bank, quizzes, versioned snapshots per attempt, server-side scoring.
* Proctoring as a server-side policy engine: the browser reports events and uploads evidence; the server counts, decides, and records.
* Bans as records with an appeal workflow rather than a status flag.
* Role-based admin API (permission checks on every endpoint) with an audit log.
* Every student endpoint derives the user from the token; no user ids in URLs.

## 7. Migration plan

1. **Baseline.** First EF migration describes the six existing tables. On a database where they already exist the initializer records it as applied instead of running it, so nothing is dropped or recreated and all student accounts and password hashes survive.
2. **Additive migration.** New tables and new nullable columns only. `auth_users_import` is left untouched.
3. **Data carry-over inside PostgreSQL.** Existing `DISQUALIFIED` rows become ban records so those students see the appeal option.
4. **Content from SQL Server (one-off, needs your database).** Quiz questions/options and word lists are exported to CSV and loaded through the admin import screens. `docs/04-MSSQL-EXPORT.md` contains the export queries.
5. **Historical results.** Past attempts in SQL Server are not copied automatically; the reports can be archived as CSV from the old system before it is switched off.
6. **Cut-over.** Back up PostgreSQL (`pg_dump`), deploy, let migrations run, import content, remove the SQL Server connection string.
