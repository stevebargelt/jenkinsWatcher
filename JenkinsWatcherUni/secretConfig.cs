using Windows.Data.Json;

namespace JenkinsWatcherUni
{
    public class secretConfig
    {
        public string jenkinsUsername { get; set; }
        public string jenkinsApiKey { get; set; }
        public string jenkinsUrl { get; set; }


        public secretConfig()
        {
            jenkinsUsername = "";
            jenkinsApiKey = "";
            jenkinsUrl = "";
        }

        public secretConfig(string jsonString) : this()
        {
            JsonObject jsonObject = JsonObject.Parse(jsonString);

            jenkinsUsername = jsonObject.GetNamedString("jenkinsUsername", "");
            jenkinsApiKey = jsonObject.GetNamedString("jenkinsApiKey", "");
            jenkinsUrl = jsonObject.GetNamedString("jenkinsUrl", "");
        }
    }
}
