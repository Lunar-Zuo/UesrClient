using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspect.Entities
{
    public class ConfigImageSave
    {
        [XmlElement(ElementName = "localPath")]
        public string LocalImagePath { get; set; }

        [XmlElement(ElementName = "historyPath")]
        public string HistoryImagePath { get; set; }

        [XmlElement(ElementName = "historyUserLocal")]
        public bool UserLocalHistoryPath { get; set; }
    }
}
