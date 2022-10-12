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

        [XmlArray("cameras")]
        [XmlArrayItem("camera")]
        public List<ConfigCameraEntity> Cameras { get; set; }

        [XmlElement(ElementName = "algorithm")]
        public ConfigAlgorithmEntity Algorithm { get; set; }

        [XmlElement(ElementName = "hbDuration")]
        public int HbDuration { get; set; }

        [XmlElement(ElementName = "image")]
        public ConfigImageSave ImageSave { get; set; }

        [XmlElement(ElementName = "recipeFilePath")]
        public string RecipeFilePath { get; set; }
        [XmlElement(ElementName = "recipeFileName")]
        public string RecipeFileName { get; set; }
    }
}
