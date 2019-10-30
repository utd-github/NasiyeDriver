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

        FirebaseAuth mAuth;

        public AFirebaseAuthClass()
        {
            var instance = FirebaseAuth.GetInstance(MainActivity.app);
            if (instance == null)
            {
                mAuth = new FirebaseAuth(MainActivity.app);
            }
            else
            {
                mAuth = instance;
            }
        }

        public Task<string> GetCurrentUser()
        {
            var instance = FirebaseAuth.GetInstance(MainActivity.app);
            if (instance == null)
            {
                mAuth = new FirebaseAuth(MainActivity.app);
            }
            else
            {
                mAuth = instance;
            }

            if (mAuth.CurrentUser != null)
            {
                // User is signed in
                return Task.FromResult(mAuth.CurrentUser.Uid.ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<string> LoginWithEmailPasswordAsync(string email, string password)
        {
            var instance = FirebaseAuth.GetInstance(MainActivity.app);
            if (instance == null)
            {
                mAuth = new FirebaseAuth(MainActivity.app);
            }
            else
            {
                mAuth = instance;
            }

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