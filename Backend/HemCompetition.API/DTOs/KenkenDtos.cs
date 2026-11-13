using System.Collections.Generic;

namespace HemCompetition.API.DTOs
{
    public class KenkenQuestionDto
    {
        public int KCMID { get; set; }
        public int Size { get; set; }
        public string[][] CageGrid { get; set; } = new string[0][];
        public string[][] TargetGrid { get; set; } = new string[0][];
    }

    public class SaveKenkenAnswerDto
    {
        public long UserId { get; set; }
        public int KCMID { get; set; }
        public string UserAnswer { get; set; } = string.Empty; // Grid input values flat (e.g. "321213132")
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsLastQuestion { get; set; }
        public int TimeDiff {  get; set; }
    }

    public class SkipKenkenDto
    {
        public long UserId { get; set; }
        public int KCMID { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int TimeDiff { get; set; }
    }
}