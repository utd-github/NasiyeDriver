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
        public readonly IFirebaseAuthInterface _firebaseAuth;

        public readonly IFirebaseDBInterface _firebaseDatabase;

        UserModel UserModel;
        public ProfilePage()
        {
            InitializeComponent();
            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();
           

            MessagingCenter.Subscribe<object, object>(this, "profile", (sender, data) =>
            {
                UserModel = data as UserModel;
                SetProfile(UserModel);
            });
        }

       
        private void SetProfile(UserModel User)
        {
            ename.Text = User.Name;
            ephone.Text = User.Phone;
            eemail.Text = User.Email;
            userimage.Source = ImageSource.FromUri(new Uri(User.Image));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object, object>(this, "get", "profile");
        }
    }
}