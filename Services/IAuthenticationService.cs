using Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user based on their credentials.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A JWT string if authentication is successful; otherwise, <see langword="null"/>.</returns>
        Task<string?> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <remarks>In a stateless JWT implementation, logout is typically handled client-side by deleting the token.
        /// A server-side implementation would require a token denylist.</remarks>
        Task LogoutAsync();
    }
}
