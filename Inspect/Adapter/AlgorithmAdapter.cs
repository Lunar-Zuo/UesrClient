using CwCommon.Entities;
using CwCommon.Utils;
using Inspect.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Adapter
{
    public class AlgorithmAdapter
    {
        public static AlgorithmAdapter Instance = new AlgorithmAdapter();
        private object objlock = new object();
        private List<string> PointList = new List<string>();
        public void AddAlogServ(string port)
        {
            PointList.Add(port);
        }

        private string GetBaseUrl(string port)
        {
            //return "http://127.0.0.1:" + port;
            return "http://localhost:" + port;
        }

        private string GetPoint()
        {
            lock (objlock)
            {
                if (PointList.Count == 0) throw new Exception("算法服务数量不够");
                string port = PointList[0];
                PointList.RemoveAt(0);
                return port;
            }
        }

        private void SetPoint(string port)
        {
            lock (objlock)
            {
                PointList.Add(port);
            }
        }
        /// <summary>
        /// recipe算法参数传入
        /// </summary>
        /// <param name="recipeId"></param>
        /// <param name="data"></param>
        public async void InspectRecipeRequest(int recipeId, string data)
        {
            string port = "";
            try
            {
                port = GetPoint();
                string url = GetBaseUrl(port) + "/api/algorithm/inspect_recipe_params";
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("recipeId", recipeId);
                body.Add("data", data);

                LoggerHelper.Debug(JsonConvert.SerializeObject(body));
                await HttpHelper.PostJsonAsync(url, JsonConvert.SerializeObject(body));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!string.IsNullOrEmpty(port)) SetPoint(port);
            }
        }
        /// <summary>
        /// 算法检测请求
        /// </summary>
        /// <param name="pathname">NG图片存储路径（D:/T9_save_defect/NG/年月日/panelID/）</param>
        /// <param name="panelId">产品码</param>
        /// <param name="filename">存储文件名（全路径）</param>
        /// <param name="slice">层编号</param>
        /// <param name="edge">边编号</param>
        /// <param name="idx">照片编号</param>
        /// <param name="env_param">环境参数</param>
        /// <param name="insp_param">检测参数</param>
        /// <returns></returns>
        public async Task<ResponseEntity> InspectRequest(string panelCode, int recipeId, string imageFile, string ngPath, int cameraId, int imageId)
        {
            string port = "";
            try
            {
                port = GetPoint();
                string url = GetBaseUrl(port) + "/api/algorithm/defect_inspect";
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("panelCode", panelCode);
                body.Add("recipeId", recipeId);
                body.Add("imageFileName", imageFile);
                body.Add("ngPath", ngPath);
                body.Add("cameraId", cameraId);
                body.Add("imageId", imageId);
                //LoggerHelper.Debug(url);
                LoggerHelper.Debug(JsonConvert.SerializeObject(body));
                string res = await HttpHelper.PostJsonAsync(url, JsonConvert.SerializeObject(body));
                LoggerHelper.Debug("算法返回res：" + res);
                string res1 = RanksConvert(res, recipeId);
                LoggerHelper.Debug("数据处理返回res1：" + res1);

                return JsonConvert.DeserializeObject<ResponseEntity>(res);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!string.IsNullOrEmpty(port)) SetPoint(port);
            }
        }

        /// <summary>
        /// 行列数据转换
        /// </summary>
        /// <param name="res">算法结果</param>
        /// <param name="RecipeParam">后台数据，需要行列与端子朝向</param>
        /// <returns></returns>
        public string RanksConvert(string res, int recipeId)
        {
            try
            {
                Dictionary<int, DefectDataEntity> DefectData = new Dictionary<int, DefectDataEntity>();

                ResponseEntity Response = JsonConvert.DeserializeObject<ResponseEntity>(res);
                //LoggerHelper.Debug("反序列化res，得到Response：" + JsonConvert.SerializeObject(Response));

                DefectInspectResEntity de = JsonConvert.DeserializeObject<DefectInspectResEntity>(JsonConvert.SerializeObject(Response.Data));

                //LoggerHelper.Debug("处理第二步（反序列化Response.Data，得到de）：" + JsonConvert.SerializeObject(de).ToString());
                //LoggerHelper.Debug("处理第二步(得到具体缺陷数据de.Data)：" + JsonConvert.SerializeObject(de.Data));
                if (de.Count != 0)
                {
                    //获取像素转换距离因子值
                    DeviceParamExtEntity DeviceParamExt = CommitAdapter.Instance.GetDeviceParamExt();
                    LoggerHelper.Debug("获取后台像素转换距离因子值：" + JsonConvert.SerializeObject(DeviceParamExt));

                    //LoggerHelper.Debug("后台基础参数数据：" + JsonConvert.DeserializeObject<ResponseEntity>(RecipeParam.ToString()));
                    int rowTotal = 1;
                    int columnTotal = 1;
                    int duanzi = 1;
                    int count = 0;
                    //行列转换
                    foreach (DefectDataEntity detefectData in de.Data)
                    {
                        //LoggerHelper.Debug("处理第三步，获取单独data：" + JsonConvert.SerializeObject(detefectData));
                        if (detefectData.row == 0 && detefectData.column == 0)
                        { }
                        else
                        {
                            if (duanzi == 1)//端子朝下料端,行对调
                            {
                                detefectData.row = rowTotal + 1 - detefectData.row;
                            }
                            else if (duanzi == 2) //端子朝PLC操作屏一侧，行列均对调
                            {
                                detefectData.row = rowTotal + 1 - detefectData.row;
                                detefectData.column = columnTotal + 1 - detefectData.column;
                            }
                            else//段子朝上料端
                            {

                            }
                        }
                        DefectData.Add(count, detefectData);
                        //LoggerHelper.Debug("处理第五步：" + JsonConvert.SerializeObject(DefectData.Values));
                        count++;
                    }
                    //LoggerHelper.Debug("转换完成data：" + JsonConvert.SerializeObject(DefectData.Values));

                    DefectInspectResEntity data = new DefectInspectResEntity();
                    data.Count = DefectData.Count;
                    data.Data = JsonConvert.DeserializeObject<List<DefectDataEntity>>(JsonConvert.SerializeObject(DefectData.Values));

                    //LoggerHelper.Debug("处理第六步：" + JsonConvert.SerializeObject(data));

                    Response.Data = JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(data));

                    LoggerHelper.Debug("处理最后一步(返回res)：" + JsonConvert.SerializeObject(Response).ToString());

                    return JsonConvert.SerializeObject(Response).ToString();
                }
                else
                {
                    return res;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug("数据处理异常," + ex.Message);
                return res;
            }
        }

        #region 调用本地算法（本地调试使用）
        /// <summary>
        /// 算法检测请求（本地调试时使用）
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="ngPath"></param>
        /// <param name="setupFile"></param>
        /// <param name="panelCode"></param>
        /// <param name="slice"></param>
        /// <param name="cameraId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public DefectInspectResEntity TradInspectRequest(string imageFile, string ngPath, string setupFile, string panelCode, int slice, int cameraId, int imageId)
        {
            lock (objlock)
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();//开始时间

                byte[] buffer = new byte[256];
                int len = buffer.Length;
                LoggerHelper.Info(string.Format("算法图片路径：{0} ,算法文件路径：{1},ngPath:{2},panelCode:{3},slice:{4},cameraId:{5},imageId:{6}", imageFile, setupFile, ngPath, panelCode, slice, cameraId, imageId));
                var ret = DefectInspect(Encoding.Default.GetBytes(imageFile),
                    Encoding.Default.GetBytes(ngPath),
                    Encoding.Default.GetBytes(setupFile),
                    Encoding.Default.GetBytes(panelCode),
                    slice, cameraId, imageId, ref buffer[0], ref len);

                DefectInspectResEntity data = new DefectInspectResEntity()
                {
                    Count = ret
                };

                if (ret > 0)
                {
                    string name = System.Text.Encoding.Default.GetString(buffer, 0, len);

                    var defectData = System.IO.File.ReadAllText(name);

                    LoggerHelper.Info("算法返回缺陷数据： " + defectData);
                    data.Data = JsonConvert.DeserializeObject<List<DefectDataEntity>>(defectData);
                }
                watch.Stop();//结束时间
                TimeSpan timespan = watch.Elapsed;//相差时间
                LoggerHelper.Info("算法处理时长： " + timespan.TotalMilliseconds + "ms");

                return data;
            }
        }

        [DllImport(@"C:\AlgoLibs\TradAlgoDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int DefectInspect(byte[] imageFile, byte[] ngPath, byte[] setupFile, byte[] panelCode,
            int sliceId, int cameraId, int imageId, ref byte defectFile, ref int size);
        #endregion
    }
}
