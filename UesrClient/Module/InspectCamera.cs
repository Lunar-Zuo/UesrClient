using CwCommon.Entities;
using CwCommon.Utils;
using Inspect.Adapter;
using Inspect.Commons;
using Inspect.Entities;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inspect.Module
{
    public class InspectCamera
    {
        public delegate void CameraStatusChangeDelegate(int id, int status);
        public event CameraStatusChangeDelegate CameraStatusChangeHandler;
        public delegate void CameraInspectCompleteDelegate(string sn, string panelId, int cameraId, int errCode);
        public event CameraInspectCompleteDelegate CameraInspectCompleteHandler;

        //private DalsaCameraAdapter device = null;
        private CameraParamsEntity cameraParam = null;
        private CameraRecipeParamsEntity cameraRecipeParam = null;
        private DeviceParamExtEntity deviceParamExt = null;

        //针对当前相机是否完成处理
        private Dictionary<string, CountUtils> PanelProcess = new Dictionary<string, CountUtils>();

        private object _PanelProcessLock = new object();

        private bool bCameraOn = false;

        public bool IsInspecting { get; set; }
        public string SliceName { get; set; }
        public int CameraId { get; set; }
        /// <summary>
        /// 历史数据存储目录是否使用配置文件
        /// </summary>
        public bool HistoryUserLocal = false;
        /// <summary>
        /// 算法启用开关
        /// </summary>
        public bool AlgorithmEnable = false;
        public string HistoryPathBase = @"E:\Images";
        //public string LocalPathBase = @"E:\LocalImage";
        public string AlgorithmRecipeFileName = @"C:\BJCW\AlgoConfig\recipe.ini";

        //数据更新，配方切换，相机初始化流程保护
        private object _cameraLock = new object();

        private string _SerialNumber;
        private string _PanelId;
        private int _Recipe;
        private int _ImageId;
        private string _RelativePath;

        /// <summary>
        /// 初始化，加载后台参数，开启相机线程
        /// </summary>
        /// <param name="recipe"></param>
        public void Create(int recipe)
        {
            if (bCameraOn) return;

            _Recipe = recipe;
            cameraStatus = 0;

            IsInspecting = false;

            LoadParams(recipe);

            int CameraDevice = GetCameraDevice(CameraId);
            device = new DalsaCameraAdapter();
            device.CameraConnectionStatusHandler += OnCameraConnectionStatus;
            device.ConfigFileName = cameraRecipeParam.FileName;
            //device.OpenDevice();
            device.GigeOpenDevice(CameraDevice);
            if (cameraRecipeParam.Usable == 1)
            {
                device.SetFeatureValueDouble(DalsaCameraAdapter.FeatureNameGain, double.Parse(cameraRecipeParam.Gain));
                device.SetFeatureValueDouble(DalsaCameraAdapter.FeatureNameExposureTime, double.Parse(cameraRecipeParam.ExposureTime));
                device.CameraCapuredDataHandler += OnCameraCapturedData;
                device.StartCapture();
                bCameraOn = true;
            }
            else
            {
                LoggerHelper.Info($"相机{CameraId} 启用标识为{cameraRecipeParam.Usable}。 ");
            }
        }
        public int GetCameraDevice(int CameraId)
        {
            ConfigEntity configEntity = LoadConfig();
            int cameraDevice = 0;
            foreach (ConfigCameraEntity ConfigCamera in configEntity.Cameras)
            {
                if (CameraId == ConfigCamera.CameraId_local)
                {
                    cameraDevice = ConfigCamera.CameraDevice;
                }
            }
            return cameraDevice;
        }

        public void Destroy()
        {
            if (bCameraOn)
            {
                device.StopCapture();
                device.CloseDevice();
                device.CameraCapuredDataHandler -= OnCameraCapturedData;
                device = null;
                bCameraOn = false;
            }
        }
        /// <summary>
        /// 初始化，后台拿数据，开启相机线程
        /// </summary>
        /// <param name="recipe"></param>
        public void Reload(int recipe)
        {
            lock (_cameraLock)
            {
                Destroy();
                Create(recipe);
            }
        }

        public void UpdateParams(int recipe)
        {
            _Recipe = recipe;
            device.StopCapture();
            LoadParams(recipe);
            device.StartCapture();
        }
        /// <summary>
        /// 加载后台相机参数、算法参数、图像预处理
        /// </summary>
        /// <param name="recipe"></param>
        public void LoadParams(int recipe)
        {
            LoadDeviceParamSExt();
            LoadCameraRecipeParams(recipe);
            //LoadRecipeAlgorithmParams(recipe);
            //暂时不用
            //LoadImagePreProcessParams(recipe);
            //LoadAlgoParams();

        }

        private void LoadCameraRecipeParams(int recipe)
        {
            cameraRecipeParam = CommitAdapter.Instance.GetCameraRecipeParams(CameraId, recipe);
        }
        /// <summary>
        /// 后台加载算法传统参数数据，再传给算法
        /// </summary>
        public void LoadRecipeAlgorithmParams(int recipe)
        {
            try
            {
                Dictionary<int, TradAlgoDefectParamEntity> AlgoDefectParam = new Dictionary<int, TradAlgoDefectParamEntity>();
                var res = CommitAdapter.Instance.GetTradAlgoRecipeParams(recipe);

                List<TradAlgoInspParamEntity> data = JsonConvert.DeserializeObject<List<TradAlgoInspParamEntity>>(res.ToString());
                //string pathname = MainLogic.Config.Algorithm.RecipeFilePath + MainLogic.Config.Algorithm.RecipeFileName;
                //TradAlgoInspFileHelper.WriteTradAlgoInspFile(pathname, data);

                //后台获取的传统算法处理
                foreach (TradAlgoInspParamEntity AlgoInspParam in data)
                {
                    AlgoDefectParam.Add(AlgoInspParam.DefectId, AlgoInspParam.Data);
                }
                string AlgoDefectParams = JsonConvert.SerializeObject(AlgoDefectParam.Values).ToString();

                //调用算法接口将传统算法参数传给算法
                AlgorithmAdapter.Instance.InspectRecipeRequest(recipe, AlgoDefectParams);
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn("算法传统参数传入失败，" + ex.Message);
            }
        }
        /// <summary>
        /// 高级参数获取
        /// </summary>
        /// <param name="recipe"></param>
        private void LoadDeviceParamSExt()
        {
            deviceParamExt = CommitAdapter.Instance.GetDeviceParamExt();
        }
        private void LoadAlgoParams()
        {
            //cameraParam = CommitAdapter.Instance.GetCameraParams(CameraId);
            //AlgorithmRecipeFileName = MainLogic.Config.Algorithm.RecipeFilePath + MainLogic.Config.Algorithm.RecipeFileName;//算法ini文件路径
        }

        public void InspectStart(string SN, string panelId, int recipe, string path)
        {
            _ImageId = 0;
            _SerialNumber = SN;
            _Recipe = recipe;
            _PanelId = panelId;
            IsInspecting = true;
            _RelativePath = path;

            CreatePanelProcessInfo(SN, panelId);

            if (HistoryUserLocal)
            {
                string pathname = HistoryPathBase + "/" + _RelativePath + "/";
                pathname = pathname.Replace("//", "/");
                if (!Directory.Exists(pathname)) Directory.CreateDirectory(pathname);
            }
        }

        public void InspectTerminate()
        {
            IsInspecting = false;
        }
        /// <summary>
        /// 相机连接状态
        /// </summary>
        /// <param name="status"></param>
        private void OnCameraConnectionStatus(bool status)
        {
            CameraStatus = status ? Constant.CameraStatusConnected : Constant.CameraStatusDisconnected;
        }

        private void OnCameraCapturingStatus(bool status)
        {
            if (status)
            {
                CameraStatus = Constant.CameraStatusCapturing;
            }
            else
            {
                if (CameraStatus == Constant.CameraStatusCapturing)
                    CameraStatus = Constant.CameraStatusConnected;
            }
        }


        private string CreateFileName(string panelId, int EdgeId, int imageId)
        {
            return string.Format("{0}_{1}_{2}_{3}", panelId, EdgeId, CameraId, imageId);
        }

        /// <summary>
        /// 相机图像数据处理，需要快速复制数据，归还相机线程
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="len"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void OnCameraCapturedData(IntPtr pData, uint len, int width, int height)
        {
            try
            {
                if (!IsInspecting) return;

                byte[] buffer = new byte[len];
                Marshal.Copy(pData, buffer, 0, (int)len);
                InspectImageInfoEntity info = new InspectImageInfoEntity()
                {
                    Recipe = _Recipe,
                    SerialNumber = _SerialNumber,
                    PanelId = _PanelId,
                    RelativePath = _RelativePath,
                    ImageId = Interlocked.Increment(ref _ImageId),
                    HistoryFilePath = HistoryPathBase + "/" + _RelativePath + "/",
                    //LocalFilePath = LocalPathBase + "/" + _RelativePath + "/",
                    ValidImageCount = 1/*cameraRecipeParam.ValidImageCount*/
                };
                ProcessCapturedData(buffer, width, height, info);

                if (_ImageId >= cameraRecipeParam.ValidImageCount) IsInspecting = false;
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn("复制相机数据异常，" + ex.Message);
            }
        }
        /// <summary>
        /// 异步处理相机图像数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="info"></param>
        private async void ProcessCapturedData(byte[] buffer, int width, int height, InspectImageInfoEntity info)
        {
            await Task.Run(async () =>
            {
                try
                {
                    //1.处理info信息
                    string filename = CreateFileName(info.PanelId, cameraRecipeParam.EdgeId, info.ImageId);
                    //存图路径
                    info.HistoryFilePath = HistoryPathBase + "/" + info.RelativePath + "/";
                    info.HistoryFileName = filename + ".jpg";
                    LoggerHelper.Info("收到照片 " + filename);

                    //2.OpenCV载入数据
                    Mat org = CreateImage(buffer, width, height, MatType.CV_8UC1);
                    LoggerHelper.Info(string.Format("OpenCV载入数据完成，开始存原图，传给算法检测!"));

                    //3.检测
                    var tInspect = AlgorithmProcess(org, info);

                    //4.历史图存储
                    //var tSave = HistoryImageSave(org, info);

                    //5.等待异步线程处理完成
                    var inspResult = await tInspect;
                    //var inspResult = JsonConvert.SerializeObject(new DefectInspectResEntity() { Count = 0, Data = new List<object>() });
                    //string fileName = await tSave;

                    if (inspResult == null)
                    {
                        //检测异常
                        MessageClient.Instance.SendInspectErrorToServer(info.SerialNumber, info.PanelId, cameraRecipeParam.EdgeId, 0, info.ImageId);
                    }
                    else
                    {
                        DefectInspectResEntity res = (DefectInspectResEntity)inspResult;

                        int result = res.Count > 0 ? Constant.JudgeNG : Constant.JudgeOK;
                        //7.向数据后台提交数据
                        CommitAdapter.Instance.CommitInspectResult(info.SerialNumber,
                            info.PanelId, cameraRecipeParam.EdgeId, CameraId, info.ImageId, info.HistoryFileName, res.Data, result);

                        //8.结果发送给Server
                        MessageClient.Instance.SendDefectDataToSever(info.SerialNumber,
                            info.PanelId, cameraRecipeParam.EdgeId, CameraId, info.ImageId, res.Data, result);

                        //count<0 算法异常
                        if (res.Count < 0)
                        {
                            switch (res.Count)
                            {
                                case -1://算法接口异常
                                    LoggerHelper.Warn(string.Format("调用算法接口异常,算法返回值：{0}", res.Count));
                                    break;
                                case -2://深度学习通讯异常
                                    LoggerHelper.Warn(string.Format("深度学习通讯异常,算法返回值：{0}", res.Count));
                                    break;
                                case -3://存图异常，算法未检测
                                    LoggerHelper.Warn(string.Format("存图异常,算法未检测,算法返回值：{0}", res.Count));
                                    break;
                                default:
                                    LoggerHelper.Warn(string.Format("算法检测异常,算法返回值：{0}", res.Count));
                                    break;
                            }
                        }
                    }
                    //await tSave;
                    org.Dispose();
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn(string.Format("处理{0} {1} 相机{2}第{3}张照片异常，{4}", info.PanelId, SliceName, CameraId, info.ImageId, ex.Message));

                    info.ErrCount++;
                }
                finally
                {
                    if (PanelProcessOneImage(info.SerialNumber, info.PanelId, info.ValidImageCount))
                    {
                        //相机检测完成
                        CameraInspectCompleteHandler?.Invoke(info.SerialNumber, info.PanelId, CameraId, info.ErrCount);
                    }
                }
            });
        }

        private Mat CreateImage(byte[] buffer, int width, int height, MatType type)
        {
            Mat org = new Mat();
            org.Create(height, width, type);
            Marshal.Copy(buffer, 0, org.Data, (int)buffer.Length);
            return org;
        }

        /// <summary>
        /// 异步算法处理流程
        /// </summary>
        /// <param name="image"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task<object> AlgorithmProcess(Mat src, InspectImageInfoEntity info)
        {
            return await Task<object>.Run(async () =>
            {
                try
                {
                    #region 存缩略图  传给算法
                    Mat dst = new Mat();
                    Cv2.Resize(src, dst, new Size() { Width = (int)(src.Width / deviceParamExt.ScaleFactorVer), Height = (int)(src.Height / deviceParamExt.ScaleFactorHor) });
                    int[] param = new int[2] { 1, Convert.ToInt32(deviceParamExt.PixelDistaFactor) };
                    string pathname = info.HistoryFilePath + info.HistoryFileName;
                    Cv2.ImWrite(pathname, dst, param);
                    #endregion

                    string arr = pathname.Replace("//", "/");
                    string arrpathname = arr.Replace("/", "\\");
                    Thread.Sleep(100);
                    LoggerHelper.Debug($"存图完成，路径：{arrpathname}，检测程序调用算法开始! 启用算法开关{AlgorithmEnable}");

                    if (AlgorithmEnable/*configEntity.Algorithm.Enable*/)//本地配置文件算法开关
                    {
                        //2.调用算法
                        ResponseEntity res = await AlgorithmAdapter.Instance.InspectRequest(
                            info.PanelId,
                            info.Recipe,
                            arrpathname,
                            info.HistoryFilePath,
                            CameraId,
                            info.ImageId);

                        //删除图
                        //ToolAdapter.Instance.CleanFile(arrpathname, 0);
                        switch (res.Code)
                        {
                            case 0: break;
                            case 1:
                                LoggerHelper.Info($"image not exist 。");
                                break;
                            case 51:
                                LoggerHelper.Info($"对位异常 。");
                                break;
                            case 52:
                                LoggerHelper.Info($"图像虚焦 。");
                                break;
                            case 101:
                                LoggerHelper.Info($"算法异常 。");
                                break;
                            default:
                                LoggerHelper.Info($"算法其他异常 。");
                                break;

                        }
                        //3.返回算法结果
                        return JsonConvert.DeserializeObject<DefectInspectResEntity>(res.Data.ToString());
                    }
                    else
                    {
                        LoggerHelper.Info(string.Format("不启用算法，开关为{0} Panel={1} EdgeId={2} Cam={3} imageId={4} ", AlgorithmEnable, info.PanelId, cameraRecipeParam.EdgeId, CameraId, info.ImageId));
                        //不启用算法，调用模拟的算法文件
                        DefectInspectResEntity defectInspectRes = AlgorithmAdapter.Instance.TradInspectRequest(arrpathname, info.HistoryFilePath, "C:/BJCW/AlgoConfig/recipe.ini", info.PanelId, cameraParam.EdgeId, CameraId, info.ImageId);
                        //AlgorithmAdapter.Instance.TradInspectRequest("E:/Images/20220923/FA8799232_121230/FA8799232_1_1_1.jpg", "E:/Images/20220923/FA8799232_121230/", "C:/BJCW/AlgoConfig/recipe.ini", "FA8799232", 1, 1, 1);
                        return JsonConvert.DeserializeObject<DefectInspectResEntity>(defectInspectRes.ToString());
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn(string.Format("算法处理异常，Panel={0} EdgeId={1} Cam={2} imageId={3} {4}", info.PanelId, cameraRecipeParam.EdgeId, CameraId, info.ImageId, ex.Message));
                    return null;
                }
            });
        }

        /// <summary>
        /// 加载配置文件参数
        /// </summary>
        /// <returns></returns>
        protected ConfigEntity LoadConfig()
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigEntity));
                using (var fs = File.OpenRead(Constant.ConfigFileName))
                {
                    return (ConfigEntity)xmlSerializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加载配置文件异常,{0}", ex.Message));
            }
        }

        private int cameraStatus = Constant.CameraStatusDisconnected;
        /// <summary>
        /// 相机链接状态
        /// </summary>
        public int CameraStatus
        {
            get { return cameraStatus; }
            set
            {
                if (cameraStatus == value) return;
                cameraStatus = value;
                CameraStatusChangeHandler?.Invoke(CameraId, value);
            }
        }

        private void CreatePanelProcessInfo(string sn, string panelCode)
        {
            string key = sn + "_" + panelCode;
            lock (_PanelProcessLock)
            {
                CountUtils item = new CountUtils();
                item.CountTimeoutHandler += OnPanelProcessTimeout;
                item.Start(key, 30);
                PanelProcess.Add(key, item);
            }
        }
        /// <summary>
        /// 当前相机完成一张照片处理
        /// </summary>
        /// <param name="sn">处理流水号</param>
        /// <param name="panelCode">产品码</param>
        /// <param name="validCount">相机处理有效张数</param>
        /// <returns>是否完成全部处理</returns>
        private bool PanelProcessOneImage(string sn, string panelCode, int validCount)
        {
            string key = sn + "_" + panelCode;
            lock (_PanelProcessLock)
            {
                if (PanelProcess.ContainsKey(key))
                {
                    if (PanelProcess[key].CountPlusOne() >= validCount)
                    {
                        PanelProcess[key].Dispose();
                        PanelProcess.Remove(key);
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnPanelProcessTimeout(string key)
        {
            lock (_PanelProcessLock)
            {
                PanelProcess.Remove(key);
                LoggerHelper.Info("检测超时，删除数据，PanelProcessRemove(key)： " + key);
            }
        }

        #region 暂时未使用
        /// <summary>
        /// 历史图（原图）存储
        /// </summary>
        /// <param name="src"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        //private async Task<string> HistoryImageSave(Mat src, InspectImageInfoEntity info)
        //{
        //    return await Task<string>.Run(() =>
        //    {
        //        try
        //        {
        //            #region 存图到NAS,原图
        //            Mat dst = new Mat();
        //            //存储到本地磁盘
        //            string pathname = info.HistoryFilePath + info.HistoryFileName;
        //            //Cv2.ImWrite(pathname, src);
        //            Cv2.Resize(src, dst, new Size() { Width = (int)(src.Width / 1), Height = (int)(src.Height / 1) });
        //            int[] param = new int[2] { 1, preProcessParams.JpegQuanlity };
        //            dst.SaveImage(pathname, param);
        //            #endregion

        //            //Mat dst = new Mat();
        //            //Cv2.Resize(src, dst, new Size() { Width = (int)(src.Width / preProcessParams.HistoryImageScaleX), Height = (int)(src.Height / preProcessParams.HistoryImageScaleY) });
        //            //int[] param = new int[2] { 1, preProcessParams.JpegQuanlity };
        //            //string pathname = info.HistoryFilePath + info.HistoryFileName;
        //            //Cv2.ImWrite(pathname, dst, param);

        //            LoggerHelper.Info("NAS原图存图" + pathname);

        //            return info.HistoryFileName;
        //        }
        //        catch (Exception ex)
        //        {
        //            LoggerHelper.Warn(string.Format("存储{0} {1} {2} image{3}异常, {4}",
        //                info.PanelId, cameraRecipeParam.EdgeId, CameraId, info.ImageId, ex.Message));
        //            return "";
        //        }
        //    });
        //}
        #endregion
    }
}
