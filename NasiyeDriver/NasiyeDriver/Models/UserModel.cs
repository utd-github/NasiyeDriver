using System;
using System.Collections.Generic;
using System.Text;

namespace NasiyeDriver.Models
{
    public partial class UserModel
    {
        public string Key { get; set; }

        public string Email { get; set; }

        public string Image { get; set; }

        public string Jdate { get; set; }

        public Location Location { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Rating { get; set; }

        public Vehicle Vehicle { get; set; }

        public string Status { get; set; }

        public string Trip { get; set; }

    }

    public partial class Location
    {

        public double Lat { get; set; }


        public double Lng { get; set; }
    }

}
