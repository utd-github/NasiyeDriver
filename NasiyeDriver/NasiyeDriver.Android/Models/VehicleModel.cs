using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Newtonsoft.Json;

namespace NasiyeDriver.Droid.Models
{
    public partial class VehicleModel
    {
        public VehicleModel(string key, Vehicle vehicle)
        {
            Key = key;
            Vehicle = vehicle;
        }
        public string Key { get; set; }
        [JsonProperty("Vehicle", NullValueHandling = NullValueHandling.Ignore)]
        public Vehicle Vehicle { get; set; }
    }

    public partial class Vehicle
    {
        public Vehicle()
        {
        }

        public Vehicle(string key, string imageUri, string model, string plate, string type, string createdAt)
        {
            Key = key;
            ImageUri = imageUri;
            Model = model;
            Plate = plate;
            Type = type;
            CreatedAt = createdAt;
        }

        public string Key { get; set; }

        [JsonProperty("ImageUri", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageUri { get; set; }

        [JsonProperty("Model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("Plate", NullValueHandling = NullValueHandling.Ignore)]
        public string Plate { get; set; }

        [JsonProperty("Type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("CreatedAt", NullValueHandling = NullValueHandling.Ignore)]
        public string CreatedAt { get; set; }

    }
}