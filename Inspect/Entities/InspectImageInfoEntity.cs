using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class InspectImageInfoEntity
    {
        public int Recipe { get; set; }
        public string SerialNumber { get; set; }
        public string PanelId { get; set; }

        public int ImageId { get; set; }

        public string RelativePath { get; set; }

        public string HistoryFilePath { get; set; }
        public string HistoryFileName { get; set; }

        public string LocalFilePath { get; set; }
        public string LocalFileName { get; set; }

        public int ValidImageCount { get; set; }

        public int ErrCount { get; set; }

    }
}
