namespace HemCompetition.API.DTOs
{
    public class ModuleStatusDto
    {
        public string Word_Game_Status { get; set; } = "S";
        public string Quiz_Game_Status { get; set; } = "S";
        public string Number_Game_Status { get; set; } = "S";
    }

    public class NumberGameStatusDto
    {
        public string kenken_Game_Status { get; set; } = "S";
        public string Futoshiki_Game_Status { get; set; } = "C";
        public string Game24_Game_Status { get; set; } = "S";
    }

    public class WordGameStatusDto
    {
        public string Scramble_Game_Status { get; set; } = "S";
        public string Puzzle_Game_Status { get; set; } = "S";
        public string Word_Game_Status { get; set; } = "S";
    }

    public class QuizStatusDto
    {
        public int IsQuiz1Locked { get; set; } = 0;
        public int IsQuiz2Locked { get; set; } = 0;
        public int IsQuiz3Locked { get; set; } = 0;
        public int IsQuiz4Locked { get; set; } = 0;
    }
}