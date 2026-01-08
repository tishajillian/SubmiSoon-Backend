namespace SubmiSoonProject.Models
{
    public class Class
    {
        public int ClassId { get; set; }
        public string ClassCode { get; set; } = null!;
        public int CourseId { get; set; }
        public int LecturerId { get; set; }
        public int AcademicTermId { get; set; }

        // navigation properties
        public Course Course { get; set; } = null!;
        public Lecturer Lecturer { get; set; } = null!;
        public AcademicTerm AcademicTerm { get; set; } = null!;
    }
}
