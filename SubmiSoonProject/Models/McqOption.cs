namespace SubmiSoonProject.Models
{
    public class McqOption
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public char OptionLabel { get; set; }
        public string OptionText { get; set; } = null!;
        public bool IsCorrect { get; set; }

        // navigation properties
        public Question Question { get; set; } = null!;
    }
}
