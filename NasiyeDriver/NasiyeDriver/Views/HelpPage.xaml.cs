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
    public partial class HelpPage : ContentPage
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            loader.IsVisible = false;
            webview.IsVisible = true;

        }

        private void Webview_Navigating(object sender, WebNavigatingEventArgs e)
        {
            loader.IsVisible = true;
            webview.IsVisible = false;

        }
    }
}