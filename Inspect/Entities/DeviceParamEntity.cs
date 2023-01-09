using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    /// <summary>
    /// 基础设置参数
    /// </summary>
    public class DeviceParamEntity
    {

        /// <summary>
        /// NAS原图存储地址
        /// </summary>
        [JsonProperty("historyImageStoragePath")]
        public string HistoryImageStoragePath { get; set; }
        /// <summary>
        /// NAS原图存储天数
        /// </summary>
        [JsonProperty("historyImageStorageDays")]
        public int HistoryImageStorageDays { get; set; }
        /// <summary>
        /// 缓存图存储地址
        /// </summary>
        [JsonProperty("imageCachePath")]
        public string OriginImageStoragePath { get; set; }
        /// <summary>
        /// 缓存图存储天数
        /// </summary>
        [JsonProperty("originImageStorageDays")]
        public int OriginImageStorageDays { get; set; }
        /// <summary>
        /// 数据存储天数
        /// </summary>
        [JsonProperty("dataStorageDays")]
        public int DataStorageDays { get; set; }
    }
}
