using NasiyeDriver.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage(bool v)
        {

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if (!v)
            {
                DisplayAlert("Connectivity", "Sorry - something did not go through! Please check your network connection and try again", "OK");
            }

        }

       

        private async void Getstarted_Clicked(object sender, EventArgs e)
        {
            NetworkAccess current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                    if (status != PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                        {
                            await DisplayAlert("Need location", "Gunna need that location", "OK");
                        }
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                        status = results[Permission.Location];

                    }

                    if (status == PermissionStatus.Granted)
                    {
                        //Query permission
                        NavigateToApp();
                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        //location denied
                        await DisplayAlert("Need location", "Gunna need that location", "OK");
                    }
                }
                catch (Exception ex)
                {
                    //Something went wrong
                    await DisplayAlert("Location err", "Error accured please ty again", "OK");

                }
            }
            else
            {
                await DisplayAlert("Connectivity", "Sorry - something did not go through! Please check your network connection and try again", "OK");
            }
        }

        private async void NavigateToApp()
        {
            var auth = DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();
                
                if (auth != null)
                {
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    await Navigation.PushAsync(new SigninPage());
                }
            

        }
    }
}