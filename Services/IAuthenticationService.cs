using Dtos;
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
        /// <returns>A <see cref="UserDto"/> if authentication is successful; otherwise, <see langword="null"/>.</returns>
        UserDto? Authenticate(string email, string password);

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        void Logout();
    }
}
