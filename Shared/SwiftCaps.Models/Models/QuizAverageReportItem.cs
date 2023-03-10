namespace SwiftCaps.Models.Models
{
    public class QuizAverageReportItem
    {
        public string QuizName { get; set; }
        public int? DonePercentage { get; set; } = null;
        public string AvergageTime { get; set; }
        public int GroupCount { get; set; }
        public int UserCount { get; set; }
    }
}
