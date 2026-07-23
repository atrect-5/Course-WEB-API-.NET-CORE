using Models;
using System.Security.Cryptography;

namespace Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is being generated.</param>
        /// <returns>The generated JWT token.</returns>
        string GenerateJWToken(User user);

        
    }
}
