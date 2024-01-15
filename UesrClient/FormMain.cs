using Newtonsoft.Json;
using CwCommon.Utils;
using Inspect.Adapter;
using Inspect.Commons;
using Inspect.Entities;
using Inspect.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Inspect
{
    public partial class FormMain : Form
    {
        public static ConfigEntity Config = null;
        //MySqlConnection Sqlconn;
        private int iRecipe = -1;
        private bool isInspet = false;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //foreach (AlgoServView view in AlgoViewList)
            //{
            //    view.Visible = false;
            //    view.StartAlgoServHandler += OnStartAlgoServ;
            //}

            ThreadPool.SetMinThreads(100, 100);

            string str = System.Environment.CurrentDirectory;
            string logFile = str + "\\" + "log4net-insp.xml";
            LoggerHelper.Init(logFile);

            Config = LoadConfig();

            //异步启动
            StartUpAsync();
        }

        protected async void StartUpAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    HearbeatAdapter.Instance.Enable = false;

                    //后台数据提交模块初始化
                    //CommitAdapter.Instance.Init(Config.CommitUrlBase, Config.ProgramId);

                    //加载设备参数
                    //LoadDeviceParams();

                    //初始化相机对象模块
                    //InitInspectCameras(Config.Cameras);

                    //初始化主从交互模块
                    InitMessageClient(Config.WsServerUrl, Config.ProgramId);

                    //初始化心跳模块
                    HearbeatAdapter.Instance.Create(Config.ProgramId, Config.HbDuration);

                    //初始化MySQL
                    //Sqlconn = SqlAdapter.Instance.MySqlConnect(Sqlconn);

                    LoggerHelper.Info("启动成功");

                    //提交软件版本信息
                    //CommitAdapter.Instance.SoftwareVersionAsync(Application.ProductVersion);
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
        /// 检测完成
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="cameraId"></param>
        /// <param name="errCode"></param>
        private void OnInspectComplete(string sn, string panelId, int cameraId, int errCode)
        {

        }

        /// <summary>
        /// 主从交互
        /// </summary>
        /// <param name="url"></param>
        /// <param name="progId"></param>
        private void InitMessageClient(string url, int progId)
        {
            //TODO: event init
            MessageClient.Instance.WsConnectedHandler += OnConnectStatus;
            MessageClient.Instance.UpdateParamsResHandler += OnUpdateParamsRes;

            MessageClient.Instance.Init(url, progId);
        }

        private void OnConnectStatus(bool status)
        {
            HearbeatAdapter.Instance.Enable = status;
        }
        private void OnUpdateParamsRes(int errCode)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LoggerHelper.Warn("处理UpdateParams异常" + ex.Message);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                HearbeatAdapter.Instance.Destroy();
                MessageClient.Instance.UnInit();
                LoggerHelper.Info("程序关闭");
            }
            catch (Exception) { }
        }

    }
}
