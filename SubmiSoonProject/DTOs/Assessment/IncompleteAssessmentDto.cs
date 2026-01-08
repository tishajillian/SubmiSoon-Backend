namespace SubmiSoonProject.DTOs.Assessment
{
    public class IncompleteAssessmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Class { get; set; } = null!;
        public string LecturerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
