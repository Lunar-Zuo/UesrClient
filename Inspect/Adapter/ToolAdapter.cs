using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Adapter
{
    public class ToolAdapter
    {
        public static ToolAdapter Instance = new ToolAdapter();
        /// <summary>
        /// 定期删除文件夹
        /// </summary>
        /// <param name="fileDirect">路径</param>
        /// <param name="saveDay">存储天数</param>
        public void DeleteFile(string fileDirect, int saveDay)
        {
            DateTime nowTime = DateTime.Now;
            DirectoryInfo root = new DirectoryInfo(fileDirect);
            DirectoryInfo[] dics = root.GetDirectories();//获取文件夹

            FileAttributes attr = File.GetAttributes(fileDirect);
            if (attr == FileAttributes.Directory)//判断是不是文件夹
            {
                foreach (DirectoryInfo file in dics)//遍历文件夹
                {
                    TimeSpan t = nowTime - file.CreationTime;  //当前时间  减去 文件创建时间
                    int day = t.Days;
                    if (day > saveDay)   //保存的时间 ；  单位：天
                    {
                        Directory.Delete(file.FullName, true);  //删除超过时间的文件夹
                    }
                }
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="saveDay">存储天数</param>
        public void CleanFile(string path, int saveDay)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime < DateTime.Now.AddDays(-saveDay))
                {
                    file.Delete();
                }
            }
        }
    }
}
