using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspect.Entities
{
    public class ConfigAlgorithmEntity
    {
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "recipeFilePath")]
        public string RecipeFilePath { get; set; }

        [XmlElement(ElementName = "recipeFileName")]
        public string RecipeFileName { get; set; }

        [XmlElement(ElementName = "enable")]
        public bool Enable { get; set; }

        [XmlArray("servs")]
        [XmlArrayItem("item")]
        public List<ConfigAlgoServEntity> AlgoServs { get; set; }
    }
}
