using NasiyeDriver.Models;
using NasiyeDriver.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            GetProfile();
            MessagingCenter.Subscribe<object, string>(this, "profile", (sender, data) =>
            {
                SetProfile(datatoModel(data));
            });
        }

        private async void GetProfile()
        {
            string auth = await DependencyService.Get<IFirebaseAuthInterface>().GetCurrentUser();

            if (auth != null)
            {
                DependencyService.Get<IFirebaseDBInterface>().GetUserProfile(auth);
            }
        }

        private void SetProfile(UserModel User)
        {
            ename.Text = User.Name;
            ephone.Text = User.Phone;
            eemail.Text = User.Email;

            userimage.Source = new UriImageSource
            {
                Uri = User.Image
            };

            

        }


        private UserModel datatoModel(string data)
        {
            UserModel user = null;

            try
            {
                user = JsonConvert.DeserializeObject<UserModel>(data);

                return user;
            }
            catch
            {
                return null;
            }
        }

        private void Signout_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFirebaseAuthInterface>().Singout();
            App.Current.MainPage = new NavigationPage(new WelcomePage(true));
        }
}
}