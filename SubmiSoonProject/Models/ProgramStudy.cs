namespace SubmiSoonProject.Models
{
    public class ProgramStudy
    {
        public int ProgramStudyId { get; set; }
        public int FacultyId { get; set; }
        public string Name { get; set; } = null!;

        // navigation properties
        public Faculty Faculty { get; set; } = null!;
    }
}
