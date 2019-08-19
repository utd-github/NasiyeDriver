using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NasiyeDriver.Services
{
    public interface IFirebaseAuthInterface
    {
          /// <summary>
        /// Login / Signup with email and password.
        /// </summary>
        /// <returns>OAuth token</returns>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        Task<string> LoginWithEmailPasswordAsync(string email, string password);
        void Singout();

        /// <returns>User Object</returns>
        // Get Current User
        Task<string> GetCurrentUser();
    }
}
