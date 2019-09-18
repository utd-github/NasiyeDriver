using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NasiyeDriver.CustomRenderers;
using NasiyeDriver.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomeMapRenderer))]


namespace NasiyeDriver.Droid.CustomRenderers
{
    class CustomeMapRenderer : MapRenderer
    {
        List<CustomPin> customPins;

        public CustomeMapRenderer(Context context) : base(context)
        {
        }


        protected override MarkerOptions CreateMarker(Pin pin)
        {
            CustomPin customPin = (CustomPin)pin;

            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            
            if(customPin.Icon == "driver")
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));

            }else
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.userlocal));
            }

            return marker;
        }

    }
}