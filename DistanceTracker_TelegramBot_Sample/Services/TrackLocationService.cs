using DistanceTracker_TelegramBot_Sample.Data;
using DistanceTracker_TelegramBot_Sample.Data.Models;
using DistanceTracker_TelegramBot_Sample.Services.Interfaces;

namespace DistanceTracker_TelegramBot_Sample.Services
{
    public class TrackLocationService : ITrackLocationService
    {
        private ApplicationContext _context;
        private bool disposed = false;

        public TrackLocationService(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<TrackLocationModel> GetTrackLocationsByIMEI(string imei)
        {
            return _context.TrackLocation.Where(e => e.IMEI == imei);
        }

        public IEnumerable<TrackLocationModel> GetTrackLocationsByIMEIForADay(string imei)
        {
            return _context.TrackLocation.Where(e => e.IMEI == imei && e.Date_Track.Date == DateTime.Now.Date);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
