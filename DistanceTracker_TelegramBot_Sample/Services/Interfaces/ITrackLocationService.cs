using DistanceTracker_TelegramBot_Sample.Data.Models;

namespace DistanceTracker_TelegramBot_Sample.Services.Interfaces
{
    public interface ITrackLocationService : IDisposable
    {
        public IEnumerable<TrackLocationModel> GetTrackLocationsByIMEI(string imei);
        public IEnumerable<TrackLocationModel> GetTrackLocationsByIMEIForADay(string imei);
    }
}
