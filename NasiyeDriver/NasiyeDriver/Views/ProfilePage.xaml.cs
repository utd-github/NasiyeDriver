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

        public ProfilePage()
        {
            InitializeComponent();
            _firebaseDatabase = DependencyService.Get<IFirebaseDBInterface>();
            _firebaseAuth = DependencyService.Get<IFirebaseAuthInterface>();

            GetProfile();

        }

        private async void GetProfile()
        {
            string uid = await _firebaseAuth.GetCurrentUser();

            Action<Dictionary<string, UserModel>> onValueEvent = (Dictionary<string, UserModel> user) =>
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

                    if (user != null)
                    {
                        foreach (KeyValuePair<string, UserModel> item in user)
                        {
                           if(item.Key == uid)
                            {
                                SetProfile(item.Value);
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

            _firebaseDatabase.GetProfile("drivers", onValueEvent);
        }
        private void SetProfile(UserModel User)
        {
            ename.Text = User.Name;
            ephone.Text = User.Phone;
            eemail.Text = User.Email;

            userimage.Source = ImageSource.FromUri(new Uri(User.Image));
        }

       
     }
}