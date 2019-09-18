using System;
using System.Collections.Generic;
using System.Text;

namespace NasiyeDriver.Models
{
    class RequestModel
    {
        // User 
        public string Key { get; set; }

        public User User { get; set; }

        public Driver Driver { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string Date { get; set; }
    }

    public partial class User
    {

        public string Key { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Phone { get; set; }

        public string Rating { get; set; }

        public Location Location { get; set; }

    }
    
}
