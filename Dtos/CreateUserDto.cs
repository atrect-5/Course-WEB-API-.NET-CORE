using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    /// <summary>
    /// DTO for creating a new user.
    /// </summary>
    public class CreateUserDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}