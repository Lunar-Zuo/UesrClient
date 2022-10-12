using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class CameraRecipeParamsEntity
    {
        [JsonProperty("setupFileName")]
        public string SetupFileName { get; set; }

        [JsonProperty("validImageCount")]
        public int ValidImageCount { get; set; }

        [JsonProperty("gain")]
        public string Gain { get; set; }

        [JsonProperty("exposureTime")]
        public string ExposureTime { get; set; }

    }
}
