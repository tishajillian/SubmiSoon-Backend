namespace SubmiSoonProject.Models
{
    public class Student
    {
        public int UserId { get; set; }
        public string StudentId { get; set; } = null!;
        public int EnrollmentYear { get; set; }
        public int ProgramStudyId { get; set; }

        // navigation properties
        public User User { get; set; } = null!;
        public ProgramStudy ProgramStudy { get; set; } = null!;
    }
}
