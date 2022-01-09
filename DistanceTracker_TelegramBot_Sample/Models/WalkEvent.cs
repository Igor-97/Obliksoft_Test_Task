namespace DistanceTracker_TelegramBot_Sample.Models
{
    public class WalkEvent
    {
        // Total distance should be represented in kilometers
        // Total time should be represented in minutes
        public DateTime StartedAt { get; }
        public DateTime EndedAt { get; private set; }
        public double TotalDistance { get; private set; }
        public double TotalTime { get; private set; }

        public WalkEvent(DateTime start)
        {
            StartedAt = start;
            EndedAt = start;
            TotalDistance = 0;
            TotalTime = 0;
        }

        public void SetDistance(double distance)
        {
            if (distance < 0)
                throw new ArgumentException("distance should be greater than zero");

            TotalDistance = distance;
        }

        public void SetEndTime(DateTime endTime)
        {
            if (endTime < StartedAt)
                throw new ArgumentException("endTime should be greater than StartedAt");

            EndedAt = endTime;
        }

        public void SetTotalTime(double time)
        {
            if (time < 0)
                throw new ArgumentException("time should be greater than zero");

            TotalTime = time;
        }
    }
}
