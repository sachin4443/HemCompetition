# Moving content out of SQL Server (one-off)

The application no longer connects to SQL Server. Students were already in PostgreSQL. What still
lives only in SQL Server is **content** (quiz questions, word lists). Export it once to CSV and
load it through the admin panel. Table and column names below are taken from the old scaffolded
models; adjust the `WHERE` clauses to pick the right quizzes.

## Quiz questions → Question bank import

Run in SSMS, "Results to file" (CSV with headers), one file per quiz:

```sql
SELECT
    'Mega Quiz 1 question bank'                                   AS category,
    q.QuestionName                                                AS question,
    MAX(CASE WHEN o.rn = 1 THEN o.Options END)                    AS option_a,
    MAX(CASE WHEN o.rn = 2 THEN o.Options END)                    AS option_b,
    MAX(CASE WHEN o.rn = 3 THEN o.Options END)                    AS option_c,
    MAX(CASE WHEN o.rn = 4 THEN o.Options END)                    AS option_d,
    MAX(CASE WHEN o.IsAnswer = 1 THEN CHAR(64 + o.rn) END)        AS correct,
    q.AnswerDescription                                           AS explanation,
    COALESCE(NULLIF(q.ComplexityLevel, ''), 'Medium')             AS difficulty,
    COALESCE(q.Marks, 1)                                          AS marks
FROM Question q
JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY QuestionID ORDER BY OptionID) AS rn
      FROM QuestionOption WHERE Options IS NOT NULL AND Options <> 'NULL') o
  ON o.QuestionID = q.QuestionID
WHERE q.TopicID = /* topic of this quiz */ 0
GROUP BY q.QuestionID, q.QuestionName, q.AnswerDescription, q.ComplexityLevel, q.Marks;
```

Then: **Admin → Question bank → Import CSV / Excel**. `difficulty` must be Easy, Medium or Hard.
The import is all-or-nothing and reports the row number of anything it rejects; importing the same
file twice does not create duplicates. Afterwards open **Mega Quiz**, set the rules, **Preview**, **Publish**.

## Word lists → Word bank import

```sql
-- Word Challenge (clue + answer)
SELECT WG_Answer AS word, WG_Question AS clue, WG_TopicName AS topic FROM /* word game table */;
-- Puzzle words
SELECT Answer AS word FROM Puzzle_Comp_Master;
```

**Admin → Word bank → Import**. Words must be 3–20 letters without spaces. Scramble and Puzzle
are generated from this list; Word Challenge uses the entries that have a clue. A starter list of
144 words is loaded automatically into an empty word bank so the games work before any import.

## Past results

Old attempts are not copied. Download the old reports as CSV before SQL Server is switched off if
they must be kept.

## Number games

Nothing to migrate: Game 24, KenKen and Futoshiki are generated for every attempt.
