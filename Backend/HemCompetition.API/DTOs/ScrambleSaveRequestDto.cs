namespace HemCompetition.API.DTOs
{
    public class ScrambleSaveRequestDto
    {
        public string? Answer { get; set; }
        public int QuestionId { get; set; }
        public int TopicId { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? TimeDiff { get; set; }
        public string? Remark { get; set; }
    }
}
