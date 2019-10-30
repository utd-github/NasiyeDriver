using NasiyeDriver.Models;
using NasiyeDriver.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TripsPage : ContentPage
    {
        public readonly IFirebaseAuthInterface _firebaseAuth;
        public readonly IFirebaseDBInterface _firebaseDatabase;
        ObservableCollection<TripModel> Trips;
        string cdata;
        public TripsPage()
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();
        }



        private async void GetTrips()
        {
            string uid = await _firebaseAuth.GetCurrentUser();

          

            _firebaseDatabase.GetSavedTrips(uid);

            MessagingCenter.Subscribe<object, string>(this, "savedtrips", (sender, data) =>
            {
               
                    if (data == null)
                    {
                        ShowList(null);

                    }
                    else
                    {
                        ShowList(ToTripCOllection(data));
                    }
              
                
            });
        }

        private void ShowList(ObservableCollection<TripModel> trips)
        {
            if (trips != null)
            {
               
                tripsList.ItemsSource = trips;
                info.IsVisible = false;
            }
            else
            {
                tripsList.IsVisible = false;
                info.IsVisible = true;
            }
        }

        private ObservableCollection<TripModel> ToTripCOllection(string data)
        {

            if (data != "")
            {
                

                
            try
            {
                    ObservableCollection<TripModel> trips = new ObservableCollection<TripModel>();


                    Dictionary<string, object> Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                    TripModel trip = new TripModel();

                    foreach (KeyValuePair<string, object> item in Data.ToList())

                    {
                        TripModel ctrip = JsonConvert.DeserializeObject<TripModel>(item.Value.ToString());
                        trips.Add(new TripModel
                        {
                            Key = ctrip.Key,
                            Amount = "$ " + ctrip.Amount,
                            Date = ctrip.Date,
                            Distance = ctrip.Distance,
                            Driver = ctrip.Driver,
                            DriverKey = ctrip.DriverKey,
                            Duration = ctrip.Duration + " Mins",
                            Info = ctrip.Info,
                            Location = ctrip.Location,
                            PauseTime = ctrip.PauseTime,
                            Payment = ctrip.Payment,
                            Rating = ctrip.Rating,
                            Status = ctrip.Status,
                            Time = ctrip.Time,
                            Type = ctrip.Type,
                            User = ctrip.User,
                            UserKey = ctrip.UserKey,
                        });
                    }


                    Trips = trips;

                return trips;

            }
            catch (Exception ex)
            {
                DisplayAlert("Trips", "Error: " + ex.Message, "OK");

                return null;
            }

            }
            return null;
        }

        private void TripsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            tripsList.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            GetTrips();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            cdata = "";
        }
    }
}