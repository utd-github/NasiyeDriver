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
using NasiyeDriver.CustomRenderers;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
       
        public readonly IFirebaseAuthInterface _firebaseAuth;
        public readonly IFirebaseDBInterface _firebaseDatabase;

        

        ObservableCollection<UserModel> DriversList { get; set; } = new ObservableCollection<UserModel>();

        RequestModel Mrequest;


        public HomePage()
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

          
            Mrequest = new RequestModel();

            MessagingCenter.Subscribe<object, object>(this, "profile", (sender, data) =>
            {
                UserModel user = data as UserModel;
                SetStatusAsync(user);
            });

            MessagingCenter.Subscribe<object, object>(this, "request", (sender, data) =>
            {
                RequestModel req = data as RequestModel;
                Mrequest = req;
                GetRequestData(req);
            });

        }

        private void GetDrivers()
        {
            Action<Dictionary<string, UserModel>> onValueEvent;
            onValueEvent = null;
            onValueEvent = (Dictionary<string, UserModel> drivers) =>
            {

                try
                {
                    System.Diagnostics.Debug.WriteLine("---> EVENT GetDataFromFirebase ");

                    Action onSetValueSuccess = () =>
                    {

                    };

                    Action<string> onSetValueError = (string errorDesc) =>
                    {

                    };

                    if (drivers == null)
                    {
                        mainmap.Pins.Clear();
                        DriversList.Clear();
                    }
                    else
                    {
                        if (drivers.Count != 0 && drivers.Count != DriversList.Count)
                        {
                            mainmap.Pins.Clear();
                            DriversList.Clear();

                            foreach (KeyValuePair<string, UserModel> item in drivers.ToList())
                            {
                                if (DriversList.All(d => d.Key != item.Value.Key) && item.Value.Status == "Online")
                                {
                                    DriversList.Add(item.Value);
                                }

                            }

                            if (DriversList.Count > 0)
                            {
                                SetDrivers(DriversList);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message);
                    throw;
                }
            };

            _firebaseDatabase.GetDrivers("drivers", onValueEvent);
        }



        private void SetDrivers(ObservableCollection<UserModel> drivers)
        {
            if (drivers != null)
            {
                ShowDriverLocations(drivers);
            }
            else
            {
                mainmap.Pins.Clear();
            }
        }



        private async void ShowDriverLocations(ObservableCollection<UserModel> drivers)
        {
            mainmap.Pins.Clear();

            foreach (UserModel driver in drivers)
            {
                var pin = new CustomPin
                {
                    Position = new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lng),
                    Label = "Xamarin San Francisco Office",
                    Address = "394 Pacific Ave, San Francisco CA",
                    MarkerId = "Xamarin",
                    Icon = "driver"
                };
                mainmap.CustomPins = new List<CustomPin> { pin };
                mainmap.Pins.Add(pin);
            }
            try
            {
                Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude),
                        Distance.FromMiles(1)).WithZoom(2));

                    var dpin = new CustomPin
                    {
                        Position = new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude),
                        Label = "Xamarin San Francisco Office",
                        Address = "394 Pacific Ave, San Francisco CA",
                        MarkerId = "Xamarin",
                        Icon = "driver"
                    };

                    mainmap.CustomPins.Add(dpin);
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
                await DisplayAlert("Error", "Sorry, Unable to get location! ERROR: " + ex.Message, "OK");
            }

        }

        

        private async void SetStatusAsync(UserModel user)
        {
            container.Title = user.Status;
            GetUserLocation();
            if (user.Status == "Offline")
            {
                // Hide Loader
                await StartTrackingAsync(false);

                loader.IsVisible = false;
                infobox.IsVisible = false;

                getOnline.IsVisible = true;
                getOnline.IsEnabled = true;

            }
            else if (user.Status == "Busy")
            {
                // get Request
            }
            else if(user.Status == "Online")
            {
                getOnline.IsVisible = false;
                loader.IsVisible = false;
                userimage.Source = ImageSource.FromUri(new Uri(user.Image));
                infomodel.Text = user.Vehicle.Model;
                infoplate.Text = user.Vehicle.Plate;
                infotype.Text = user.Vehicle.Type;
                getoffline.IsEnabled = true;
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
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Xamarin.Forms.Maps.Position(
                            position.Latitude, position.Longitude), 
                            Distance.FromMiles(1)).WithZoom(2)
                            );

                    local.Lat = position.Latitude;
                    local.Lng = position.Longitude;

                    var uid = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                    DependencyService.Get<IFirebaseDBInterface>().UpdateTripDriverLocation(uid,local);
                });
            }



        private async void GetUserLocation()
        {
            try
            {
                Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude), 
                        Distance.FromMiles(1)).WithZoom(2));

                    maploading.IsVisible = false;
                    mainmap.IsVisible = true;
                    geo.IsEnabled = true;
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
                await DisplayAlert("Error", "Sorry, Unable to get location! ERROR: " + ex.Message, "OK");
            }
        }

        private async void GetOnline_Clicked(object sender, EventArgs e)
        {
            // SHow Loader
            loader.IsVisible = true;
            getOnline.IsEnabled = false;
            
            var uid = await _firebaseAuth.GetCurrentUser();

            if(uid != null)
            {
                _firebaseDatabase.GetOnline(uid);
            }
        }

        private async void Getoffline_Clicked(object sender, EventArgs e)
        {
            loader.IsVisible = true;
            getoffline.IsEnabled = false;

            var user = await _firebaseAuth.GetCurrentUser();

            if(user != null)
            {
                _firebaseDatabase.GetOffline(user);
            }
        }


        private void GetRequestData(RequestModel req)
        {
            reqpopup.IsVisible = true;

            username.Text = req.User.Name;
            timer.Text = "60 Secs";



            int secs = 60;
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (secs == 0)
                {
                    Cancel_Clicked(null, null);
                }
                secs--;
                if (secs <= 0)
                {

                    timer.Text = 0 + " Secs";

                }
                else
                {
                    timer.Text = secs + "Secs";
                }
                return true;
            });

           
           
        }


        private async void Geo_Clicked(object sender, EventArgs e)
        {
            try
            {
                Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude),
                        Distance.FromMiles(1)).WithZoom(2));

                    maploading.IsVisible = false;
                    mainmap.IsVisible = true;
                    geo.IsEnabled = true;
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
                await DisplayAlert("Error", "Sorry, Unable to get location! ERROR: " + ex.Message, "OK");
            }
        }

        private async void Accept_Clicked(object sender, EventArgs e)
        {
            reqpopup.IsVisible = false;

            string uid = await _firebaseAuth.GetCurrentUser();

            // Accept request 
            if (uid != null)
            {
                _firebaseDatabase.GetBusy(uid);
                
                TripModel Trip = new TripModel();
                // Set Trip Data
                Trip.Key = "00";

               
                //Accepted, Started, Paused, Ended

                // Set Indivisuals
                Trip.User = Mrequest.User;
                Trip.Driver = Mrequest.Driver;

                // Duration, Status, Time, Date, Distance, Info, Location
                Trip.Date = Mrequest.Date;
                Trip.Location = Mrequest.Driver.Location;

                Trip.Distance = "1";
                Trip.Status = "Accepted";

                Trip.Duration = "0";
                Trip.Type = Mrequest.Type;
                Trip.PauseTime = "0";
                Trip.Amount = "1.5";
                Trip.Payment = "Cash";

               // Start Trip and navigate to ongoing page
               var key = await _firebaseDatabase.StartTrip(Trip.Driver.Key, Trip);

                if (key != null)
                {
                    App.Current.MainPage = new NavigationPage(new TripPage(key));
                }
            }

        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            string uid = await _firebaseAuth.GetCurrentUser();

            if (uid != null)
            {
                reqpopup.IsVisible = false;
                _firebaseDatabase.CancelRequest(uid);
            }
        }



        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetDrivers();
            GetUserLocation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }


    }
}