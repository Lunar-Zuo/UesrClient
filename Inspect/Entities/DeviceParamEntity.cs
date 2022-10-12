using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class DeviceParamEntity
    {

        /// <summary>
        /// 压缩图存储地址
        /// </summary>
        [JsonProperty("historyImageStoragePath")]
        public string HistoryImageStoragePath { get; set; }
        /// <summary>
        /// 压缩图存储天数
        /// </summary>
        [JsonProperty("historyImageStorageDays")]
        public int HistoryImageStorageDays { get; set; }
        /// <summary>
        /// 原图存储地址
        /// </summary>
        [JsonProperty("originImageStoragePath")]
        public string OriginImageStoragePath { get; set; }
        /// <summary>
        /// 原图存储天数
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
