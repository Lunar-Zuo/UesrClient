using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class TradAlgoInspParamEntity
    {
        [JsonProperty("id")]
        public int DefectId { get; set; }
        [JsonProperty("name")]
        public string DefectName { get; set; }
        [JsonProperty("sliceId")]
        public int SliceId { get; set; }
        [JsonProperty("data")]
        public TradAlgoDefectParamEntity Data { get; set; }
    }
}
