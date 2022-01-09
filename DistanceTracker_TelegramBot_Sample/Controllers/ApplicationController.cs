using DistanceTracker_TelegramBot_Sample.Controllers.Handlers;
using DistanceTracker_TelegramBot_Sample.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DistanceTracker_TelegramBot_Sample.Controllers
{
    public class ApplicationController
    {
        private TelegramBotClient? _telegramBot;
        private string? apiKey;

        private string? connectionString;

        public Dictionary<long, string> botLastReply;

        public async Task RunAsync()
        {
            StateLogToConsole("Application is getting started...");
            StateLogToConsole("Setting up configuration parameters...");

            // Getting configuration data:
            if (GetApiKey() != true)
            {
                StateLogToConsole("An error occured!");

                return;
            }

            if (GetConnectionString() != true)
            {
                StateLogToConsole("An error occured!");

                return;
            }

            StateLogToConsole("Setting up the bot for telegram...");

            // Telegram bot related section:
            _telegramBot = new TelegramBotClient(apiKey);

            using var cts = new CancellationTokenSource();

            var isApiValid = _telegramBot.TestApiAsync(cts.Token).GetAwaiter();
            bool keyValidation = false;

            // Check out on API key validation:
            // Will return false if within 5 seconds isApiValid isn't completed.
            if (isApiValid.IsCompleted != true)
            {
                int tries = 0;
                int totalTries = 5;
                int waitingTimer = 1000;

                while (tries != totalTries)
                {
                    Thread.Sleep(waitingTimer);

                    if (isApiValid.IsCompleted)
                    {
                        keyValidation = isApiValid.GetResult();
                        tries = totalTries;
                    }
                    else
                    {
                        tries++;
                    }
                }
            }

            if (keyValidation != true)
            {
                StateLogToConsole("API key isn't valid.");
                StateLogToConsole("An error occured!");

                return;
            }

            botLastReply = new Dictionary<long, string>();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            _telegramBot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var bot = await _telegramBot.GetMeAsync();

            StateLogToConsole($"{bot.Username} is up and ready.");

            await Task.Delay(-1);

            cts.Cancel();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;

            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            StateLogToConsole($"Message received in {chatId} chat.");

            List<WalkEvent> walkEvents;
            WalkTotals walkTotals;

            // Check out if the chat with this user has been active:
            var keyExists = botLastReply.ContainsKey(chatId);

            if (keyExists)
            {
                if (messageText == "TOP 10 Longest Walks")
                {
                    var userData = DataHandler.GetTrackDataByIMEI(connectionString, botLastReply[chatId], 
                        out walkEvents, out walkTotals);

                    walkEvents.Sort((x, y) => x.TotalTime.CompareTo(y.TotalTime));
                    walkEvents.Reverse();

                    string messageBuilder = "The longest walks (TOP 10):";

                    for (int i = 0; i < walkEvents.Count; i++)
                    {
                        if (i == 10)
                            break;

                        messageBuilder += "\n---"
                            + $"\n#{i + 1}:"
                            + $"\n** Distance: {Math.Round(walkEvents[i].TotalDistance, 1)} km"
                            + $"\n** Time: {Math.Round(walkEvents[i].TotalTime, 1)} mins";
                    }

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageBuilder,
                        cancellationToken: cancellationToken);

                    StateLogToConsole($"Response sent to {chatId} chat.");

                    return;
                }

                if (messageText == "Today's track")
                {
                    var userData = DataHandler.GetTrackDataByIMEI(connectionString, botLastReply[chatId], 
                        out walkEvents, out walkTotals, true);

                    string? messageBuilder = null;

                    if (walkTotals.TotalWalks == 0)
                    {
                        messageBuilder = "Ah, there is no data for the current day.";
                    }
                    else
                    {
                        messageBuilder = "Today's track:"
                            + $"\n* Walks: {walkTotals.TotalWalks}"
                            + $"\n* Distance: {Math.Round(walkTotals.TotalDistance, 1)} km"
                            + $"\n* Time: {Math.Round(walkTotals.TotalTime, 1)} mins";
                    }

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageBuilder,
                        cancellationToken: cancellationToken);

                    StateLogToConsole($"Response sent to {chatId} chat.");

                    return;
                }

                if (messageText == "Return")
                {
                    var userData = DataHandler.GetTrackDataByIMEI(connectionString, botLastReply[chatId], 
                        out walkEvents, out walkTotals, true);

                    string messageBuilder = "Ah, well, we are back here!"
                        + "\n* Enter your IMEI to continue check out on your walks tracking.";

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageBuilder,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                    botLastReply.Remove(chatId);

                    StateLogToConsole($"Response sent to {chatId} chat.");

                    return;
                }
            }

            // Original way on interaction with user:
            long userImei;
            bool isImei = long.TryParse(messageText, out userImei);

            if (isImei && messageText.Length == 15)
            {
                var userData = DataHandler.GetTrackDataByIMEI(connectionString, userImei.ToString(), 
                    out walkEvents, out walkTotals);

                if (userData)
                {
                    string messageBuilder = "Your totals:"
                        + $"\n* Walks: {walkTotals.TotalWalks}"
                        + $"\n* Distance: {Math.Round(walkTotals.TotalDistance, 1)} km"
                        + $"\n* Time: {Math.Round(walkTotals.TotalTime, 1)} mins";

                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] 
                    {
                        new KeyboardButton[] {"TOP 10 Longest Walks", "Today's track", "Return"},
                    })
                    {
                        ResizeKeyboard = true
                    };

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageBuilder,
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);

                    if (keyExists != true)
                        botLastReply.Add(chatId, userImei.ToString());

                    StateLogToConsole($"Response sent to {chatId} chat.");
                }
                else
                {
                    string messageBuilder = "Ah, something went wrong:"
                        + "\n* Your IMEI isn't correct;"
                        + "\n* Your IMEI doesn't exist in our databse;"
                        + "\n* Your IMEI doesn't have any tracks yet.";

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageBuilder,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                    if (keyExists)
                        botLastReply.Remove(chatId);

                    StateLogToConsole($"Response sent to {chatId} chat.");
                }
            }
            else
            {
                string messageBuilder = "Please, enter your IMEI correctly:"
                        + "\n* It should consist only numbers;"
                        + "\n* It should be 15 characters long.";

                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageBuilder,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                if (keyExists)
                    botLastReply.Remove(chatId);

                StateLogToConsole($"Response sent to {chatId} chat.");
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);

            return Task.CompletedTask;
        }

        private bool GetApiKey()
        {
            string? telegramBotApiKey = null;

            try
            {
                telegramBotApiKey = ConfigurationHandler.GetTelegramBotApiKey();
            }
            catch (Exception ex)
            {
                StateLogToConsole(ex.ToString());

                return false;
            }

            if (telegramBotApiKey != null)
            {
                apiKey = telegramBotApiKey;
            }
            else
            {
                StateLogToConsole("Required configuration parameters weren't found in settings file.");

                return false;
            }

            return true;
        }

        private bool GetConnectionString()
        {
            string? connString = null;

            try
            {
                connString = ConfigurationHandler.GetConnectionString();
            }
            catch (Exception ex)
            {
                StateLogToConsole(ex.ToString());

                return false;
            }

            if (connString != null)
            {
                connectionString = connString;
            }
            else
            {
                StateLogToConsole("Required configuration parameters weren't found in settings file.");

                return false;
            }

            return true;
        }

        private void StateLogToConsole(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]" +
                $" {message}");
        }
    }
}
