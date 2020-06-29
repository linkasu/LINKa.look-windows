using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF
{
    public class SettingsFile
    {
        [JsonProperty("keys")]
        public Dictionary<string, string> Keys { get; set; }

        [JsonProperty("isHasGazeEnabled")]
        public bool? IsHasGazeEnabled { get; set; }

        [JsonProperty("isAnimatedClickEnabled")]
        public bool? IsAnimatedClickEnabled { get; set; }

        [JsonProperty("clickDelay")]
        public double? ClickDelay { get; set; }
    }
}
