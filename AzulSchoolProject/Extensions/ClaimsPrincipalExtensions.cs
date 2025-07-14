using System.Security.Claims;

namespace AzulSchoolProject.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the user ID from the specified <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="user">The <see cref="ClaimsPrincipal"/> instance representing the authenticated user.</param>
        /// <returns>The user ID as an integer.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="ClaimsPrincipal"/> does not contain a valid user ID claim or if the claim value
        /// cannot be parsed as an integer.</exception>
        public static int GetUserId (this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string .IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new InvalidOperationException("El token de usuario no contiene un ID de usuario válido.");

            return userId;
        }
    }
}
