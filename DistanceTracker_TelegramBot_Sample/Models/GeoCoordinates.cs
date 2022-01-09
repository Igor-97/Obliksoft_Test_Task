using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceTracker_TelegramBot_Sample.Models
{
    public class GeoCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoCoordinates(decimal latitude, decimal longitude)
        {
            Latitude = (double)latitude;
            Longitude = (double)longitude;
        }
    }
}
