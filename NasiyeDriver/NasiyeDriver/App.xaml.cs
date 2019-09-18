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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTI1OTgzQDMxMzcyZTMyMmUzMFhldFRrMmk3SWdoY3g2bEFBd29yNWJRNlRib3RWK3lITWszbW1tRlozdW89");

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

        protected async override void OnSleep()
        {
            NetworkAccess current = Connectivity.NetworkAccess;

            // Handle when your app sleeps
            if (current == NetworkAccess.Internet)
            {

                var auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                if (auth != null)
                {
                    _firebaseDatabase.CancelRequest(auth);
                    _firebaseDatabase.GetOffline(auth);
                }
                else
                {
                   
                }

            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
