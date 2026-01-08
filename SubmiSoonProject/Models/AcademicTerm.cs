namespace SubmiSoonProject.Models
{
    public enum SemesterType
    {
        odd,
        even
    }

    public class AcademicTerm
    {
        public int AcademicTermId { get; set; }
        public int Year { get; set; }
        public SemesterType Semester { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
