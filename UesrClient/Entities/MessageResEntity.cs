using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UesrClient.Entities
{
    public class MessageResEntity
    {
        [JsonProperty("errCode")]
        public int errCode { get; set; }
    }
}
