using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspect.Entities
{
    public class ConfigDebug
    {
        /// <summary>
        /// 单机调试开关
        /// </summary>
        [XmlElement(ElementName = "enable")]
        public bool DebugEnable { get; set; }

        [XmlElement(ElementName = "path")]
        public bool LocalHistoryPath { get; set; }
    }
}
