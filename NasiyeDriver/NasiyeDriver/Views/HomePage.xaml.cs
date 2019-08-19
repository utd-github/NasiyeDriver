using NasiyeDriver.Models;
using NasiyeDriver.Services;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        
        Plugin.Geolocator.Abstractions.Position savedPosition;

        public HomePage()
        {
            InitializeComponent();

            GetUserLocation();
            GetProfile();

        }
        private async void GetProfile()
        {
            string auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            if (auth != null)
            {
                DependencyService.Get<IFirebaseDBInterface>().GetUserProfile(auth);
            }
        }

        private async void SetStatusAsync(UserModel user)
        {
            container.Title = user.Status;

            if(user.Status == "Offline")
            {
                // Hide Loader
                await StartTrackingAsync(false);

                getOnline.IsVisible = true;
                loader.IsVisible = false;
            }else
            {

                getOnline.IsVisible = false;
                loader.IsVisible = false;
                infomodel.Text = user.Vehicle.Model;
                infoplate.Text = user.Vehicle.Plate;
                infotype.Text = user.Vehicle.Type;
                infobox.IsVisible = true;
                //StartUpudatingLocation
                await StartTrackingAsync(true);

            }

        }

        private async Task StartTrackingAsync(bool tracking)
        {
            try
            {
                PermissionStatus hasPermission = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (hasPermission != PermissionStatus.Denied)
                {
                    // Permission Granted
                    // check for req
                    if (tracking)
                    {
                        // Check is listening...
                        if (CrossGeolocator.Current.IsListening)
                        {
                            // Start

                            CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                            CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
                        }
                        else
                        {
                            CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                            CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;

                            object aType = "Automotive Navigation";
                            if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 50,
                                true, new ListenerSettings
                                {
                                    ActivityType = ActivityType.AutomotiveNavigation,
                                    AllowBackgroundUpdates = true,
                                    DeferLocationUpdates = true,
                                    DeferralDistanceMeters = 100,
                                    DeferralTime = TimeSpan.FromSeconds(10),
                                    ListenForSignificantChanges = true,
                                    PauseLocationUpdatesAutomatically = false
                                }))
                            {
                                
                                tracking = true;
                            }
                        }

                    }else
                    {
                        CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                        CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;

                        await CrossGeolocator.Current.StopListeningAsync();
                      

                        tracking = false;

                    }
                   
                }
                else
                {
                    // Permission Denied
                    await DisplayAlert("Location Error", "Error Getting Location", "OK");
                }

            }
            catch (Exception ex)
            {
                // Error Accured
                await DisplayAlert("Location Error", "Error Getting Location: " + ex.Message, "OK");
            }



        }

        private void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e)
        {
         
        }

        private void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Models.Location local = new Models.Location();

                var position = e.Position;

                mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude), Distance.FromMiles(1)).WithZoom(2));

                local.Lat = position.Latitude;
                local.Lng = position.Longitude;

                var uid = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                await DependencyService.Get<IFirebaseDBInterface>().UpdateDriverLocation(uid,local);
            });
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

        private async void GetUserLocation()
        {
            MessagingCenter.Subscribe<object, string>(this, "profile", (sender, data) =>
            {
                SetStatusAsync(datatoModel(data));
            });
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                
                if (location != null)
                {
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude), Distance.FromMiles(1)).WithZoom(2));

                    maploading.IsVisible = false;
                    mainmap.IsVisible = true;

                    string slocal = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Error", "Sorry, Maps not supported!", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Error", "Sorry, Maps not enabled!", "OK");

            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Error", "Sorry, Maps need permission", "OK");

            }
            catch (Exception ex)
            {
                // Unable to get location

                await DisplayAlert("Error", "Sorry, Unable to get location!", "OK");
            }
        }

        private async void GetOnline_Clicked(object sender, EventArgs e)
        {
            // SHow Loader
            getOnline.IsVisible = false;
            loader.IsVisible = true;
            var uid = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            if(uid != null)
            {
                await DependencyService.Get<IFirebaseDBInterface>().GetOnline(uid);
            }
        }

        private async void Getoffline_Clicked(object sender, EventArgs e)
        {
            infobox.IsVisible = false;
            loader.IsVisible = true;

            var user = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            bool dbres = await DependencyService.Get<IFirebaseDBInterface>().GetOffline(user);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetUserLocation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object, string>(this, "profile");
            MessagingCenter.Unsubscribe<object, string>(this, "activeprofile");
        }

    }
}