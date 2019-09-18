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
    public partial class RatingPage : ContentPage
    {
        public readonly IFirebaseAuthInterface _firebaseAuth;
        public readonly IFirebaseDBInterface _firebaseDatabase;

        TripModel Trip;


        public RatingPage(object trip)
        {
            InitializeComponent();

            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();

            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

            Trip = new TripModel();
            Trip = trip as TripModel;


            ShowTripInfo(trip as TripModel);

        }

        private void ShowTripInfo(TripModel tripModel)
        {
           
            User user = tripModel.User;

            userImage.Source = ImageSource.FromUri(new Uri(user.Image));

            username.Text = user.Name;
            price.Text = "$ "  +tripModel.Amount;
            duration.Text = tripModel.Duration;
            distance.Text = tripModel.Distance;
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            // Submit Comment and stars
            // Get Rating
            // Get Any comments
            submit.IsEnabled = false;
            double rated = rating.Value;

            string comment = body.Text;

            _firebaseDatabase.SubmitTripRating(Trip.Key, rated, comment);

            App.Current.MainPage = new MainPage();

        }
    }
}