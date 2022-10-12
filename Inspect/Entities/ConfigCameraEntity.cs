using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspect.Entities
{
    public class ConfigCameraEntity
    {
        [XmlAttribute("id")]
        public int CameraId { get; set; }
    }
}
