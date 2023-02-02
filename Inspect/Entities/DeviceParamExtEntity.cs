using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    /// <summary>
    /// 高级设置参数
    /// </summary>
    public class DeviceParamExtEntity
    {
        /// <summary>
        /// 水平缩放参数
        /// </summary>
        [JsonProperty("scaleFactorVer")]
        public double ScaleFactorVer { get; set; }
        /// <summary>
        /// 垂直缩放参数
        /// </summary>
        [JsonProperty("scaleFactorHor")]
        public double ScaleFactorHor { get; set; }
        /// <summary>
        /// 缓存图片存储目录
        /// </summary>
        [JsonProperty("imageCachePath")]
        public string ImageCachePath { get; set; }
        /// <summary>
        /// 像素->距离 转换因子
        /// </summary>
        [JsonProperty("pixelDistaFactor")]
        public double PixelDistaFactor { get; set; }

    }
}
