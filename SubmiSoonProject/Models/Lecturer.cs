namespace SubmiSoonProject.Models
{
    public class Lecturer
    {
        public int UserId { get; set; }
        public string LecturerId { get; set; } = null!;
        public int ProgramStudyId { get; set; }

        // navigation properties
        public User User { get; set; } = null!;
        public ProgramStudy ProgramStudy { get; set; } = null!;
    }
}
