using Newtonsoft.Json;

namespace Circle.Desktop.Deploy
{
    public class GitHubAsset
    {
        [JsonProperty("url")]
        public string Url = string.Empty;

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("name")]
        public string Name = string.Empty;
    }
}
