namespace SubmiSoonProject.DTOs.Assessment
{
    public class AnswerInput
    {
        public int QuestionId { get; set; }
        public string AnswerType { get; set; } = string.Empty;
        public string? Text { get; set; }
        public int? OptionId { get; set; }
        public IFormFile? File { get; set; }
    }
}
