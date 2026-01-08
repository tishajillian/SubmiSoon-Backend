namespace SubmiSoonProject.DTOs.Assessment
{
    public class SaveDraftResponse
    {
        public int AssessmentId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public int SavedAnswers { get; set; }
        public List<UploadedFileDto> UploadedFiles { get; set; } = new();
    }

    public class SubmitAssessmentResponse
    {
        public int AssessmentId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime SubmittedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public List<UploadedFileDto> UploadedFiles { get; set; } = new();
    }

    public class UploadedFileDto
    {
        public int QuestionId { get; set; }
        public int FileId { get; set; }
        public string Filename { get; set; } = null!;
        public int Size { get; set; }
        public string DownloadUrl { get; set; } = null!;
        public string PreviewUrl { get; set; } = null!;
    }
}
