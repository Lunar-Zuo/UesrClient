using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class CameraParamsEntity
    {
        [JsonProperty("number")]
        public int CameraId { get; set; }
        [JsonProperty("name")]
        public int CameraName { get; set; }

        [JsonProperty("slice")]
        public int SliceId { get; set; }

    }
}
