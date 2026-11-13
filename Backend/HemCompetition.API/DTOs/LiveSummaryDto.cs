namespace HemCompetition.API.DTOs.Reports
{
    public class LiveParticipantDto
    {
        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserRoles { get; set; }

        public string? RoleCategory { get; set; }

        public string? MobileNo { get; set; }

        public string? EmailID { get; set; }

        public string? CityName { get; set; }

        public string? SchoolName { get; set; }

        public QuizSummaryDto Quiz { get; set; } = new();

        public NumberGameSummaryDto NumberGame { get; set; } = new();

        public WordGameSummaryDto WordGame { get; set; } = new();

        public decimal CombinedScore { get; set; }

        public bool HasQuizActivity { get; set; }

        public bool HasNumberGameActivity { get; set; }

        public bool HasWordGameActivity { get; set; }

        public int GamesStarted { get; set; }
    }


    public class QuizSummaryDto
    {
        public int Attempts { get; set; }

        public decimal TotalMarksObtain { get; set; }

        public int TotalTime { get; set; }

        public decimal Quiz1 { get; set; }

        public decimal Quiz2 { get; set; }

        public decimal Quiz3 { get; set; }

        public decimal Quiz4 { get; set; }

        public List<QuizAttemptDto> AttemptsList { get; set; } = new();
    }


    public class QuizAttemptDto
    {
        public decimal Quiz1 { get; set; }

        public int Quiz1Time { get; set; }

        public decimal Quiz2 { get; set; }

        public int Quiz2Time { get; set; }

        public decimal Quiz3 { get; set; }

        public int Quiz3Time { get; set; }

        public decimal Quiz4 { get; set; }

        public int Quiz4Time { get; set; }

        public decimal TotalMarksObtain { get; set; }

        public int TotalTime { get; set; }
    }


    public class NumberGameSummaryDto
    {
        public int FutoshikiAnswer { get; set; }
        public int FutoshikiSkipped { get; set; }
        public int FutoshikiNext { get; set; }
        public decimal FutoshikiScore { get; set; }
        public int FutoshikiTotalTime { get; set; }

        public int Game24Answer { get; set; }
        public int Game24Skipped { get; set; }
        public int Game24Next { get; set; }
        public decimal Game24Score { get; set; }
        public int Game24TotalTime { get; set; }

        public int KenKenAnswer { get; set; }
        public int KenKenSkipped { get; set; }
        public int KenKenNext { get; set; }
        public decimal KenKenScore { get; set; }
        public int KenKenTotalTime { get; set; }

        public decimal TotalScore { get; set; }
        public int TotalTime { get; set; }
    }


    public class WordGameSummaryDto
    {
        public int PuzzleAnswer { get; set; }
        public int PuzzleSkipped { get; set; }
        public int PuzzleNext { get; set; }
        public decimal PuzzleScore { get; set; }
        public int PuzzleTotalTime { get; set; }

        public int ScrambleAnswer { get; set; }
        public int ScrambleSkipped { get; set; }
        public int ScrambleNext { get; set; }
        public decimal ScrambleScore { get; set; }
        public int ScrambleTotalTime { get; set; }

        public int WordGameAnswer { get; set; }
        public int WordGameSkipped { get; set; }
        public int WordGameNext { get; set; }
        public decimal WordGameScore { get; set; }
        public int WordGameTotalTime { get; set; }

        public decimal TotalScore { get; set; }
        public int TotalTime { get; set; }
    }
}