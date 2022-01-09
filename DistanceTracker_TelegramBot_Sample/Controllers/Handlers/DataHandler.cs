using DistanceTracker_TelegramBot_Sample.Data;
using DistanceTracker_TelegramBot_Sample.Data.Models;
using DistanceTracker_TelegramBot_Sample.Extensions;
using DistanceTracker_TelegramBot_Sample.Models;
using DistanceTracker_TelegramBot_Sample.Services;

namespace DistanceTracker_TelegramBot_Sample.Controllers.Handlers
{
    public static class DataHandler
    {
        public static bool GetTrackDataByIMEI(string connString, string imei, 
            out List<WalkEvent> walkEvents, out WalkTotals walkTotals, 
            bool isCurrentDayOnly = false)
        {
            using (var service = new TrackLocationService(
                new ApplicationContext(connString)))
            {
                IEnumerable<TrackLocationModel> dataOutput;

                if (isCurrentDayOnly != false)
                {
                    dataOutput = service
                        .GetTrackLocationsByIMEIForADay(imei)
                        .OrderBy(e => e.Date_Track);

                    if (dataOutput.Any())
                    {
                        walkEvents = SortOutToWalkEvents(dataOutput.ToList(), out walkTotals);

                        return true;
                    }

                    walkEvents = new List<WalkEvent>();
                    walkTotals = new WalkTotals();

                    return false;
                }

                dataOutput = service
                    .GetTrackLocationsByIMEI(imei)
                    .OrderBy(e => e.Date_Track);

                if (dataOutput.Any())
                {
                    walkEvents = SortOutToWalkEvents(dataOutput.ToList(), out walkTotals);

                    return true;
                }

                walkEvents = new List<WalkEvent>();
                walkTotals = new WalkTotals();

                return false;
            }
        }

        private static List<WalkEvent> SortOutToWalkEvents(List<TrackLocationModel> dataOutput, out WalkTotals walkTotals)
        {
            var walksTrack = new List<WalkEvent>();

            WalkEvent? walkEventTmp = null;
            GeoCoordinates? cordsTmp = null;

            walkTotals = new WalkTotals();

            for (int i = 0; i < dataOutput.Count; i++)
            {
                if (i == 0)
                {
                    walkEventTmp = new WalkEvent(dataOutput[i].Date_Track);
                    cordsTmp = new GeoCoordinates(dataOutput[i].Latitude, dataOutput[i].Longitude);

                    continue;
                }

                if (walkEventTmp != null && cordsTmp != null)
                {
                    if ((dataOutput[i].Date_Track - walkEventTmp.EndedAt).TotalMinutes >= 31)
                    {
                        if (walkEventTmp.TotalDistance > 0)
                        {
                            walkEventTmp.SetTotalTime((walkEventTmp.EndedAt - walkEventTmp.StartedAt).TotalMinutes);

                            walksTrack.Add(walkEventTmp);

                            walkTotals.TotalDistance += walkEventTmp.TotalDistance;
                            walkTotals.TotalTime += (walkEventTmp.EndedAt - walkEventTmp.StartedAt).TotalMinutes;
                        }

                        walkEventTmp = new WalkEvent(dataOutput[i].Date_Track);
                        cordsTmp = new GeoCoordinates(dataOutput[i].Latitude, dataOutput[i].Longitude);

                        continue;
                    }

                    walkEventTmp.SetDistance(walkEventTmp.TotalDistance +
                        DistanceCalculatorExtension.GetDistanceBetweenCords(
                            cordsTmp,
                            new GeoCoordinates(
                                dataOutput[i].Latitude,
                                dataOutput[i].Longitude)));

                    walkEventTmp.SetEndTime(dataOutput[i].Date_Track);

                    cordsTmp = new GeoCoordinates(dataOutput[i].Latitude, dataOutput[i].Longitude);

                    if (i == dataOutput.Count - 1)
                    {
                        walkEventTmp.SetTotalTime((walkEventTmp.EndedAt - walkEventTmp.StartedAt).TotalMinutes);

                        walksTrack.Add(walkEventTmp);

                        walkTotals.TotalDistance += walkEventTmp.TotalDistance;
                        walkTotals.TotalTime += (walkEventTmp.EndedAt - walkEventTmp.StartedAt).TotalMinutes;
                    }
                }

                walkTotals.TotalWalks = walksTrack.Count;
            }

            return walksTrack;
        }
    }
}
