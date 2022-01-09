using Newtonsoft.Json;

namespace DistanceTracker_TelegramBot_Sample.Controllers.Handlers
{
    public static class ConfigurationHandler
    {
        private static readonly string appSettingsFilePath = "app.settings.json";

        public static string GetConnectionString()
        {
            var json = Deserialize(appSettingsFilePath);

            if (json == null)
                throw new Exception("app.settings.json - an issue occured while processing the file");

            return json["ConnectionString"];
        }

        public static string GetTelegramBotApiKey()
        {
            var json = Deserialize(appSettingsFilePath);

            if (json == null)
                throw new Exception("app.settings.json - an issue occured while processing the file");

            return json["TelegramBotApiKey"];
        }

        private static dynamic Deserialize(string file)
        {
            string? jsonFile = null;
            dynamic? deserializedFile = null;

            try
            {
                jsonFile = File.ReadAllText(file);
                deserializedFile = JsonConvert.DeserializeObject(jsonFile);
            }
            catch
            {
                return null;
            }

            return deserializedFile;
        }
    }
}
