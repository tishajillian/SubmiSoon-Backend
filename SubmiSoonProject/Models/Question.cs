namespace SubmiSoonProject.Models
{
    public enum QuestionType
    {
        essay,
        mcq,
        file
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public int AssessmentId { get; set; }
        public QuestionType Type { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation properties
        public Assessment Assessment { get; set; } = null!;
        public ICollection<McqOption> McqOptions { get; set; } = new List<McqOption>();
    }
}
