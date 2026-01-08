using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace SubmiSoonProject.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Generate random salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8, // threads
            Iterations = 4,          // time cost
            MemorySize = 65536       // 64 MB
        };

        byte[] hash = argon2.GetBytes(32);

        // Store as: salt.hash (Base64)
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            Iterations = 4,
            MemorySize = 65536
        };

        byte[] actualHash = argon2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
