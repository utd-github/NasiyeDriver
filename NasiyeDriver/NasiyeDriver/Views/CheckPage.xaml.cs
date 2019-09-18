using NasiyeDriver.Models;
using NasiyeDriver.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckPage : ContentPage
    {


        public readonly IFirebaseAuthInterface _firebaseAuth;
        public readonly IFirebaseDBInterface _firebaseDatabase;

        UserModel user;


        public CheckPage()
        {
            InitializeComponent();
            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

            CheckForInternet();
        }
        private async void GetProfile()
        {
            string uid = await _firebaseAuth.GetCurrentUser();

            Action<Dictionary<string, UserModel>> onValueEvent = (Dictionary<string, UserModel> users) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("---> EVENT Get Profile Firebase ");

                    Action onSetValueSuccess = () =>
                    {

                    };

                    Action<string> onSetValueError = (string errorDesc) =>
                    {

                    };

                    if (users != null)
                    {
                        foreach (KeyValuePair<string, UserModel> item in users)
                        {
                            if (item.Key == uid)
                            {
                                SetProfile(item.Value);
                                user = item.Value;
                            }
                        }

                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error Get Profile Firebase " + ex.Message);
                    throw;
                }
            };


            _firebaseDatabase.GetCheckProfile("users", onValueEvent);

        }

        private void SetProfile(UserModel value)
        {
            if(value == null)
            {
                _firebaseAuth.Singout();

                App.Current.MainPage = new NavigationPage(new WelcomePage());
            }
            if (value.Status == "Busy")
            {
                // navigate to main page
                App.Current.MainPage = new NavigationPage(new TripPage(value.Trip));
            }
            else
            {
                App.Current.MainPage = new  MainPage();
            }
        }


        // Check internet
        private void CheckForInternet()
        {
            NetworkAccess current = Connectivity.NetworkAccess;


            if (current == NetworkAccess.Internet)
            {
                GetProfile();
            }
            else
            {
                loader.IsVisible = false;
                internetinfo.IsVisible = true;
            }
        }

        // Check if verified
        // Check if user signed up
        private void Refresh_Clicked(object sender, EventArgs e)
        {
            NetworkAccess current = Connectivity.NetworkAccess;


            if (current == NetworkAccess.Internet)
            {

                var auth = DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                if (auth != null)
                {
                    App.Current.MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    App.Current.MainPage = new NavigationPage(new WelcomePage());
                }


            }
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _firebaseDatabase.RemoveGetCheckProfile("users");
        }

    }
}