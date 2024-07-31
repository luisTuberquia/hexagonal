using FDLM.Infrastructure.EntrypointsAdapters.SQS.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.EntrypointsAdapters.Resources
{
    public class SumRequestEvent
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("detail-type")]
        public string DetailType { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("resources")]
        public List<object> Resources { get; set; }

        [JsonProperty("detail")]
        public SumRequest Detail { get; set; }
    }
}
