namespace SubmiSoonProject.Models
{
    public class FileEntity
    {
        public int FileId { get; set; }
        public int UserId { get; set; }
        public int AssessmentId { get; set; }
        public string OriginalFilename { get; set; } = null!;
        public string StoredFilename { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public int FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation properties
        public User User { get; set; } = null!;
        public Assessment Assessment { get; set; } = null!;
    }
}
