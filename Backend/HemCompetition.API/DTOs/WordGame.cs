
    public class WordGameInitResponseDto
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public IEnumerable<WordGameDataDto>? Words { get; set; }
    }

    public class WordGameDataDto
    {
        public int? WGID { get; set; }
        public int? WGTopicID { get; set; }
        public string? WG_TopicName { get; set; }
        public string? WG_Question { get; set; }
        public string? WG_Answer { get; set; }
    }

    public class WordGameSaveRequestDto
    {
        public int? Game24ID { get; set; }
        public int? GameTopicID { get; set; }
        public string? UserAnswer { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? TimeDiff { get; set; }
        public string? Remark { get; set; } // Null for normal play, "Skipped" for skips
    }
