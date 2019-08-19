using System;
using System.Collections.Generic;
using System.Text;

namespace NasiyeDriver.Models
{
    class RequestModel
    {
        // User 
        public string Name { get; set; }
        public string UserUID { get; set; }

        public string Type { get; set; }

        public Location Location { get; set; }

        public string Status { get; set; }

        public string CreatedAt { get; set; }

    }
}
