namespace SwiftCaps.Models.Models
{
    public class GroupAverageReportItem
    {
        public string GroupName { get; set; }
        public int QuizCount { get; set; }
        public int? AverageDonePercentage { get; set; } = null;
        public string AvergageTime { get; set; }
        public int UserCount { get; set; }
    }
}
