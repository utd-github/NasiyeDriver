using NasiyeDriver.CustomRenderers;
using NasiyeDriver.Models;
using NasiyeDriver.Services;
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
        TripModel Trip;
        Stopwatch stopwatch = new Stopwatch(), pstopwatch = new Stopwatch();

        Plugin.Geolocator.Abstractions.Position CPosition;

        public TripPage(string key)
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

            Trip = new TripModel();

            GetTrip(key);

        }


        private void ShowMap(User user, Driver driver)
        {

            mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lat), Distance.FromMiles(1)).WithZoom(2));

            var pin = new CustomPin
            {
                Position = new Xamarin.Forms.Maps.Position(driver.Location.Lat, driver.Location.Lng),
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA",
                MarkerId = "Xamarin",
                Icon = "driver"
            };

            var upin = new CustomPin
            {
                Position = new Xamarin.Forms.Maps.Position(user.Location.Lat, user.Location.Lng),
                Label = "Xamarin San Francisco Office",
                Address = "394 Pacific Ave, San Francisco CA",
                MarkerId = "Xamarin",
                Icon = "user"
            };

            mainmap.CustomPins = new List<CustomPin> { pin };

            mainmap.Pins.Add(upin);

            mainmap.Pins.Add(pin);


            maploading.IsVisible = false;
            mainmap.IsVisible = true;
        }

        private void GetTrip(string key)
        {
            Action<Dictionary<string, TripModel>> onValueEvent = (Dictionary<string, TripModel> trips) =>
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

                    if (trips != null)
                    {
                        foreach (KeyValuePair<string, TripModel> item in trips)
                        {
                            if (item.Key == key)
                            {
                                ShowTripProgress(item.Value);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message);
                    throw;
                }
            };

            _firebaseDatabase.GetTrips("trips", onValueEvent);
        }

        private async void Canceltrip_Clicked(object sender, EventArgs e)
        {
            // Cancel trip
            string auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            if (auth != null)
            {
                DependencyService.Get<IFirebaseDBInterface>().UpdateTripInfo(auth, "Canceled");
            }
            // POP Navigation
            await Navigation.PopAsync();
        }

        private void GetCurrentLocation()
        {
            if (Trip.Key != null)
            {
                // Get Location
                Models.Location location = new Models.Location();

                location.Lat = CPosition.Latitude;
                location.Lng = CPosition.Longitude;

                _firebaseDatabase.UpdateTripLocation(Trip.Key, location);

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
                DependencyService.Get<IFirebaseDBInterface>().UpdateTripInfo(Trip.Key, "Wait");
            }
        }

        private void Arived_Clicked(object sender, EventArgs e)
        {
            if (Trip.Key != null)
            {
                _firebaseDatabase.UpdateTripInfo(Trip.Key, "Arrived");
            }
        }

        private async void Startstrip_Clicked(object sender, EventArgs e)
        {
            if (Trip.Key != null)
            {
                if (await DisplayAlert("Trip", "Are your sure you want to Start the trip?", "YES", "NO"))
                {

                    GetCurrentLocation();
                    // 
                    //
                    _firebaseDatabase.UpdateTripInfo(Trip.Key, "Started");

                    DistanceUpdate(true);

                }
            }
        }

        private async void Pausetrip_Clicked(object sender, EventArgs e)
        {
            if (Trip.Key != null)
            {
                if (await DisplayAlert("Trip", "Are your sure you want to Pause the trip?", "YES", "NO"))
                {
                    _firebaseDatabase.UpdateTripInfo(Trip.Key, "Paused");
                }
            }
        }

        private async void Endtrip_Clicked(object sender, EventArgs e)
        {
            if (Trip.Key != null)
            {

                if(await DisplayAlert("Trip", "Are your sure you want to End the trip?","YES", "NO"))
                {
                    DependencyService.Get<IFirebaseDBInterface>().UpdateTripInfo(Trip.Key, "End");
                    // Navigate to rating page

                }

            }
        }




        private void ShowTripProgress(TripModel tripModel)
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
                status.Text = "On the Way";

                ShowTripInfo(tripModel);

            }
            else if (tripModel.Status == "Wait")
            {
                // SHow arrived
                ontheway.IsVisible = false;
                arived.IsVisible = true;
                status.Text = "Waiting";

            }
            else if (tripModel.Status == "Arrived")
            {
                arived.IsVisible = false;
                start.IsVisible = true;
                status.Text = "Waiting for Customer";

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

                UpdatePausedTime(false);

                pausetrip.IsVisible = true;

                rstart.IsVisible = false;

                UpdateDuration();

                UpdateAmount(true);

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

                UpdateDuration(false);

                rstart.IsVisible = true;

                pausetrip.IsVisible = false;

                stopwatch.Stop();

                UpdatePausedTime();
            }
            else if (tripModel.Status == "End")
            {
                // Stop Trip Updates
                UpdateDuration(false);

                App.Current.MainPage = new RatingPage(tripModel);
            }
        }

     
        private void StartTripInfo(TripModel tripModel)
        {
            // Start Counting
            StartTiming(tripModel);
            // Start distance calculation
            StartDistance(tripModel);
            //Start calculating amount
            StartAmount(tripModel);

        }

        private void StartAmount(TripModel tripModel)
        {
            amount.Text = "$ " + double.Parse(tripModel.Amount);
        }

        private void StartDistance(TripModel tripModel)
        {
            distance.Text = double.Parse(tripModel.Distance) + " KM";
        }

        private void StartTiming(TripModel tripModel)
        {
            duration.Text = double.Parse(tripModel.Duration) + " min";
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

                var position = e.Position;

                if(CPosition != e.Position)
                {
                    CPosition = e.Position;
                    mainmap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude), Distance.FromMiles(1)).WithZoom(2));

                    loader.IsVisible = false;
                    mainmap.IsVisible = true;

                    local.Lat = position.Latitude;
                    local.Lng = position.Longitude;

                    string auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

                    if (auth != null)
                    {
                        DependencyService.Get<IFirebaseDBInterface>().UpdateTripDriverLocation(auth, local);
                    }
                }


            });
        }

        // Update Trip Info



        private void DistanceUpdate(bool v)
        {
            // Get Trip Location

            Models.Location SLocal = Trip.Location;

            Xamarin.Essentials.Location startTrip = new Xamarin.Essentials.Location(SLocal.Lat, SLocal.Lng);



            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Models.Location DLocal = Trip.Driver.Location;
                Xamarin.Essentials.Location OnTrip = new Xamarin.Essentials.Location(DLocal.Lat, DLocal.Lng);


                double distance = Xamarin.Essentials.Location.CalculateDistance(startTrip, OnTrip, DistanceUnits.Kilometers);

                if (distance > 1.0)
                {
                    double oDistance = double.Parse(Trip.Distance);
                    double total = oDistance + distance;
                    _firebaseDatabase.UpdateTripDistance(Trip.Key, total.ToString());
                }


                return v;
            });



        }

        private void UpdateDuration(bool w = true)
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                TimeSpan ts = stopwatch.Elapsed;
                int durationt = int.Parse(Trip.Duration);
                int d = durationt + ts.Minutes;

                string time = d + ":" + ts.Seconds;

                timer.Text = time;
                _firebaseDatabase.UpdateTripDuration(Trip.Key, d.ToString());
                return w;
            });
        }

        private void UpdatePausedTime(bool w = true)
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                TimeSpan ts = stopwatch.Elapsed;
                int durationt = int.Parse(Trip.PauseTime);
                int d = durationt + ts.Minutes;

                string time = d +":"+ ts.Seconds;

                timer.Text = time;

                _firebaseDatabase.UpdatePausedTime(Trip.Key, d.ToString());
                return w;
            });
        }

        private void UpdateAmount(bool v)
        {

            // Get Current Amount

            double cAmount = double.Parse(Trip.Amount);


           // Get Distance 

            double cDistance = double.Parse(Trip.Distance);

            // GEt Duration

            double cDuration = double.Parse(Trip.Duration);

            // Get Pasued Time

            int cPauseTime = int.Parse(Trip.PauseTime);


            // Check if DIstance is Greater than 1.5 km

            if(cDistance > 1.5)
            {
                // Calculate
                double ccDitance = cDistance - 1.5;

                // Ditance amount 

                double cDAMount = ccDitance * 0.5;

                if(cPauseTime != 0)
                {
                    double ccPauseTime = cPauseTime * 0.05;

                    cDAMount += ccPauseTime;
                }

                // UPdate Amount 

                _firebaseDatabase.UpdateTripAmount(Trip.Key, cDAMount.ToString());
                
            }

        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}