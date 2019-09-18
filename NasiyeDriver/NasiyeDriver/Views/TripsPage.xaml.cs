using NasiyeDriver.Models;
using NasiyeDriver.Services;
using System;
using System.Collections.Generic;
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

        public TripsPage()
        {
            InitializeComponent();


            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();


        }



        private void GetTrips()
        {
            List<TripModel> tripsCollection = new List<TripModel>();

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
                        info.IsVisible = false;

                        if (trips.Count != 0 && trips.Count != tripsCollection.Count)
                        {

                            tripsCollection.Clear();

                            foreach (KeyValuePair<string, TripModel> item in trips)
                            {
                                if (tripsCollection.All(d => d.Key != item.Value.Key))
                                {
                                    tripsCollection.Add(new TripModel
                                    {
                                        Location = item.Value.Location,
                                        Amount = "$ " + item.Value.Amount,
                                        Date = item.Value.Date,
                                        Distance = item.Value.Distance + "KM",
                                        Driver = item.Value.Driver,
                                        Duration = item.Value.Duration + " mins",
                                        Info = item.Value.Info,
                                        Key = item.Value.Key,
                                        Payment = item.Value.Payment,
                                        Rating = item.Value.Rating,
                                        Status = item.Value.Status,
                                        Time = item.Value.Time,
                                        Type = item.Value.Type,
                                        User = item.Value.User
                                    });
                                }
                            }

                            tripsList.IsRefreshing = false;
                            tripsList.ItemsSource = tripsCollection;
                        }
                    }
                    else
                    {
                        tripsCollection.Clear();
                        tripsList.IsRefreshing = false;
                        tripsList.IsVisible = false;
                        info.IsVisible = true;
                    }

                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("---> error GetDataFromFirebase " + ex.Message + " , Source" + ex.Source);
                    throw;
                }
            };

            _firebaseDatabase.GetSavedTrips("trips", onValueEvent);
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
            _firebaseDatabase.RemoveGetSavedTrips("trips");
        }
    }
}