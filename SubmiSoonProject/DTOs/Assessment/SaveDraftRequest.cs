namespace SubmiSoonProject.DTOs.Assessment
{
    public class SaveDraftRequest
    {
        public List<AnswerInput> Answers { get; set; } = new List<AnswerInput>();
    }
}
