using DistanceTracker_TelegramBot_Sample.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DistanceTracker_TelegramBot_Sample.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<TrackLocationModel>? TrackLocation { get; set; }

        protected readonly string connectionString;

        public ApplicationContext(string connString)
        {
            connectionString = connString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(connectionString);
        }
    }
}
