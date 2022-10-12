using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class InspectStartEntity
    {
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }
        [JsonProperty("panelId")]
        public string PanelId { get; set; }
        [JsonProperty("onlyVcr")]
        public int OnlyVcr { get; set; }
        [JsonProperty("recipe")]
        public int Recipe { get; set; }
        [JsonProperty("mmg")]
        public int Mmg { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
