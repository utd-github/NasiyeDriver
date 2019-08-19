using NasiyeDriver.Components;
using NasiyeDriver.Models;
using NasiyeDriver.Services;
using NasiyeDriver.Views;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NasiyeDriver
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public static bool showing = false;

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


            userimage.GestureRecognizers.Add(tapGestureRecognizer);


            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            IsGestureEnabled = false;
            
            MessagingCenter.Subscribe<object, string>(this, "profile", (sender, data) =>
            {
                SetProfile(datatoModel(data));
            });

            MessagingCenter.Subscribe<object, string>(this, "request", (sender, data) =>
            {
                ShowTripRequestPopUp(ToReqModel(data));
            });

            GetProfile();

        }

        private RequestModel ToReqModel(string data)
        {
            RequestModel req = null;

            if(data != null)
            {
              try
                {
                    req = JsonConvert.DeserializeObject<RequestModel>(data);

                            return req;
                }
                catch
                {
                    return null;
                }

            }else
            {
                return null;
            }
          
        }

        private async void ShowTripRequestPopUp(RequestModel req)
        {
            if (showing)
            {
                return;
            }
            if(req.Status == "Pending")
            {
                showing = true;
                // Open a PopupPage
                await Navigation.PushPopupAsync(new TripPopUp(req));
            }
        }

        private UserModel datatoModel(string data)
        {
            UserModel user = null;

            try
            {
                user = JsonConvert.DeserializeObject<UserModel>(data);

                return user;
            }
            catch
            {
                return null;
            }
        }

        private async void SetProfile(UserModel User)
        {
            username.Text = User.Name;
            IsGestureEnabled = true;

            userimage.Source = new UriImageSource
            {
                Uri = User.Image
            };

            userrating.Text = User.Rating.ToString();
            usertrips.Text = User.Status;
            if(User.Status == "Online")
            {
                string auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                if (auth != null)
                {
                    DependencyService.Get<IFirebaseDBInterface>().ListenToquest(auth);
                }
            }

        }

        private async void GetProfile()
        {
            string auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();
            
            if (auth != null)
            {
                DependencyService.Get<IFirebaseDBInterface>().GetUserProfile(auth);
            }
        }

        private void MenuItemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MasterDetailListItem item = e.SelectedItem as MasterDetailListItem;
            if (item != null)
            {
                NavigationPage nav = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
               

                Detail = nav;

                MenuItemList.SelectedItem = null;

                IsPresented = false;

            }
        }

    }
}
