using Newtonsoft.Json;

namespace Circle.Desktop.Deploy
{
    public class GitHubObject
    {
#pragma warning disable CS8618

        [JsonProperty(@"id")]
        public int Id;

        [JsonProperty(@"name")]
        public string Name;
    }
}
