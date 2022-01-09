using DistanceTracker_TelegramBot_Sample.Models;

namespace DistanceTracker_TelegramBot_Sample.Extensions
{
    public static class DistanceCalculatorExtension
    {
        public static double GetDistanceBetweenCords(GeoCoordinates pointA, GeoCoordinates pointB)
        {
            double lon1 = ToRadians(pointA.Longitude);
            double lon2 = ToRadians(pointB.Longitude);
            double lat1 = ToRadians(pointA.Latitude);
            double lat2 = ToRadians(pointB.Latitude);

            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;
            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            double r = 6371;

            return (c * r);
        }

        private static double ToRadians(double angleIn10thofaDegree)
        {
            return (angleIn10thofaDegree * Math.PI) / 180;
        }
    }
}
