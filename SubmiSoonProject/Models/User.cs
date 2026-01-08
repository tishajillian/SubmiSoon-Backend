namespace SubmiSoonProject.Models
{
    public enum UserRole
    {
        student,
        lecturer
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
    }
}
