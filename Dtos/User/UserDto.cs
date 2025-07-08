namespace Dtos
{
    /// <summary>
    /// DTO for returning user information. It safely excludes sensitive data like the password.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User's name.
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// User's email address.
        /// </summary>
        public required string Email { get; set; }
    }
}