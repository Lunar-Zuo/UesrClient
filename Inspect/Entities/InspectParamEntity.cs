using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Entities
{
    public class InspectParamEntity
    {
        public double RadioVer { get; set; }
        public int MeasPosId { get; set; }
        public string ImageBasePath { get; set; }
        public string ImageTempPath { get; set; }

        public int JpegQulity { get; set; }
    }
}
