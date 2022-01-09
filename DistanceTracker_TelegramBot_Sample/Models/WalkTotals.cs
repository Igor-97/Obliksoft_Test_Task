namespace DistanceTracker_TelegramBot_Sample.Models
{
    public class WalkTotals
    {
        public int TotalWalks { get; set; }
        public double TotalDistance { get; set; }
        public double TotalTime { get; set; }

        public WalkTotals()
        {
            TotalWalks = 0;
            TotalDistance = 0;
            TotalTime = 0;
        }
    }
}
