using CwCommon.Utils;
using Inspect.Adapter;
using Inspect.Commons;
using Inspect.Entities;
using Inspect.Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Inspect
{
    public class MainLogic : Form
    {
        /// <summary>
        /// 相机对象
        /// </summary>
        protected static Dictionary<int, InspectCamera> CameraList = new Dictionary<int, InspectCamera>();
        public static DeviceParamEntity DeviceParams = null;
        public static DeviceParamExtEntity DeviceParamsExt = null;
        public static List<CameraParamsEntity> CameraParams = null;
        public static ConfigEntity Config = null;
        private int iRecipe = -1;
        private bool isInspet = false;

        protected async void StartUpAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    HearbeatAdapter.Instance.Enable = false;

                    //后台数据提交模块初始化
                    CommitAdapter.Instance.Init(Config.CommitUrlBase, Config.ProgramId);

                    //加载设备参数
                    LoadDeviceParams();

                    //初始化相机对象模块
                    InitInspectCameras(Config.Cameras);

                    //初始化主从交互模块
                    InitMessageClient(Config.WsServerUrl, Config.ProgramId);

                    //初始化心跳模块
                    HearbeatAdapter.Instance.Create(Config.ProgramId, Config.HbDuration);

                    LoggerHelper.Info("启动成功");

                    //提交软件版本信息
                    CommitAdapter.Instance.SoftwareVersionAsync(Application.ProductVersion);
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn("启动异常," + ex.Message);
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
        /// <summary>
        /// 加载界面设置的设备参数
        /// </summary>
        protected void LoadDeviceParams()
        {
            DeviceParams = CommitAdapter.Instance.GetDeviceParam();
            DeviceParamsExt = CommitAdapter.Instance.GetDeviceParamExt();
            CameraParams = CommitAdapter.Instance.GetCameraParams();
        }
        /// <summary>
        /// 初始化相机对象，加载相机Id
        /// </summary>
        /// <param name="list"></param>
        /// <param name="autoStart"></param>
        private void InitInspectCameras(List<ConfigCameraEntity> list)
        {
            try
            {
                foreach (var item in list)
                {
                    InspectCamera camera = new InspectCamera();
                    camera.CameraId = item.CameraId_local;

                    camera.HistoryUserLocal = Config.ImageSave.UserLocalHistoryPath;
                    if (Config.ImageSave.UserLocalHistoryPath)
                    {
                        camera.LocalPathBase = Config.ImageSave.ImageCachePath;
                        camera.HistoryPathBase = Config.ImageSave.HistoryImagePath;
                    }
                    else
                    {
                        camera.LocalPathBase = DeviceParamsExt.ImageCachePath;
                        camera.HistoryPathBase = DeviceParams.HistoryImageStoragePath;
                    }
                    camera.CameraStatusChangeHandler += HearbeatAdapter.Instance.CameraStatusChange;
                    camera.CameraInspectCompleteHandler += OnInspectComplete;
                    CameraList.Add(item.CameraId_local, camera);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("创建检测模块异常，" + ex.Message);
            }

        }
        /// <summary>
        /// 检测完成
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="cameraId"></param>
        /// <param name="errCode"></param>
        private void OnInspectComplete(string sn, string panelId, int cameraId, int errCode)
        {
            MessageClient.Instance.SendInspectCompleteToServer(sn, panelId, errCode);
            isInspet = false;
            //删除本地原图
            //DeleteFile(DeviceParams.OriginImageStoragePath,DeviceParams.OriginImageStorageDays);
            //CleanFile("D:/DefectData", 0/*DeviceParams.DataStorageDays*/);
        }
        protected void UnInitInspectCameras()
        {
            foreach (var item in CameraList.Values)
            {
                try
                {
                    item.Destroy();
                }
                catch (Exception) { }
            }
            CameraList.Clear();
        }

        /// <summary>
        /// 主从交互
        /// </summary>
        /// <param name="url"></param>
        /// <param name="progId"></param>
        private void InitMessageClient(string url, int progId)
        {
            //TODO: event init
            MessageClient.Instance.InspectStartHandler += OnInspectStart;
            MessageClient.Instance.InspectTerminateHandler += OnInspectStop;
            MessageClient.Instance.RecipeSwitchHandler += OnRecipeSwitch;
            MessageClient.Instance.UpdateParamsHandler += OnUpdateParams;
            MessageClient.Instance.RecipeNoticeHandler += OnRecipeNotice;
            MessageClient.Instance.WsConnectedHandler += OnConnectStatus;

            MessageClient.Instance.Init(url, progId);
        }

        private void OnConnectStatus(bool status)
        {
            HearbeatAdapter.Instance.Enable = status;
        }
        /// <summary>
        /// 主从通信初始化
        /// </summary>
        /// <param name="recipe"></param>
        private void OnRecipeNotice(int recipe)
        {
            RecipeNoticeProcess(recipe);
        }
        /// <summary>
        /// 接收到主程序的recipe
        /// </summary>
        /// <param name="recipe">配方号</param>
        /// <returns></returns>
        private async Task RecipeNoticeProcess(int recipe)
        {
            await Task.Run(() =>
            {
                try
                {
                    LoggerHelper.Info("主程序通知Recipe=" + recipe);

                    if (recipe <= 0) throw new Exception("请求Recipe值异常，recipe=" + recipe);
                    if (_Recipe == recipe) return;

                    foreach (var cam in CameraList)
                    {
                        //开启相机线程
                        cam.Value.Reload(recipe);
                    }
                    _Recipe = recipe;
                    LoggerHelper.Info("主程序通知Recipe处理成功，相机初始化完成");
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warn("处理RecipeNotice异常" + ex.Message);
                }
            });
        }

        private void OnUpdateParams()
        {
            UpdateParams();
        }
        private async Task UpdateParams()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_Recipe == -1) throw new Exception("没有初始化Recipe");

                    //加载设备参数
                    LoadDeviceParams();

                    //相机配方参数重新加载
                    foreach (var cam in CameraList)
                    {
                        if (!Config.ImageSave.UserLocalHistoryPath)
                        {
                            cam.Value.LocalPathBase = DeviceParamsExt.ImageCachePath;
                            cam.Value.HistoryPathBase = DeviceParams.HistoryImageStoragePath;
                        }
                        LoggerHelper.Info($"配置更新,检测状态{isInspet},准备重启相机,写入参数！");
                        if (!isInspet)
                        {   //点击配置更新，会重新加载相机
                            cam.Value.Reload(_Recipe);
                        }
                        else
                        {
                            cam.Value.UpdateParams(_Recipe);
                        }
                    }

                    MessageClient.Instance.SendUpdateCompleteToServer(0);

                }
                catch (Exception ex)
                {
                    MessageClient.Instance.SendUpdateCompleteToServer(1);
                    LoggerHelper.Warn("处理UpdateParams异常" + ex.Message);
                }
            });
        }
        /// <summary>
        /// 配置更新
        /// </summary>
        /// <param name="recipe"></param>
        private void OnRecipeSwitch(int recipe)
        {
            RecipeSwitchAsync(recipe);
        }
        private async Task RecipeSwitchAsync(int recipe)
        {
            await Task.Run(() =>
            {
                try
                {
                    LoggerHelper.Info("通知Recipe=" + recipe);
                    if (recipe <= 0) throw new Exception("请求Recipe值异常，recipe=" + recipe);
                    if (recipe != _Recipe)
                    {

                        foreach (var cam in CameraList)
                        {
                            if (!Config.ImageSave.UserLocalHistoryPath)
                            {
                                cam.Value.LocalPathBase = DeviceParamsExt.ImageCachePath;
                                cam.Value.HistoryPathBase = DeviceParams.HistoryImageStoragePath;
                            }

                            cam.Value.Reload(recipe);
                        }

                        _Recipe = recipe;
                    }
                    LoggerHelper.Info("切换Recipe处理成功，recipe=" + recipe);
                    MessageClient.Instance.SendRecipeCompleteToServer(0);
                }
                catch (Exception ex)
                {
                    MessageClient.Instance.SendRecipeCompleteToServer(1);

                    LoggerHelper.Warn(string.Format("处理RecipeSwitch异常, recipe={0} err={1}", recipe, ex.Message));
                }
            });
        }
        /// <summary>
        /// 检测停止
        /// </summary>
        private void OnInspectStop()
        {
            try
            {
                foreach (var item in CameraList.Values)
                {
                    item.InspectTerminate();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn("处理InspectStop异常" + ex.Message);
            }
        }

        /// <summary>
        /// 检测开始应答，回应主程序通知PLC允许进入
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="recipe"></param>
        /// <param name="path"></param>
        private void OnInspectStart(string sn, string panelId, int recipe, string path)
        {
            try
            {
                isInspet = true;
                LoggerHelper.Info(string.Format("sn={0} panelId={1} recipe={2} path={3}", sn, panelId, recipe, path));

                //创建本地图存储目录
                //string pathname = DeviceParams.OriginImageStoragePath + "/" + path;
                //pathname = pathname.Replace("//", "/");
                //if (!Config.ImageSave.UserLocalHistoryPath)
                //    if (!Directory.Exists(pathname))
                //        Directory.CreateDirectory(pathname);

                //检测端显示信息
                SetInspectInfo(sn, panelId, recipe, path);

                foreach (var item in CameraList.Values)
                {
                    item.InspectStart(sn, panelId, recipe, path);
                }
                MessageClient.Instance.SendInspectStartResToServer(0, sn);
                LoggerHelper.Info("检测端应答成功，PanelID=" + panelId);
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn(string.Format("处理InspectStart sn={0} id={1}异常, {2}", sn, panelId, ex.Message));
            }
        }

        public int _Recipe
        {
            get { return iRecipe; }
            set
            {
                if (iRecipe == value) return;
                iRecipe = value;
                SetRecipe(value);
            }
        }
        protected virtual void SetRecipe(int recipe) { }
        protected virtual void SetInspectInfo(string sn, string panelCode, int recipe, string path) { }

    }

}
