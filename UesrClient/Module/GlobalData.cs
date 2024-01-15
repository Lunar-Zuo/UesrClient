using Inspect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Module
{
    public class GlobalData
    {
        public static Dictionary<int, string> SliceNameList = new Dictionary<int, string>();


        /// <summary>
        /// 检测参数
        /// </summary>
        public static InspectParamEntity InspectParam = new InspectParamEntity();




        public static string GetSliceName(int id)
        {
            return SliceNameList[id];
        }




     
    }
}
