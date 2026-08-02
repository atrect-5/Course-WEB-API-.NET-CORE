using System.Security.Cryptography;
using System.Text;

namespace Repositories
{
    public static class RefreshTokenUtils
    {
        /// <summary>
        /// Generates a cryptographically secure random token (Base64) and its expiry.
        /// </summary>
        public static (string Token, DateTime ExpiresAt) GenerateRefreshToken(int entropyBytes = 64, int validDays = 45)
        {
            var randomBytes = new byte[entropyBytes];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);
            var expires = DateTime.UtcNow.AddDays(validDays);
            return (token, expires);
        }

        /// <summary>
        /// Computes HMAC-SHA256 of the token using the provided secretKey.
        /// Returns Base64 encoded hash suitable for storage/comparison.
        /// </summary>
        public static string HashTokenHmacSha256(string token, string secretKey)
        {
            ArgumentNullException.ThrowIfNull(token);
            if (string.IsNullOrEmpty(secretKey)) throw new ArgumentNullException(nameof(secretKey));

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var data = Encoding.UTF8.GetBytes(token);
            using var hmac = new HMACSHA256(keyBytes);
            var mac = hmac.ComputeHash(data);
            return Convert.ToBase64String(mac);
        }

        /// <summary>
        /// Verifies token against stored Base64 HMAC-SHA256 hash using constant-time comparison.
        /// </summary>
        public static bool VerifyHashHmacSha256(string token, string storedBase64Hash, string secretKey)
        {
            if (token is null || storedBase64Hash is null) return false;
            var computed = HashTokenHmacSha256(token, secretKey);
            var a = Convert.FromBase64String(computed);
            var b = Convert.FromBase64String(storedBase64Hash);
            return CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}