using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Foundation;
using NasiyeDriver.iOS.Services;
using NasiyeDriver.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OFirebaseAuthClass))]


namespace NasiyeDriver.iOS.Services
{
    class OFirebaseAuthClass : IFirebaseAuthInterface
    {
       
        public Task<string> GetCurrentUser()
        {
            var user = Auth.DefaultInstance.CurrentUser;

            if (user != null)
            {
                // User is signed in
                return Task.FromResult(user.Uid.ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<string> LoginWithEmailPasswordAsync(string email, string password)
        {
            AuthDataResult res;

            try
            {
                res = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
            }
            catch (Exception ex)
            {
                return await Task.FromResult("false:" + ex.Message);
            }
            return null;
        }

        public async Task<string> SignupWithEmailPasswordAsync(string email, string password)
        {
            try
            {
                AuthDataResult res = await Auth.DefaultInstance.CreateUserAsync(email, password);

                return await Task.FromResult(res.User.Uid);

            }
            catch (NSErrorException ex)
            {
            return await Task.FromResult("Error");

            }

        }

        public void Singout()
        {
            
        }
    }
}