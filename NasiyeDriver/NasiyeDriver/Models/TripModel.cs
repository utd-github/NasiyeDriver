using System;
using System.Collections.Generic;
using System.Text;

namespace NasiyeDriver.Models
{
    class TripModel
    {
        // Trip Key
        public string Key { get; set; }
        public string UserKey { get; set; }
        public string DriverKey { get; set; }

        public User User { get; set; }

        public Driver Driver { get; set; }

        public string Type { get; set; }

        public Location Location { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }


        public string Status { get; set; }

        public string Info { get; set; }

        public Rating Rating { get; set; }

        public string Duration { get; set; }

        public string Distance { get; set; }

        public string Amount { get; set; }

        public string Payment { get; set; }

        public string PauseTime { get; set; }

    }

    public partial class Driver
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public Uri Image { get; set; }

        public string Phone { get; set; }

        public string Rating { get; set; }

        public string Trips { get; set; }


        public Vehicle Vehicle { get; set; }

        public Location Location { get; set; }
    }

    public partial class Rating
    {
        public string User { get; set; }

        public string Driver { get; set; }
    }
}
