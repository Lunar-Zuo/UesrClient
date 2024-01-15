using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspect.Entities
{
    [XmlRoot("configure")]
    public class ConfigEntity
    {
        /// <summary>
        /// 程序ID
        /// </summary>
        [XmlElement(ElementName = "programId")]
        public int ProgramId { get; set; }

        /// <summary>
        /// Websocket服务监听地址
        /// </summary>
        [XmlElement(ElementName = "wsUrl")]
        public string WsServerUrl { get; set; }
        /// <summary>
        /// 相机相关设置
        /// </summary>
        [XmlArray("cameras")]
        [XmlArrayItem("camera")]
        public List<ConfigCameraEntity> Cameras { get; set; }
        /// <summary>
        /// 心跳周期
        /// </summary>
        [XmlElement(ElementName = "hbDuration")]
        public int HbDuration { get; set; }



    }
}
