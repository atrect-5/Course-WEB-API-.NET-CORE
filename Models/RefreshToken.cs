using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public required int UserId { get; set; }

        // Hashed (SHA-256 / HMAC) representation of the token
        public required string TokenHash { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }

        // FK to the replacing token (helps queries / integrity)
        public int? ReplacedByTokenId { get; set; }

        // The hash too — useful for audit and when the replacement row may be purged
        public string? ReplacedByTokenHash { get; set; }

        public string? CreatedByIp { get; set; }
        public string? UserAgent { get; set; }

        // Navegación opcional hacia el usuario
        public User? User { get; set; }

        // Navegación opcional hacia el token que reemplazó a éste
        public RefreshToken? ReplacedByToken { get; set; }
    }
}
