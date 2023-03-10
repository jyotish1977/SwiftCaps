using System;
using SwiftCaps.Models.Enums;

namespace SwiftCaps.Models.Models
{
    public class QuizReport
    {
        public virtual Guid Id { get; set; }
        public Recurrence Recurrence { get; set; }
        public bool IsCompleted { get; set; }
        public Guid QuizId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
