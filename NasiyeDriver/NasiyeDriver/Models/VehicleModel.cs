using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NasiyeDriver.Models
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
        public string ImageUri { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public string Type { get; set; }
        public string CreatedAt { get; set; }

    }
}
