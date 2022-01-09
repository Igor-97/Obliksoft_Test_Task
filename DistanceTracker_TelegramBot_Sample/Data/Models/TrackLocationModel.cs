namespace DistanceTracker_TelegramBot_Sample.Data.Models
{
    public class TrackLocationModel
    {
        public int ID { get; set; }
        public string? IMEI { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime DateEvent { get; set; }
        public DateTime Date_Track { get; set; }
        public int TypeSource { get; set; }
    }
}
