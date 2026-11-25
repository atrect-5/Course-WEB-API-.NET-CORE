using System.ComponentModel.DataAnnotations;

namespace Dtos.User
{
    /// <summary>
    /// DTO for changing the authenticated user's password.
    /// </summary>
    public class ChangePasswordDto
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        public required string NewPassword { get; set; }
    }
}
