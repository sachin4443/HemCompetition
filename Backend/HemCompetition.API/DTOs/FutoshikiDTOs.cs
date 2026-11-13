namespace HemCompetition.API.DTOs
{
    public class FutoshikiQuestionDto
    {
        public int FCID { get; set; }
        public int[][]? InitialGrid { get; set; }
        public string[][]? HSigns { get; set; }
        public string[][]? VSigns { get; set; }
    }

    public class ApiResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    public class SaveFutoshikiAnswerDto
    {
        public int UserId { get; set; }
        public int FCID { get; set; }
        public string? UserAnswer { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsLastQuestion { get; set; }
    }

    public class SkipFutoshikiDto
    {
        public long UserId { get; set; }
        public long FCID { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}