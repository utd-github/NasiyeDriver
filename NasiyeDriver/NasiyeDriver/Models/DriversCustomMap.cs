using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace NasiyeDriver.Models
{
    class DriversCustomMap : Map
    {
        internal List<DriverCustomPin> CustomPins { get; set; }
    }
}
