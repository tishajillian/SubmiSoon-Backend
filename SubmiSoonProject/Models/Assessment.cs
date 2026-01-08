namespace SubmiSoonProject.Models
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation properties
        public Class Class { get; set; } = null!;
    }
}
