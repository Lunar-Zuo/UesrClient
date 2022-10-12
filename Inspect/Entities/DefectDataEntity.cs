using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class DefectDataEntity
    {
        /// <summary>
        /// 缺陷名称
        /// </summary>
        [JsonProperty("defectId")]
        public int defectId { get; set; }

        /// <summary>
        /// 坐标x
        /// </summary>
        [JsonProperty("x")]
        public int x { get; set; }

        /// <summary>
        /// 坐标y
        /// </summary>
        [JsonProperty("y")]
        public int y { get; set; }

        /// <summary>
        /// 缺陷框的宽度
        /// </summary>
        [JsonProperty("w")]
        public int w { get; set; }

        /// <summary>
        /// 缺陷框的高度
        /// </summary>
        [JsonProperty("h")]
        public int h { get; set; }

        /// <summary>
        /// 缺陷宽度
        /// </summary>
        [JsonProperty("w_d")]
        public int w_d { get; set; }

        /// <summary>
        /// 缺陷高度
        /// </summary>
        [JsonProperty("h_d")]
        public int h_d { get; set; }

        /// <summary>
        /// 行
        /// </summary>
        [JsonProperty("row")]
        public int row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [JsonProperty("column")]
        public int column { get; set; }

        /// <summary>
        /// 缺陷小图名称
        /// </summary>
        [JsonProperty("filename")]
        public string filename { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        [JsonProperty("result")]
        public int result { get; set; }

    }
}
