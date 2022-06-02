using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwiftFitFileDownloaderConsoleApp
{
    internal class Activity
    {
        [JsonProperty("id_str")]
        public string ActivityId { get; set; }
        [JsonProperty("name")]
        public string ActivityName { get; set; }
        [JsonProperty("startDate")]
        public string ActivityStartDate { get; set; }
        [JsonProperty("fitFileBucket")]
        public string ActivityFitFileBucket { get; set; }
        [JsonProperty("fitFileKey")]
        public string ActivityFitFileKey { get; set; }
    }
}
