namespace SubmiSoonProject.Models
{
    public enum EnrollmentStatus
    {
        active,
        dropped,
        completed
    }

    public class StudentEnrollment
    {
        public int StudentEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public EnrollmentStatus? Status { get; set; }

        // navigation properties
        public Student Student { get; set; } = null!;
        public Class Class { get; set; } = null!;
    }
}
