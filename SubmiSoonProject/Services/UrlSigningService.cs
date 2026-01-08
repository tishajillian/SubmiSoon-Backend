using System.Security.Cryptography;
using System.Text;

namespace SubmiSoonProject.Services
{
    public interface IUrlSigningService
    {
        /// Generate a signed token for file access
        /// returns Tuple containing (token, expirationTimestamp)
        (string token, long expires) GenerateFileAccessToken(int fileId, int userId, int validMinutes = 3);

        /// Validate a signed file access token
        /// returnd True if valid, false otherwise
        bool ValidateFileAccessToken(int fileId, int userId, long expires, string token);
    }
    public class UrlSigningService : IUrlSigningService
    {
        private readonly string _signingKey;

        public UrlSigningService(IConfiguration configuration)
        {
            _signingKey = configuration["UrlSigning:SecretKey"] 
                ?? throw new InvalidOperationException("UrlSigning:SecretKey not configured");
        }

        public (string token, long expires) GenerateFileAccessToken(int fileId, int userId, int validMinutes = 3)
        {
            var expires = DateTimeOffset.UtcNow.AddMinutes(validMinutes).ToUnixTimeSeconds();
            var payload = $"{fileId}|{userId}|{expires}";
            
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_signingKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var token = Convert.ToBase64String(hash)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", ""); // URL-safe base64
            
            return (token, expires);
        }

        public bool ValidateFileAccessToken(int fileId, int userId, long expires, string token)
        {
            // Check expiration
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expires)
            {
                return false;
            }

            // Recompute signature
            var payload = $"{fileId}|{userId}|{expires}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_signingKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var expectedToken = Convert.ToBase64String(hash)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedToken),
                Encoding.UTF8.GetBytes(token)
            );
        }
    }
}
