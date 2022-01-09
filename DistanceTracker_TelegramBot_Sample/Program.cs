using DistanceTracker_TelegramBot_Sample.Controllers;

namespace DistanceTracker_TelegramBot_Sample
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var app = new ApplicationController();
            await app.RunAsync();
        }
    }
}