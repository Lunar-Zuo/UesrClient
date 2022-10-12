using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class DefectDensityEntity
    {
        /// <summary>
        /// 编号，如被检出，将作为缺陷号添加到缺陷列表里
        /// </summary>
        [JsonProperty("number")]
        public int Number { get; set; }

        /// <summary>
        /// 需要进行密度检出的缺陷类型
        /// </summary>
        [JsonProperty("defectId")]
        public int DefectId { get; set; }

        /// <summary>
        /// 规则检出区域宽度
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// 规则检出区域高度
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// 规则检出区域内检出标准
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
