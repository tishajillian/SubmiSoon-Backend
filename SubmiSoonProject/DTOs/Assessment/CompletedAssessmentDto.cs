namespace SubmiSoonProject.DTOs.Assessment
{
    public class CompletedAssessmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string LecturerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
