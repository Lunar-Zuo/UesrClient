using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class DefectInspectResEntity
    {
        [JsonProperty("count")]
        public int Count { get; set; }


        [JsonProperty("data")]
        public List<DefectDataEntity> Data { get; set; }
    }
}
