using System;

namespace HemCompetition.API.DTOs
{
    public class Game24QuestionDto
    {
        public long GCID { get; set; }
        public int QuestionNo { get; set; } // Added Question Number
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public int Number3 { get; set; }
        public int Number4 { get; set; }
    }

    public class Save24AnswerDto
    {
        public long UserId { get; set; }
        public long GCID { get; set; }
        public int QuestionNo { get; set; } // Important for proper DB mapping
        public string UserAnswer { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int GameId { get; set; } = 3; // Game24 default ID
        public string? TT { get; set; } // Added if needed
    }

    public class SkipDto
    {
        public long UserId { get; set; }
        public long GCID { get; set; }
        public int QuestionNo { get; set; } // Important for proper DB mapping
        public int GameId { get; set; } = 3;
    }
}