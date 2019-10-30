using NasiyeDriver.CustomRenderers;
using NasiyeDriver.Models;
using NasiyeDriver.Services;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TripPage : ContentPage
    {

        public readonly IFirebaseAuthInterface _firebaseAuth;
        public readonly IFirebaseDBInterface _firebaseDatabase;
        string tripkey;
        TripModel Trip;
        Stopwatch stopwatch, pstopwatch;
        bool sent = false;
        Plugin.Geolocator.Abstractions.Position Cposition;

        string cdata = null;

        bool sisrunning = false, pisrunning = false;

        string accountSid = "AC034b0fca9b66ae9d444b75c94cdd55f9";
        string authToken = "9eb2133530d8447ee1f74eb383332b3e";
       

        public TripPage(string key)
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

            Trip = new TripModel();

            tripkey = key;

            stopwatch = new Stopwatch();
            pstopwatch = new Stopwatch();

            GetTrip();

        }


        private void ShowMap(User user, Driver driver)
        {
            mainmap.Pins.Clear();

            mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lng), Distance.FromMiles(1)).WithZoom(2));

            var pin = new CustomPin
            {
                Position = new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lng),
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA",
                MarkerId = "Xamarin",
                Icon = "driver"
            };

            mainmap.CustomPins = new List<CustomPin> { pin };


            if (Trip.Status != "Started")
            {
                var upin = new CustomPin
                {
                    Position = new Xamarin.Forms.Maps.Position(user.Location.Lat, user.Location.Lng),
                    Label = "Xamarin San Francisco Office",
                    Address = "394 Pacific Ave, San Francisco CA",
                    MarkerId = "Xamarin",
                    Icon = "user"
                };
                mainmap.Pins.Add(upin);

            }



            mainmap.Pins.Add(pin);


            maploading.IsVisible = false;
            mainmap.IsVisible = true;
        }

        private void GetTrip()
        {
            _firebaseDatabase.GetTrips(tripkey);
            DeviceDisplay.KeepScreenOn = true;

            MessagingCenter.Subscribe<object, string>(this, "tripin", (sender, data) =>
             {
                 if(data != cdata)
                 {
                     cdata = data;

                     Trip = ToTripMOdel(data);

                     if (Trip != null)
                     {
                         ShowTripProgress(Trip);
                     }
                     else
                     {
                         _firebaseDatabase.GetTrips(tripkey);
                     }

                 }
             });
        }

        private TripModel ToTripMOdel(string data)
        {
            if (data != "")
            {
                try
                {
                    TripModel trip = JsonConvert.DeserializeObject<TripModel>(data);

                    return trip;
                }
                catch
                {
                    return null;
                }
            }

            return null;
            
        }

        private async void Canceltrip_Clicked(object sender, EventArgs e)
        {
            // Cancel trip

            if (await DisplayAlert("Trip", "Do you want Cancel trip?", "YES", "NO"))
            {

                if (tripkey != null)
                {
                    DependencyService.Get<IFirebaseDBInterface>().UpdateTripInfo(tripkey, "Canceled");
                }
            }

            // POP Navigation
        }

        private void GetCurrentLocation()
        {
            if (Trip.Key != null)
            {
                    if (Trip.Driver.Location != null)
                    {
                        _firebaseDatabase.UpdateTripLocation(Trip.Key, Trip.Driver.Location);

                    }
            }
        }

        private void Calluser_Clicked(object sender, EventArgs e)
        {
            string phonenumber = Trip.User.Phone;

            try
            {
                PhoneDialer.Open(phonenumber);
            }
            catch (ArgumentNullException anEx)
            {
                DisplayAlert("Error", "Phone number not found", "OK");
            }
            catch (FeatureNotSupportedException ex)
            {
                DisplayAlert("Error", "Sorry this feature is not supported!", "OK");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                DisplayAlert("Error", "Sorry Error accured try calling manually!", "OK");

            }

        }

        private void Ontheway_Clicked(object sender, EventArgs e)
        {
            if (Trip.Key != null)
            {
                GetTrip();

                DependencyService.Get<IFirebaseDBInterface>().UpdateTripInfo(Trip.Key, "Wait");
            }
        }

        private void Arived_Clicked(object sender, EventArgs e)
        {
            if (tripkey != null)
            {
                _firebaseDatabase.UpdateTripInfo(tripkey, "Arrived");
            }
        }

        private async void Startstrip_Clicked(object sender, EventArgs e)
        {
            if (tripkey != null)
            {
                GetTrip();

                if (await DisplayAlert("Trip", "Are your sure you want to Start the trip?", "YES", "NO"))
                {

                    GetCurrentLocation();
                    // 
                    //
                    if (!sisrunning)
                    {
                        sisrunning = true;
                        DistanceUpdate(true);
                        UpdateDuration();
                    }

                    if (pisrunning)
                    {
                        pisrunning = false;
                        UpdatePausedTime(false);
                    }

                    _firebaseDatabase.UpdateTripInfo(tripkey, "Started");


                }
            }
        }

        private async void Pausetrip_Clicked(object sender, EventArgs e)
        {
            if (tripkey != null)
            {
                GetTrip();

                if (await DisplayAlert("Trip", "Are your sure you want to Pause the trip?", "YES", "NO"))
                {

                    if (sisrunning)
                    {
                        sisrunning = false;
                        UpdateDuration(false);
                        DistanceUpdate();
                    }

                    if (!pisrunning)
                    {
                        pisrunning = true;
                        UpdatePausedTime();
                    }

                    

                    _firebaseDatabase.UpdateTripInfo(tripkey, "Paused");
                }
            }
        }

        private async void Endtrip_Clicked(object sender, EventArgs e)
        {
            if (tripkey != null)
            {
                GetTrip();

                if (await DisplayAlert("Trip", "Are your sure you want to End the trip?","YES", "NO"))
                {

                    DependencyService.Get<IFirebaseDBInterface>().UpdateTripInfo(tripkey, "End");
                    StartTrackingAsync(false);
                    // Navigate to rating page

                }

            }
        }

        private async void ShowTripProgress(TripModel tripModel)
        {

            Trip = tripModel;

            ShowMap(tripModel.User, tripModel.Driver);

            status.Text = tripModel.Status;


            if (tripModel.Status == "Accepted")
            {
                // SHow wait
                // SHow action buttons
                iconsbox.IsVisible = true;
                wcon.IsVisible = true;
                loader.IsVisible = false;
                status.Text = "Accepted";

                ShowTripInfo(tripModel);

            }
            else if (tripModel.Status == "Wait")
            {
                // SHow arrived
                ontheway.IsVisible = false;
                arived.IsVisible = true;
                status.Text = "User waiting";

            }
            else if (tripModel.Status == "Arrived")
            {
                arived.IsVisible = false;
                start.IsVisible = true;
                status.Text = "Waiting for User";

            }
            else if (tripModel.Status == "Started")
            {
                status.Text = "Trip On Going";

                if (stopwatch.IsRunning)
                {
                    stopwatch.Restart();
                }
                else
                {
                    stopwatch.Start();
                }

                // hide all

                wcon.IsVisible = false;
                // Show trip
                trpinfocon.IsVisible = true;
                // Show loading
                loader.IsVisible = false;
                // SHow trip info
                pstopwatch.Stop();

                StartTripInfo(tripModel);

                if (!sisrunning)
                {
                    sisrunning = true;
                    DistanceUpdate(true);
                    UpdateDuration();
                }

                if (pisrunning)
                {
                    pisrunning = false;
                    UpdatePausedTime(false);
                }

                pausetrip.IsVisible = true;

                rstart.IsVisible = false;

            }
            else if (tripModel.Status == "Paused")
            {
                if (pstopwatch.IsRunning)
                {
                    pstopwatch.Restart();
                }
                else
                {
                    pstopwatch.Start();
                }

                status.Text = "Trip Paused";

                if (sisrunning)
                {
                    sisrunning = false;
                    UpdateDuration(false);
                    DistanceUpdate();
                }

                if (!pisrunning)
                {
                    pisrunning = true;
                    UpdatePausedTime();
                }


                rstart.IsVisible = true;

                pausetrip.IsVisible = false;

                stopwatch.Stop();

            }
            else if (tripModel.Status == "End")
            {
                DeviceDisplay.KeepScreenOn = true;

                // Stop Trip Updates
                UpdateDuration(false);
                UpdatePausedTime(false);

                GetAmountUpdate(tripModel);

                await StartTrackingAsync(false);

                MessagingCenter.Unsubscribe<object, string>(this, "tripin");

                _firebaseDatabase.RemoveGetTrips(Trip.Key);

                App.Current.MainPage = new NavigationPage(new RatingPage(tripModel));
            }
            else if (tripModel.Status == "Canceled")
            {
                // Stop Trip Updates
                UpdateDuration(false);
                UpdatePausedTime(false);
                DistanceUpdate();

                string uid = await _firebaseAuth.GetCurrentUser();

                StartTrackingAsync(false);

                MessagingCenter.Unsubscribe<object, string>(this, "tripin");

                await DisplayAlert("Trip", "Trip Canceled", "OK");

                _firebaseDatabase.GetOnline(uid);
                _firebaseDatabase.RemoveGetTrips(Trip.Key);


                App.Current.MainPage = new MainPage();
            }
        }

        private void GetAmountUpdate(TripModel tripModel)
        {
            // Calculate Amount
            var client = new TwilioRestClient(accountSid, authToken);

            if (double.Parse(tripModel.Distance ) > 3.0)
            {
                // Get Paused time
                int ptime = int.Parse(tripModel.PauseTime);

                double km = float.Parse(tripModel.Distance) - 3.0;

                double amount = float.Parse(tripModel.Amount);

                double time = ptime * 0.05;


                double dp = km * 0.5;

                double total = Math.Round(dp + time + amount, 2);
                // 
                string body = "Thank you for using Nasiye Taxi Service, The Total Distance is " + Trip.Distance + " KM, and your total is : $" + total + ".";

                //
                if (!sent)
                {
                    sent = true;
                    var message = MessageResource.Create(
                              to: new PhoneNumber(Trip.User.Phone),
                              from: "Nasiye",
                              body: body,
                              client: client);
                } 
              

                _firebaseDatabase.UpdateTripAmount(tripModel.Key, total.ToString());
            }
            else
            {
                string body = "Thank you for using Nasiye Taxi Service, The Total Distance is " + Trip.Distance + " KM, and your total is : $" + Trip.Amount + ".";
                var message = MessageResource.Create(
                               to: new PhoneNumber(Trip.User.Phone),
                               from: "Nasiye",
                               body: body,
                               client: client);
            }
           
        }

        private void StartTripInfo(TripModel tripModel)
        {
            amount.Text = "$ " + double.Parse(tripModel.Amount);
            distance.Text = double.Parse(tripModel.Distance) + " KM";
            duration.Text = double.Parse(tripModel.Duration) + " MIN";

        }

        private async void ShowTripInfo(TripModel tripmodel)
        {
            // SHow wait
            // SHow action buttons
            iconsbox.IsVisible = true;
            wcon.IsVisible = true;
            loader.IsVisible = false;

            userimage.Source = ImageSource.FromUri(new Uri(tripmodel.User.Image));
            username.Text = tripmodel.User.Name;
            userrating.Value = long.Parse(tripmodel.User.Rating);
            userphone.Text = tripmodel.User.Phone;

            infobox.IsVisible = true;

            //// STart tracking
            await StartTrackingAsync(true);
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

                    }
                    else
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

                Plugin.Geolocator.Abstractions.Position position = e.Position;

                if (Cposition != e.Position)
                {
                    Cposition = e.Position;
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude), Distance.FromMiles(1)).WithZoom(2));

                    loader.IsVisible = false;
                    mainmap.IsVisible = true;

                    local.Lat = position.Latitude;
                    local.Lng = position.Longitude;

                   
                    if (Trip.Key != null)
                    {
                        _firebaseDatabase.UpdateTripDriverLocation(Trip.Key, local);

                        if(Trip.Status == "Started")
                        {
                            _firebaseDatabase.UpdateTripLocation(Trip.Key, local);
                        }
                    }
                }


            });
        }




        private void DistanceUpdate(bool w = false)
        {
            Device.StartTimer(new TimeSpan(0, 0, 10), () =>
            {
                UpdateDistance();
                return w;
            });
        }

        private async  void UpdateDistance()
        {
            // Get Trip Location

            Models.Location SLocal = Trip.Location;

            Models.Location local = new Models.Location();

            Xamarin.Essentials.Location startTrip = new Xamarin.Essentials.Location(SLocal.Lat, SLocal.Lng);

            Models.Location NowLocal = new Models.Location();

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);

                var location = await Geolocation.GetLocationAsync(request);


                if (location != null)
                {
                    local.Lat = location.Latitude;
                    local.Lng = location.Longitude;

                    Xamarin.Essentials.Location OnTrip = new Xamarin.Essentials.Location(local.Lat, local.Lng);

                    double distance = Xamarin.Essentials.Location.CalculateDistance(startTrip, OnTrip, DistanceUnits.Kilometers);


                    double oDistance = double.Parse(Trip.Distance);

                    double total = oDistance + distance;

                    _firebaseDatabase.UpdateTripDistance(Trip.Key, Math.Round(total, 2).ToString());

                    GetCurrentLocation();
                }
            }
            catch (Exception ex)
            {
                // Unable to get location

                await DisplayAlert(ex.Source, "ERROR: " + ex.Message, "OK");

            }
        }

        private void UpdateDuration(bool w = true)
        {

            Device.StartTimer(new TimeSpan(0, 1, 0 ), () =>
            {
                int durationt = int.Parse(Trip.Duration);

                durationt++;

                timer.Text = durationt.ToString() +":00";

                _firebaseDatabase.UpdateTripDuration(Trip.Key, durationt.ToString());

                return w;
            });
        }

        private void UpdatePausedTime(bool w = true)
        {
            Device.StartTimer(new TimeSpan(0, 1, 0), () =>
            {
                int durationt = int.Parse(Trip.PauseTime);

                int d = durationt + 1;

                string time = d.ToString() +":00";

                timer.Text = time;

                _firebaseDatabase.UpdatePausedTime(Trip.Key, d.ToString());
                return w;
            });
        }


        
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetTrip();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            cdata = "";

        }
    }
}