using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class TradAlgoDefectParamEntity
    {
        public string AreaMin { get; set; }
        public string AreaMax { get; set; }
        public string MeanMin { get; set; }
        public string MeanMax { get; set; }
        public string DeviaMin { get; set; }
        public string DeviaMax { get; set; }
        public string WidthMin { get; set; }
        public string WidthMax { get; set; }
        public string HeightMin { get; set; }
        public string HeightMax { get; set; }
        public string RectanMin { get; set; }
        public string RectanMax { get; set; }
        public string CirculMin { get; set; }
        public string CirculMax { get; set; }
        public string Distance { get; set; }
        public string Number { get; set; }

    }
}
