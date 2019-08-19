using NasiyeDriver.Models;
using NasiyeDriver.Services;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TripPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public TripPopUp(object request)
        {
            InitializeComponent();

            GetRequestData(request as RequestModel);
            
        }

        private async void GetRequestData(RequestModel req)
        {
            name.Text = req.Name;
            type.Text = req.Type;
            timer.Text = "60 secs";
        }


        private async void InitiateTrip()
        {
            // Trip Accepted
            //InitiateTrip wait

        }

        private async void Accept_Clicked(object sender, EventArgs e)
        {

             string uid = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            // Accept delete request 
            if (uid != null)
            {
                var x = await DependencyService.Get<IFirebaseDBInterface>().AcceptrReq(uid);
                if (x)
                {
                    // Start trip on-the-way 

                    if (uid != null)
                    {
                        var y = await DependencyService.Get<IFirebaseDBInterface>().AcceptrReq(uid);
                        if (y)
                        {

                        }
                        else
                        {

                        }
                    }
                }
            }

            

        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            string uid = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            if (uid != null)
            {
                var x = await DependencyService.Get<IFirebaseDBInterface>().CancelReuest(uid);
                if (x)
                {
                    // Close the last PopupPage int the PopupStack
                    await Navigation.PopPopupAsync();
                }
                else
                {
                    // Close the last PopupPage int the PopupStack
                    await Navigation.PopPopupAsync();
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainPage.showing = false;

        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            MainPage.showing = false;

            return base.OnBackButtonPressed();

           
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            CancelRequest();
            MainPage.showing = false;

            return base.OnBackgroundClicked();
        }

        private async void CancelRequest()
        {
            string uid = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            if (uid != null)
            {
                var x = await DependencyService.Get<IFirebaseDBInterface>().CancelReuest(uid);
                if (x)
                {
                    // Close the last PopupPage int the PopupStack
                    await Navigation.PopPopupAsync();
                }
                else
                {
                    // Close the last PopupPage int the PopupStack
                    await Navigation.PopPopupAsync();
                }
            }
        }
    }
}