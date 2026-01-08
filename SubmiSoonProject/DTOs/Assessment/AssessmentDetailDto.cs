namespace SubmiSoonProject.DTOs.Assessment
{
    public class AssessmentDetailDto
    {
        public AssessmentInfoDto Assessment { get; set; } = null!;
        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class AssessmentInfoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Status { get; set; }
        public int? Score { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public string AnswerType { get; set; } = null!;
        public AnswerDto? Answer { get; set; }
        public List<McqOptionDto>? Options { get; set; }
    }

    public class AnswerDto
    {
        public string? Text { get; set; }
        public McqSelectionDto? Mcq { get; set; }
        public FileInfoDto? File { get; set; }
    }

    public class McqSelectionDto
    {
        public int OptionId { get; set; }
        public char Label { get; set; }
    }

    public class McqOptionDto
    {
        public int OptionId { get; set; }
        public char Label { get; set; }
        public string Text { get; set; } = null!;
    }

    public class FileInfoDto
    {
        public int FileId { get; set; }
        public string Filename { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public int Size { get; set; }
        public string DownloadUrl { get; set; } = null!;
        public string PreviewUrl { get; set; } = null!;
    }
}
