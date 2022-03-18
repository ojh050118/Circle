using Newtonsoft.Json;

namespace Circle.Desktop.Deploy
{
    public class GitHubRelease
    {
#pragma warning disable CS8618

        [JsonProperty(@"id")]
        public int Id;

        [JsonProperty(@"tag_name")]
        public string TagName => $"{Name}";

        [JsonProperty(@"name")]
        public string Name;

        [JsonProperty(@"draft")]
        public bool Draft;

        [JsonProperty(@"prerelease")]
        public bool PreRelease;

        [JsonProperty(@"upload_url")]
        public string UploadUrl;
    }
}
