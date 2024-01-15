using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UesrClient.Entities
{
    public class UpdateParamsEntity
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
