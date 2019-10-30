using NasiyeDriver.Services;
using NasiyeDriver.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver
{
    public partial class App : Application
    {
        public readonly IFirebaseDBInterface _firebaseDatabase;

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTYzNTA1QDMxMzcyZTMzMmUzMFVTWi9OZ0V6Tkpka2k4VHJzMnQ3V0JKa216UkpENGVLZlZReGhyUEZaL0k9");

            InitializeComponent();

            NetworkAccess current = Connectivity.NetworkAccess;
            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();


            if (current == NetworkAccess.Internet)
            {

                var auth = DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                if (auth != null)
                {
                    MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    MainPage = new NavigationPage(new WelcomePage());
                }

            }
            else
            {
                MainPage = new NavigationPage(new CheckPage());
            }


        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
           
        }

        protected override void OnResume()
        {
            // Handle when your app resumes





        }
    }
}
