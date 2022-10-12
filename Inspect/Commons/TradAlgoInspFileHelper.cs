using Inspect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Commons
{
    public class TradAlgoInspFileHelper
    {
        private const string DefectInfoSectorName = "AAClassify";
        private const string DefectInfoSectorNameF = "AAClassifyF";
        private const string DefectInfoSector = "RCJudge";

        [DllImport("kernel32")]
        public extern static void WritePrivateProfileString(string sector, string key, string value, string filename);
        /// <summary>
        /// 缺陷阈值生成ini文件
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="li"></param>
        public static void WriteTradAlgoInspFile(string pathname, List<TradAlgoInspParamEntity> li)
        {
            foreach (var defect in li)
            {
                int id = defect.DefectId;
                if (defect.SliceId == 1)
                {
                    Dictionary<string, dynamic> data = (Dictionary<string, dynamic>)MakeNameList(defect.Data);
                    foreach (var item in data)
                    {
                        string key = id.ToString() + "_" + item.Key;
                        var val = item.Value;
                        WritePrivateProfileString(DefectInfoSectorName, key, val, pathname);
                    }
                }
                else
                {
                    Dictionary<string, dynamic> data = (Dictionary<string, dynamic>)MakeNameList(defect.Data);
                    foreach (var item in data)
                    {
                        string key = id.ToString() + "_" + item.Key;
                        var val = item.Value;
                        WritePrivateProfileString(DefectInfoSectorNameF, key, val, pathname);
                    }
                }
            }
        }
        /// <summary>
        /// 获取的Recipe基础参数生成ini文件  RCJudge
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="re"></param>
        public static void WriteTradAlgoInspFile(string pathname, RecipeParamEntity re)
        {
            Dictionary<string, dynamic> data = (Dictionary<string, dynamic>)MakeNameList(re);
            foreach (var item in data)
            {
                string key = item.Key;
                var val = item.Value.ToString();
                if (key != "DuanZi") { WritePrivateProfileString(DefectInfoSector, key, val, pathname); }
            }
        }

        private static object MakeNameList(object obj)
        {
            var properties = obj.GetType().GetProperties();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                var val = info.GetValue(obj, null);
                data.Add(info.Name, val);
            }
            return data;
        }

    }
}
