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
        [XmlElement("id")]
        public int CameraId_local { get; set; }
    }
}
