using NasiyeDriver.Models;
using NasiyeDriver.Services;
using NasiyeDriver.Views;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NasiyeDriver
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public readonly IFirebaseAuthInterface _firebaseAuth;
        public static bool isreq = false;  
        public readonly IFirebaseDBInterface _firebaseDatabase;
        UserModel sUser = new UserModel();
        bool mIsFocused = false;
        Type CurrentItem;
        public MainPage()
        {
            
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            var tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += (s, e) => {
                var nav = new NavigationPage((Page)Activator.CreateInstance(typeof(ProfilePage)));
                Detail = nav;
                IsPresented = false;
            };

            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

            //userimage.GestureRecognizers.Add(tapGestureRecognizer);

            CurrentItem = typeof(HomePage);


        }

        private async void ShowTripRequestPopUp(RequestModel req)
        {
            if(req == null)
            {
                return;
            }
            if(req != null)
            {
                if (req.Status == "Pending")
                {
                    if (!isreq)
                    {
                        isreq = true;


                        CrossLocalNotifications.Current.Show("Taxi", "You're being requested");

                        MessagingCenter.Send<object, object>(this, "request", req);
                    }
                }
                else if (req.Status == "Accepted")
                {
                    VibrationCancel();
                    isreq = false;
                    if (isreq)
                    {
                        _firebaseDatabase.GetBusy(req.Driver.Key);
                        await Navigation.PushAsync(new TripPage(req.Key));
                    }
                   
                }
            }
            
        }

        private void VibrationCancel()
        {
            try
            {
                Vibration.Cancel();
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void SetProfile(UserModel User)
        {
            MessagingCenter.Send<object, object>(this, "profile", User);

            sUser = User;

            username.Text = User.Name;
            IsGestureEnabled = true;
            userimage.Source = ImageSource.FromUri(new Uri(User.Image));

            //userrating.Text = GetRating(User.Trips,User.Stars);
            //usertrips.Text = User.Trips;

            if(User.Status == "Online")
            {
                if (User.Trip != "00")
                {
                    App.Current.MainPage = new NavigationPage(new TripPage(User.Trip));
                }
               
                if (!isreq)
                {
                    isreq = true;
                    GetRequest(User.Key);
                }
                CrossLocalNotifications.Current.Show("Nasiye", "You're online now, waiting for requests");

            }
            if (User.Status == "Busy")
            {
                if(User.Trip != "00")
                {
                    App.Current.MainPage = new NavigationPage(new TripPage(User.Trip));
                }
                else
                {
                    _firebaseDatabase.GetOnline(User.Key);
                }
            }
            else
            {
                if (User.Trip != "00")
                {
                    App.Current.MainPage = new NavigationPage(new TripPage(User.Trip));
                }

                isreq = false;
            }

            loader.IsVisible = false;
            profilecon.IsVisible = true;
        }

        private string GetRating(string trips, string stars)
        {
            if(trips == "0")
            {
                return "Not Rated";
            }


            // Calculate Stars here




            // return the result;





            return "Not Rated";
        }

        private void GetRequest(string uid)
        {
            Action<Dictionary<string, RequestModel>> onValueEvent = (Dictionary<string, RequestModel> reqs) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("---> EVENT Get Request Firebase ");

                    Action onSetValueSuccess = () =>
                    {

                    };

                    Action<string> onSetValueError = (string errorDesc) =>
                    {

                    };

                    if (reqs != null)
                    {
                        foreach (KeyValuePair<string, RequestModel> item in reqs)
                        {
                            if (uid != null)
                            {
                                if (item.Key == uid)
                                {
                                    ShowTripRequestPopUp(item.Value);
                                }
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

            _firebaseDatabase.GetRequests("requests", onValueEvent);
        }

        private async void GetProfile()
        {
            string uid = await _firebaseAuth.GetCurrentUser();

            Action<Dictionary<string, UserModel>> onValueEvent = (Dictionary<string, UserModel> user) =>
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

                    if (user != null)
                    {
                        foreach (KeyValuePair<string, UserModel> item in user)
                        {
                            if (item.Key == uid)
                            {
                                SetProfile(item.Value);
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
           
            _firebaseDatabase.GetProfile("drivers", onValueEvent);
        }

        private void MenuItemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MasterDetailListItem item = e.SelectedItem as MasterDetailListItem;

            if (item != null)
            {
                
                NavigationPage nav = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));

                IsPresented = false;

               
                if (CurrentItem != item.TargetType)
                {
                    CurrentItem = item.TargetType;

                    Detail = nav;

                    if (item.TargetType == typeof(HomePage))
                    {
                        MessagingCenter.Subscribe<object, object>(this, "get", (ssender, data) =>
                        {
                            MessagingCenter.Send<object, object>(this, "profile", sUser);
                        });
                    }

                    if (item.TargetType == typeof(ProfilePage))
                    {
                        MessagingCenter.Send<object, object>(this, "profile", sUser);
                    }
                }

                MenuItemList.SelectedItem = null;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetProfile();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (mIsFocused)
            {
                return base.OnBackButtonPressed();
            }
            else
            {
                mIsFocused = true;
                IsPresented = true;
                return true;
            }
           
        }
    }
}
