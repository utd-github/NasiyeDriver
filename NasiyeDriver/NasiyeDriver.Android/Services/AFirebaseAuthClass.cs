using Android.Widget;
using Firebase.Auth;
using NasiyeDriver.Droid.Services;
using NasiyeDriver.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AFirebaseAuthClass))]


namespace NasiyeDriver.Droid.Services
{
    class AFirebaseAuthClass : IFirebaseAuthInterface
    {
        public static int SucessCode = 0;

        public static int EmailNotFoundCode = 1;
        public static int EmailExistsCode = 2;
        public static int IncorrectPassword = 3;
        public static int WeakPasswordCode = 4;
        public static int ErrorUserDisAbled = 5;

        public static int ErrorEmptyParams = 6;

        FirebaseAuth mAuth = FirebaseAuth.GetInstance(MainActivity.app);

       
        public Task<string> GetCurrentUser()
        {
            var user = mAuth.CurrentUser;

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
            IAuthResult res;

            try
            {
                res = await mAuth.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception ex)
            {
                return await Task.FromResult("false:" + ex.Message);
            }
            return null;
        }

      
        public void Singout()
        {
           mAuth.SignOut();
        }
    }
}