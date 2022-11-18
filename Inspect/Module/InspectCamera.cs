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

        private DalsaCameraAdapter device = null;

        private CameraParamsEntity cameraParam = null;
        private CameraRecipeParamsEntity cameraRecipeParam = null;
        private ImagePreProcessParamsEntity preProcessParams = null;

        //针对当前相机是否完成处理
        private Dictionary<string, CountUtils> PanelProcess = new Dictionary<string, CountUtils>();
        private object _PanelProcessLock = new object();

        private bool bCameraOn = false;

        public bool IsInspecting { get; set; }
        public string SliceName { get; set; }
        //public int SliceId { get; set; }
        public int CameraId { get; set; }
        public bool HistoryUserLocal = false;
        public string HistoryPathBase = @"E:\Images";
        public string LocalPathBase = @"E:\LocalImage";
        public string AlgorithmRecipeFileName = @"C:\BJCW\AlgoConfig\recipe.ini";

        //数据更新，配方切换，相机初始化流程保护
        private object _cameraLock = new object();

        private string _SerialNumber;
        private string _PanelId;
        private int _Recipe;
        private int _ImageId;
        private string _RelativePath;

        /// <summary>
        /// 后台加载参数，开启相机线程
        /// </summary>
        /// <param name="recipe"></param>
        public void Create(int recipe)
        {
            if (bCameraOn) return;

            _Recipe = recipe;
            cameraStatus = 0;

            IsInspecting = false;

            LoadParams(recipe);

            device = new DalsaCameraAdapter();
            device.CameraConnectionStatusHandler += OnCameraConnectionStatus;
            device.ConfigFileName = cameraRecipeParam.SetupFileName;
            device.OpenDevice();

            device.SetFeatureValueDouble(DalsaCameraAdapter.FeatureNameGain, double.Parse(cameraRecipeParam.Gain));
            device.SetFeatureValueDouble(DalsaCameraAdapter.FeatureNameExposureTime, double.Parse(cameraRecipeParam.ExposureTime));

            device.CameraCapuredDataHandler += OnCameraCapturedData;
            device.StartCapture();
            bCameraOn = true;
        }

        public void Destroy()
        {
            if (bCameraOn)
            {
                device.CloseDevice();
                device.CameraCapuredDataHandler -= OnCameraCapturedData;
                device = null;
                bCameraOn = false;
            }
        }
        /// <summary>
        /// 后台拿数据，开启相机线程
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

        public void LoadParams(int recipe)
        {
            LoadCameraParams();
            LoadCameraRecipeParams(recipe);
            LoadRecipeAlgorithmParams(recipe);
            LoadImagePreProcessParams(recipe);
            AlgorithmRecipeFileName = MainLogic.Config.Algorithm.RecipeFilePath + MainLogic.Config.Algorithm.RecipeFileName;//算法ini文件路径
        }

        private void LoadImagePreProcessParams(int recipe)
        {
            preProcessParams = CommitAdapter.Instance.GetImagePreProcessParams(recipe);
        }

        private void LoadCameraRecipeParams(int recipe)
        {
            cameraRecipeParam = CommitAdapter.Instance.GetCameraRecipeParams(CameraId, recipe);
        }

        private void LoadCameraParams()
        {
            cameraParam = CommitAdapter.Instance.GetCameraParams(CameraId);
        }

        /// <summary>
        /// 后台加载所有数据，包括相机参数，预处理参数、解析参数等
        /// </summary>
        public void LoadRecipeAlgorithmParams(int recipe)
        {
            var res = CommitAdapter.Instance.GetTradAlgoRecipeParams(recipe);
            string pathname = MainLogic.Config.Algorithm.RecipeFilePath + MainLogic.Config.Algorithm.RecipeFileName;

            List<TradAlgoInspParamEntity> data = JsonConvert.DeserializeObject<List<TradAlgoInspParamEntity>>(res.ToString());
            TradAlgoInspFileHelper.WriteTradAlgoInspFile(pathname, data);

            //2022.09.17增加将recipe基础配置生成ini文件
            RecipeParamEntity re = CommitAdapter.Instance.GetRecipeParam(recipe);
            TradAlgoInspFileHelper.WriteTradAlgoInspFile(pathname, re);
        }

        public void ExecTriggerSoftware()
        {
            //device.TriggerSoftwareExec();
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


        private string CreateFileName(string panelId,int SliceId ,int imageId)
        {
            return string.Format("{0}_{1}_{2}_{3}", panelId, SliceId, CameraId, imageId);
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
                    ValidImageCount = cameraRecipeParam.ValidImageCount
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
                    string filename = CreateFileName(info.PanelId, cameraParam.SliceId, info.ImageId);
                    info.HistoryFileName = HistoryPathBase + "/" + info.RelativePath + "/";
                    info.HistoryFileName = filename + ".jpg";
                    //info.LocalFileName = LocalPathBase + "/" + info.RelativePath + "/";
                    //info.LocalFileName = filename + ".jpg";

                    LoggerHelper.Info("收到照片 " + filename);

                    //2.OpenCV载入数据
                    Mat org = CreateImage(buffer, width, height, MatType.CV_8UC1);
                    LoggerHelper.Info(string.Format("OpenCV载入数据完成，开始存原图，传给算法检测"));

                    //3.检测
                    var tInspect = AlgorithmProcess(org, info);

                    //4.历史图存储
                    //var tSave = HistoryImageSave(org, info);

                    //5.实时缩略图处理
                    RealThumb(org, info);

                    //6.等待异步线程处理完成
                    var inspResult = await tInspect;
                    //var inspResult = JsonConvert.SerializeObject(new DefectInspectResEntity() { Count = 0, Data = new List<object>() });
                    //string fileName = await tSave;

                    if (inspResult == null)
                    {
                        //检测异常
                        MessageClient.Instance.SendInspectErrorToServer(info.SerialNumber, info.PanelId, cameraParam.SliceId, 0, info.ImageId);
                    }
                    else
                    {
                        DefectInspectResEntity res = (DefectInspectResEntity)inspResult;
                        
                        int result = res.Count > 0 ? Constant.JudgeNG : Constant.JudgeOK;
                        //7.向数据后台提交数据
                        CommitAdapter.Instance.CommitInspectResult(info.SerialNumber,
                            info.PanelId, cameraParam.SliceId, CameraId, info.ImageId, info.HistoryFileName, res.Data, result);

                        //8.结果发送给Server
                        MessageClient.Instance.SendDefectDataToSever(info.SerialNumber,
                            info.PanelId, cameraParam.SliceId, CameraId, info.ImageId, res.Data, result);

                        //count<0 算法异常
                        if (res.Count < 0)
                        {
                            ConfigEntity Config= LoadConfig();
                            HearbeatAdapter.Instance.AlgorithmStatusChange(Config.Cameras[0].CameraId_local.ToString(), 0);

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
                    #region 存图到NAS,缩略图
                    Mat dst = new Mat();
                    Cv2.Resize(src, dst, new Size() { Width = (int)(src.Width / preProcessParams.HistoryImageScaleX), Height = (int)(src.Height / preProcessParams.HistoryImageScaleY) });
                    int[] param = new int[2] { 1, preProcessParams.JpegQuanlity };
                    string pathname = info.HistoryFilePath + info.HistoryFileName;
                    Cv2.ImWrite(pathname, dst, param);
                    #endregion

                    #region 存图到本地,原图
                    //Mat dst = new Mat();
                    ////1.存储到本地磁盘
                    //string pathname = info.LocalFilePath + info.LocalFileName;
                    ////Cv2.ImWrite(pathname, src);
                    //Cv2.Resize(src, dst, new Size() { Width = (int)(src.Width / 1), Height = (int)(src.Height / 1) });
                    //int[] param = new int[2] { 1, preProcessParams.JpegQuanlity };
                    //dst.SaveImage(pathname, param);
                    #endregion

                    string arr = pathname.Replace("//", "/");
                    string arrpathname = arr.Replace("/", "\\");
                    Thread.Sleep(100);
                    //LoggerHelper.Debug($"原图存图完成，路径：{arrpathname}，检测程序调用算法开始!");

                    //2.调用算法
                    //return AlgorithmAdapter.Instance.TradInspectRequest(pathname, info.HistoryFilePath, "C:/BJCW/AlgoConfig/recipe.ini", info.PanelId, cameraParam.SliceId, CameraId, info.ImageId);
                    ResponseEntity res = await AlgorithmAdapter.Instance.InspectRequest(arrpathname, 
                        info.HistoryFilePath, 
                        AlgorithmRecipeFileName, 
                        info.PanelId,
                        /*cameraParam.SliceId*/
                        info.Recipe, 
                        CameraId, 
                        info.ImageId);

                    if (res.Code != 0)
                    {
                        //返回异常
                        throw new Exception("算法请求异常," + res.Message);
                    }
                    //3.返回算法结果
                    return JsonConvert.DeserializeObject<DefectInspectResEntity>(res.Data.ToString());
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn(string.Format("Panel={0} Slice={1} Cam={2} imageId={3} {4}", info.PanelId, cameraParam.SliceId, CameraId, info.ImageId, ex.Message));
                    return null;
                }
            });
        }


        /// <summary>
        /// 历史图（缩略图）存储
        /// </summary>
        /// <param name="src"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task<string> HistoryImageSave(Mat src, InspectImageInfoEntity info)
        {
            return await Task<string>.Run(() =>
            {
                try
                {
                    Mat dst = new Mat();
                    Cv2.Resize(src, dst, new Size() { Width = (int)(src.Width / preProcessParams.HistoryImageScaleX), Height = (int)(src.Height / preProcessParams.HistoryImageScaleY) });
                    int[] param = new int[2] { 1, preProcessParams.JpegQuanlity };
                    string pathname = info.HistoryFilePath + info.HistoryFileName;
                    Cv2.ImWrite(pathname, dst, param);

                    LoggerHelper.Info("缩略图存图" + pathname);

                    return info.HistoryFileName;
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn(string.Format("存储{0} {1} {2} image{3}异常, {4}",
                        info.PanelId, cameraParam.SliceId, CameraId, info.ImageId, ex.Message));
                    return "";
                }
            });
        }

        /// <summary>
        /// 处理实时显示缩略图
        /// </summary>
        /// <param name="src"></param>
        /// <param name="info"></param>
        private async void RealThumb(Mat src, InspectImageInfoEntity info)
        {
            await Task.Run(() =>
            {
                try
                {
                    Mat dst = new Mat();
                    Cv2.Resize(src, dst, new Size() { Width = preProcessParams.ThumbWidth, Height = preProcessParams.ThumbHeight });
                    byte[] buf;
                    int[] param = new int[2] { 1, 100 };
                    Cv2.ImEncode(".jpg", dst, out buf, param);
                    string strBase64 = Convert.ToBase64String(buf);
                    MessageClient.Instance.SendRealThumbToServer(info.SerialNumber,
                        info.PanelId,
                        cameraParam.SliceId,
                        CameraId,
                        info.ImageId,
                        strBase64);
                    LoggerHelper.Info($"缩略图信息：{info.SerialNumber},{info.PanelId},{cameraParam.SliceId},{CameraId},{info.ImageId},{strBase64.Length}");
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn(string.Format("处理{0} {1} {2} image{3}缩略图异常, {4}",
                        info.PanelId, cameraParam.SliceId, CameraId, info.ImageId, ex.Message));
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
                item.Start(key,30);
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
                LoggerHelper.Info("检测超时，删除数据，PanelProcessRemove(key)： "+ key);
            }
        }
    }
}
