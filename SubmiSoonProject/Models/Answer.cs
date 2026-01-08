namespace SubmiSoonProject.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public int UserAssessmentId { get; set; }
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }
        public int? SelectedOptionId { get; set; }
        public int? FileId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation properties
        public UserAssessment UserAssessment { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public McqOption? McqOption { get; set; }
        public FileEntity? File { get; set; }
    }
}
