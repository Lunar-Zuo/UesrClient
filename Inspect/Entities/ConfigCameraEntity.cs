using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Inspect.Entities
{
    public class ConfigCameraEntity
    {
        [JsonProperty("id")]
        public int CameraId { get; set; }

        [XmlElement("LocalId")]
        public int CameraId_local { get; set; }
    }
}
