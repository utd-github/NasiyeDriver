using NasiyeDriver.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NasiyeDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SigninPage : ContentPage
    {
        public SigninPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }


        private async void Signin_Clicked(object sender, EventArgs e)
        {
            NetworkAccess current = Connectivity.NetworkAccess;

            string email = emailtxt.Text;
            string password = passwordtxt.Text;

            // Show loading
            if (CheckAllFieldsAsync(email, password))
            {
                if (await ValidateFormAsync(email, password))
                {
                    // Check for >> Connectivity
                    if (current == NetworkAccess.Internet)
                    {
                        // Connection to internet is available
                        // Pass it to Service: Auth
                        string auth = await DependencyService.Get<IFirebaseAuthInterface>().LoginWithEmailPasswordAsync(email, password);
                        if (auth == null)
                        {
                            // Navigate
                            App.Current.MainPage = new MainPage();
                        }
                        else
                        {
                            // Error Loging in
                            var message = auth.Split(':');

                            await DisplayAlert("Error", message[1], "OK");

                        }

                    }
                    else
                    {
                        // No COnnectivity Toast
                        await DisplayAlert("Connections",
                            "Sorry did not go through! Please check your network connection and try again",
                            "OK");
                    }
                }
                else
                {
                    // Error Validating
                    await DisplayAlert("Error", "Enter a valid email address and password", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please fill all fields first", "OK");
            }

        }


        private bool CheckAllFieldsAsync(string emailtxt, string passwordtxt)
        {
            if (String.IsNullOrWhiteSpace(emailtxt) && String.IsNullOrWhiteSpace(passwordtxt))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private async Task<bool> ValidateFormAsync(string email, string password)
        {

            if (String.IsNullOrWhiteSpace(email))
            {
                /// TODO: Error email field
                /// 

                await DisplayAlert("Error", "Sorry - Invalid Email, try again", "OK");

                return false;
            }
            else
            {
                if (!CheckIfEmail(email))
                {
                    return false;
                }
                else
                {
                    /// TODO: Hide Error email field
                    /// 

                    if (String.IsNullOrWhiteSpace(password) || password.Length < 8)
                    {
                        /// TODO: Error password field
                        /// 

                        await DisplayAlert("Error", "Sorry - Invalid password, try again", "OK");

                        return false;
                    }
                    else
                    {
                        /// TODO: Hide Error password field
                        /// 
                        return true;
                    }

                }
            }
        }

        private bool CheckIfEmail(string email)
        {

            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private void Signup_Clicked(object sender, EventArgs e)
        {
           
        }
    }
}