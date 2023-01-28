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
        /// 后台base地址
        /// </summary>
        [XmlElement(ElementName = "commitUrl")]
        public string CommitUrlBase { get; set; }

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
        /// 算法相关
        /// </summary>
        [XmlElement(ElementName = "algorithm")]
        public ConfigAlgorithmEntity Algorithm { get; set; }
        /// <summary>
        /// 心跳周期
        /// </summary>
        [XmlElement(ElementName = "hbDuration")]
        public int HbDuration { get; set; }
        /// <summary>
        /// 存图设置
        /// </summary>
        [XmlElement(ElementName = "image")]
        public ConfigImageSave ImageSave { get; set; }
        /// <summary>
        /// 单设备调试开关
        /// </summary>
        [XmlElement(ElementName = "debug")]
        public ConfigDebug Debug { get; set; }


    }
}
