﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class CameraRecipeParamsEntity
    {
        [JsonProperty("filename")]
        public string FileName { get; set; }
        /// <summary>
        /// 相机拍摄图片数量
        /// </summary>
        [JsonProperty("validImageCount")]
        public int ValidImageCount { get; set; }
        /// <summary>
        /// 增益
        /// </summary>
        [JsonProperty("gain")]
        public string Gain { get; set; }
        /// <summary>
        /// 曝光时间
        /// </summary>
        [JsonProperty("exposureTime")]
        public string ExposureTime { get; set; }
        /// <summary>
        /// 启用标识，1-启用，0-不启用
        /// </summary>
        [JsonProperty("usable")]
        public int Usable { get; set; }

    }
}
