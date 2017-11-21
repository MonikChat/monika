// Fucking Monikammmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
// mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
// this on MIT btw uwu

using Newtonsoft.Json;

namespace Monika.Discord.Updater
{
    public class GitHubRelease
    {
        [JsonProperty(@"id")]
        public string Id;

        [JsonProperty(@"tag_name")]
        public string TagName => $"{Name}";

        [JsonProperty(@"prerealease")]
        public bool PreRelease;
        
    }
}