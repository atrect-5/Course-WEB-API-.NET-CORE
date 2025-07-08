namespace Dtos
{
    /// <summary>
    /// DTO for updating an existing user. All properties are optional.
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// The new name for the user. If not provided, the name will not be changed.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The new password for the user. If not provided, the password will not be changed.
        /// </summary>
        public string? Password { get; set; }
    }
}