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
    public partial class FeedBackPage : ContentPage
    {
        public readonly IFirebaseAuthInterface _firebaseAuth;
        public readonly IFirebaseDBInterface _firebaseDatabase;


        public FeedBackPage()
        {
            InitializeComponent();
            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            // Message
            FeedbackModel feedback = new FeedbackModel();

            // Get input
            feedback.Key = "00";
            feedback.Subject = subject.SelectedItem.ToString();
            feedback.Body = body.Text;
            feedback.Date = DateTime.Today.Date.ToString();
            feedback.From = "Driver";
            feedback.Status = false;


            _firebaseDatabase.SubmitFeedback(feedback);
            await Navigation.PopAsync();
        }

        private void Body_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(body.Text.Length > 10)
            {
                submit.IsEnabled = true;
            }
            else
            {
                submit.IsEnabled = false;
            }
        }
    }
}