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
        bool IsFocused = false;
        MasterDetailListItem CurrentItem;
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

            userimage.GestureRecognizers.Add(tapGestureRecognizer);
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
                    CrossLocalNotifications.Current.Show("Hail", "You're being hailed");
                    if (isreq)
                    {
                        MessagingCenter.Send<object, object>(this, "request", req);
                    }
                }
                else if (req.Status == "Accepted")
                {
                    isreq = false;
                    if (isreq)
                    {
                        _firebaseDatabase.GetBusy(req.Driver.Key);
                        await Navigation.PushAsync(new TripPage(req.Key));
                    }
                   
                }
            }
            
        }

        private void SetProfile(UserModel User)
        {
            MessagingCenter.Send<object, object>(this, "profile", User);

            sUser = User;

            username.Text = User.Name;
            IsGestureEnabled = true;
            userimage.Source = ImageSource.FromUri(new Uri(User.Image));

            userrating.Text = User.Rating.ToString();
            usertrips.Text = User.Status;

            if(User.Status == "Online")
            {
                if (!isreq)
                {
                    isreq = true;
                    GetRequest(User.Key);
                }
            }
            else
            {
                isreq = false;
                _firebaseDatabase.RemoveGetRequests("requests");
            }
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

               if(CurrentItem == null)
                {
                    CurrentItem = item;
                }

                if (CurrentItem != item)
                {
                    CurrentItem = item;

                    Detail = nav;

                    if (item.TargetType == typeof(HomePage))
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
            _firebaseDatabase.RemoveGetProfile("drivers");
        }

        protected override bool OnBackButtonPressed()
        {
            if (IsFocused)
            {
                return base.OnBackButtonPressed();
            }
            else
            {
                IsFocused = true;
                IsPresented = true;
                return true;
            }
           
        }
    }
}
