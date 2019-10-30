using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Firebase;
using Android;
using Android.Support.V4.Content;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using System.Threading.Tasks;
using System.IO;
using Plugin.LocalNotifications;
using Android.Content.Res;

namespace NasiyeDriver.Droid
{
    [Activity(Label = "NasiyeDriver", 
        Icon = "@mipmap/icon", 
        Theme = "@style/Splashscreen", 
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }
        public static Context context;
        public static FirebaseApp app;

        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);

            Instance = this;

            context = this;

            app = FirebaseApp.InitializeApp(context);


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.icon;
            
            LoadApplication(new App());

        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override Android.Content.Res.Resources Resources
        {
            get
            {
                var config = new Configuration();

                config.SetToDefaults();

                return CreateConfigurationContext(config).Resources;
            }
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }
}