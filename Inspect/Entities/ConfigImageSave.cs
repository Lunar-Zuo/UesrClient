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
        /// <summary>
        /// 历史数据存储根目录
        /// </summary>
        [XmlElement(ElementName = "historyPath")]
        public string HistoryImagePath { get; set; }
        /// <summary>
        /// 历史数据存储目录是否使用配置文件
        /// </summary>
        [XmlElement(ElementName = "historyUserLocal")]
        public bool UserLocalHistoryPath { get; set; }
    }
}
