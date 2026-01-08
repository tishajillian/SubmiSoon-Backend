namespace SubmiSoonProject.DTOs.Auth
{
    public class AuthResponse
    {
        public required UserInfo User { get; set; }
        public required string ExpiresAt { get; set; }
        public required string AccessToken { get; set; }
        public bool Success { get; set; } = true;
    }

    public class UserInfo
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}