using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class RecipeParamEntity
    {   
        //需要跟界面设置参数名统一
        /// <summary>
        /// 端子朝向
        /// </summary>
        [JsonProperty("DuanZi")]
        public int DuanZi { get; set; }

        /// <summary>
        /// 图像中，屏幕的列数
        /// </summary>
        [JsonProperty("column")]
        public int columnTotal { get; set; }
        /// <summary>
        /// 图像中，屏幕的行数
        /// </summary>
        [JsonProperty("row")]
        public int rowTotal { get; set; }

        /// <summary>
        /// 单个屏幕的宽度信息
        /// </summary>
        [JsonProperty("WidthPanel")]
        public int WidthPanel { get; set; }
        /// <summary>
        /// 单个屏幕的高度信息
        /// </summary>
        [JsonProperty("heightPanel")]
        public int heightPanel { get; set; }
        /// <summary>
        /// 起始的行数
        /// </summary>
        [JsonProperty("Rnumber")]
        public int Rnumber { get; set; }
        /// <summary>
        /// 起始的列数
        /// </summary>
        [JsonProperty("Cnumber")]
        public int Cnumber { get; set; }
        /// <summary>
        /// 是否混排，1是，0否
        /// </summary>
        [JsonProperty("hunpai")]
        public int hunpai { get; set; }
        /// <summary>
        /// 针对混排,屏幕1的宽度信息
        /// </summary>
        [JsonProperty("WidthPanel1")]
        public int WidthPanel1 { get; set; }
        /// <summary>
        /// 针对混排,屏幕1的高度信息
        /// </summary>
        [JsonProperty("heightPanel1")]
        public int heightPanel1 { get; set; }
        /// <summary>
        /// 针对混排，屏幕的列数
        /// </summary>
        [JsonProperty("columnTotal1")]
        public int columnTotal1 { get; set; }
        /// <summary>
        /// 针对混排,屏幕的行数
        /// </summary>
        [JsonProperty("rowTotal1")]
        public int rowTotal1 { get; set; }
        /// <summary>
        /// 针对混排,屏幕1的起始行数
        /// </summary>
        [JsonProperty("Rnumber1")]
        public int Rnumber1 { get; set; }
        /// <summary>
        /// 针对混排,屏幕1的起始列数
        /// </summary>
        [JsonProperty("Cnumber1")]
        public int Cnumber1 { get; set; }
    }
}
