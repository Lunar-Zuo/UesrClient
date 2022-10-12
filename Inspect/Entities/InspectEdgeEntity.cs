using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class InspectEdgeEntity
    {

        [JsonProperty("inspOrder")]
        public int Order { get; set; }

        [JsonProperty("sliceId")]
        public int SliceId { get; set; }

        [JsonProperty("edgeId")]
        public int EdgeId { get; set; }

        [JsonProperty("imageValid")]
        public int ImageValid { get; set; }

        [JsonProperty("imageCount")]
        public int ImageCount { get; set; }
    }
}
