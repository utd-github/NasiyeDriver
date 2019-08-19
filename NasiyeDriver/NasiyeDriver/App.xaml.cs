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
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTI1OTgzQDMxMzcyZTMyMmUzMFhldFRrMmk3SWdoY3g2bEFBd29yNWJRNlRib3RWK3lITWszbW1tRlozdW89");

            InitializeComponent();

            NetworkAccess current = Connectivity.NetworkAccess;


            if (current == NetworkAccess.Internet)
            {

                var auth = DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                if (auth != null)
                {
                    MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    MainPage = new NavigationPage(new WelcomePage(true));
                }

            }
            else
            {
                MainPage = new NavigationPage(new WelcomePage(false));
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
