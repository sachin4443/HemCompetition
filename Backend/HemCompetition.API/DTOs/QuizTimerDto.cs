namespace HemCompetition.API.DTOs
{
    public class QuizTimerDto
    {
        public string? Type { get; set; }
        public int? Duration { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
    public class ExamModel
    {
        int _quizTime = 10;
        public int QuizTime { get => _quizTime; set => _quizTime = value; }
        public int QuizId { get; set; }
        public int LevelID { get; set; }
        public int TopicID { get; set; }
        public string? IsInstituteQuiz { get; set; }
        public string? Iscertificate { get; set; }
        public bool IsAttempts { get; set; }
    }

    public class QuestionViewModel1
    {
        public QuestionViewModel1()
        {
            QuestionOption = new List<QuestionOptionViewModel>();
        }

        public int? QuestionID { get; set; }

        public string QuestionName { get; set; } = string.Empty;
        public int? Marks { get; set; }
        public string Type { get; set; } = string.Empty;
        public string ComplexityLevel { get; set; } = string.Empty;

        public string AnswerDescription { get; set; } = string.Empty;
        public int? CourseID { get; set; }
        public int? TopicID { get; set; }
        public List<QuestionOptionViewModel> QuestionOption { get; set; }
    }

    public class QuestionOptionViewModel
    {
        public int? OptionID { get; set; }
        public int? QuestionID { get; set; } // Added this to bind options correctly in C# memory
        public string Options { get; set; } = string.Empty;
        public bool IsAnswer { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class ExamResultModel
    {
        public int ExamID { get; set; }
        public int CorrectQuestion { get; set; }
        public int InCorrectQuestion { get; set; }
        public int UnattemptedQuestion { get; set; }
        public int Coins { get; set; }
        public int Rank { get; set; }
        public int QuizGems { get; set; }
    }
    public class ExamSubmitModel
    {
        public int ExamID { get; set; }
        public int questionID { get; set; }
        public int selectedOption { get; set; }
    }
}
