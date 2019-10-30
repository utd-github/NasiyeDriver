using NasiyeDriver.Models;
using NasiyeDriver.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedBackPage : ContentPage
    {
        public FeedBackPage()
        {
            InitializeComponent();
            // Tap Gesture initialization
            var phonetapped = new TapGestureRecognizer();
            phonetapped.Tapped += Phonetapped_Tapped;
            var emailtapped = new TapGestureRecognizer();
            emailtapped.Tapped += Emailtapped_Tapped;
            var websitetapped = new TapGestureRecognizer();
            websitetapped.Tapped += Websitetapped_Tapped;
            var maptapped = new TapGestureRecognizer();

            // Add gestures to labels
            phonetxt.GestureRecognizers.Add(phonetapped);
            emailtxt.GestureRecognizers.Add(emailtapped);
            websitetxt.GestureRecognizers.Add(websitetapped);

        }


        // Info tapped
        private void Emailtapped_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("mailto:info@nasiyeservices.com"));
        }

        private void Phonetapped_Tapped(object sender, EventArgs e)
        {
            var number = "+252660000000";
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
                Console.WriteLine("Error Argument Null Exception: ", anEx);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device..
                Console.WriteLine("Error Feature Not Supported Exception: ", ex);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Console.WriteLine("Error Exception: ", ex);
            }
        }

        private async void Websitetapped_Tapped(object sender, EventArgs e)
        {
            var uri = "http://www.nasiyeservices.com/";

            await Browser.OpenAsync(uri, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = Color.Aqua,
                PreferredControlColor = Color.Violet
            });
        }


    }
}