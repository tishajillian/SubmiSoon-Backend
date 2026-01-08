namespace SubmiSoonProject.Models
{
    public enum AssessmentStatus
    {
        draft,
        on_review,
        completed
    }

    public class UserAssessment
    {
        public int UserAssessmentId { get; set; }
        public int UserId { get; set; }
        public int AssessmentId { get; set; }
        public AssessmentStatus Status { get; set; }
        public int? Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation properties
        public User User { get; set; } = null!;
        public Assessment Assessment { get; set; } = null!;
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
