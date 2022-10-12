using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class ImagePreProcessParamsEntity
    {
        [JsonProperty("historyImageScaleX")]
        public double HistoryImageScaleX { get; set; }

        [JsonProperty("historyImageScaleY")]
        public double HistoryImageScaleY { get; set; }

        [JsonProperty("thumbWidth")]
        public int ThumbWidth { get; set; }

        [JsonProperty("thumbHeight")]
        public int ThumbHeight { get; set; }

        [JsonProperty("jpegQuanlity")]
        public int JpegQuanlity { get; set; }
    }
}
