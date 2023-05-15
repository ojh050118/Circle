using Newtonsoft.Json;

namespace Circle.Desktop.Deploy
{
    public class GitHubObject
    {
        [JsonProperty(@"id")]
        public int Id;

        [JsonProperty(@"name")]
        public string Name;
    }
}
