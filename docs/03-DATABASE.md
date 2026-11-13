# Database

PostgreSQL only. Schema is owned by EF Core migrations in `Backend/HemCompetition.API/Data/Migrations`.

## Tables

**Kept from the original database (data preserved)**

| Table | Notes |
|---|---|
| `auth_users` | Students. `user_id` (bigint, unique) is the id every other table references. |
| `auth_sessions` | One live session per student. |
| `competition_security` | Per-student consent record (version, accepted at). Old `DISQUALIFIED` rows were copied into `student_bans`. |
| `competition_settings` | Competitions. New columns: `description`, `show_results_to_students`, `show_leaderboard`, `proctoring_required`. |
| `admin_users` | New columns: `email`, `role_id`, `lockout_end_utc`, `must_change_password`. |
| `ExceptionLogs` | Unhandled exception log. |
| `auth_users_import` | Not used by the application; left untouched. |

**New**

| Area | Tables |
|---|---|
| Access | `admin_roles`, `admin_role_permissions`, `refresh_tokens` |
| Catalogue | `game_categories`, `games`, `word_bank` |
| Game play | `game_attempts`, `game_attempt_rounds` |
| Quiz content | `quiz_categories`, `questions`, `question_options`, `quizzes`, `quiz_questions` |
| Quiz play | `quiz_attempts`, `quiz_attempt_questions` |
| Proctoring | `proctoring_sessions`, `violation_rules`, `violation_events`, `evidence_files` |
| Restrictions | `student_bans`, `appeals`, `appeal_notes` |
| Platform | `notifications`, `notification_reads`, `audit_logs`, `system_settings` |

Suggested entities that were deliberately **not** created, and why:

| Suggested | Decision |
|---|---|
| StudentProfiles | Profile columns already live on `auth_users`; a second table would only add a join. |
| Permissions / UserRoles | Permissions are a fixed catalogue in code; an admin has one role (`admin_users.role_id`). |
| QuizAnswers, GameAttemptDetails | Answers live on `quiz_attempt_questions` / `game_attempt_rounds` (one row per question/round). |
| GameResults, StudentScores, Leaderboards | Derived from attempts on demand; storing them would allow drift. |
| CompetitionGames | Games and quizzes are enabled globally (active / published flags). |
| QuestionMedia | One optional image per question: `questions.image_key` + content type. |

## ER diagram

```mermaid
erDiagram
    auth_users ||--o{ auth_sessions : has
    auth_users ||--o{ game_attempts : plays
    auth_users ||--o{ quiz_attempts : takes
    auth_users ||--o{ proctoring_sessions : monitored_in
    auth_users ||--o{ violation_events : causes
    auth_users ||--o{ student_bans : restricted_by
    auth_users ||--o{ refresh_tokens : holds
    auth_users ||--o{ notifications : receives

    competition_settings ||--o{ game_attempts : scopes
    competition_settings ||--o{ quiz_attempts : scopes

    game_categories ||--o{ games : contains
    games ||--o{ game_attempts : attempted_as
    game_attempts ||--|{ game_attempt_rounds : has

    quiz_categories ||--o{ questions : holds
    questions ||--|{ question_options : has
    quiz_categories ||--o{ quizzes : default_bank
    quizzes ||--o{ quiz_questions : pins
    questions ||--o{ quiz_questions : pinned_in
    quizzes ||--o{ quiz_attempts : attempted_as
    quiz_attempts ||--|{ quiz_attempt_questions : has

    proctoring_sessions ||--o{ violation_events : records
    violation_rules ||--o{ violation_events : classifies
    violation_events ||--o{ evidence_files : has
    game_attempts ||--o{ violation_events : during
    quiz_attempts ||--o{ violation_events : during

    violation_events ||--o{ student_bans : triggers
    student_bans ||--o{ appeals : appealed_by
    appeals ||--o{ appeal_notes : has

    admin_roles ||--o{ admin_users : assigned
    admin_roles ||--o{ admin_role_permissions : grants
    admin_users ||--o{ refresh_tokens : holds
```

## Integrity rules worth knowing

| Rule | Mechanism |
|---|---|
| One attempt per student, game/quiz and competition | unique index on `(competition_id, student_id, game_id)` / `(…, quiz_id)` |
| One round per number in an attempt | unique `(attempt_id, round_no)` / `(attempt_id, order_no)` |
| Exactly one correct option | partial unique index `ux_question_options_one_correct` + API validation |
| One pending appeal per ban | partial unique index `ux_appeals_one_pending` |
| A decided appeal has a reason | check constraint `ck_appeals_decision` |
| A refresh token belongs to a student or an admin, not both | check constraint |
| Sensible ranges (rounds, duration, marks, thresholds, file size) | check constraints |
| No double submission | row lock on the attempt inside a transaction |

JSONB is used only for generated puzzles/solutions/submissions, quiz question snapshots, game
options, violation details and audit details. Everything relational is relational.
Images and videos are files; the database stores key, type, size, SHA-256 and retention date.
Questions and quizzes are soft-deleted (`is_deleted`) so history stays intact.

## Migrations

| Migration | Content |
|---|---|
| `Baseline` | The six original tables exactly as the old code mapped them. |
| `PlatformSchema` | Everything new, the new columns, and the one-off data carry-over. |

`DatabaseInitializer` runs at start-up:

* empty database → runs both migrations;
* database with all six original tables and no migration history → records `Baseline` as applied
  (does **not** run it), then runs `PlatformSchema`;
* only some of the original tables, or duplicate `auth_users.user_id` values → stops with a clear
  message instead of guessing.

Create a new migration after changing entities:

```bash
cd Backend/HemCompetition.API
dotnet ef migrations add <Name> -o Data/Migrations
```
