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
using OpenCvSharp;
using Newtonsoft.Json.Linq;
using System.Threading;
using Inspect.Views;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Inspect
{
    public partial class FormMain : MainLogic
    {
        private List<AlgoServView> AlgoViewList;
        private List<AlgoServView> ValidAlgoServList;

        public FormMain()
        {
            InitializeComponent();

            AlgoViewList = new List<AlgoServView>() { algoServViews1, algoServViews2, algoServViews3, algoServViews4, algoServViews5, algoServViews6, algoServViews7, algoServViews8 };
            ValidAlgoServList = new List<AlgoServView>();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            foreach (AlgoServView view in AlgoViewList)
            {
                view.Visible = false;
                view.StartAlgoServHandler += OnStartAlgoServ;
            }

            ThreadPool.SetMinThreads(100, 100);

            string str = System.Environment.CurrentDirectory;
            string logFile = str + "\\" + "log4net-insp.xml";
            LoggerHelper.Init(logFile);

            Config = LoadConfig();

            InitAlgoServsView();

            //异步启动
            StartUpAsync();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                HearbeatAdapter.Instance.Destroy();
            }
            catch (Exception) { }

            UnInitInspectCameras();

            try
            {
                MessageClient.Instance.UnInit();
            }
            catch (Exception) { }

            KillAllAlgoServ();
        }


        protected override void SetRecipe(int recipe)
        {
            UiShowInfo(labelRecipeCurrent, recipe.ToString());
        }

        protected override void SetInspectInfo(string sn, string panelCode, int recipe, string path)
        {
            UiShowInfo(labelSN, sn);
            UiShowInfo(labelPanelCode, panelCode);
            UiShowInfo(labelRecipeInsp, recipe.ToString());
            UiShowInfo(labelPath, path);
        }

        private void UiShowInfo(Label label, string text)
        {
            Action act = (() =>
            {
                try { label.Text = text; } catch (Exception) { }
            });
            if (label.InvokeRequired) label.Invoke(act);
            else act();
        }

        #region 算法服务监测
        private void InitAlgoServsView()
        {
            for (int i=0; i<AlgoViewList.Count && i<Config.Algorithm.AlgoServs.Count; i++)
            {
                AlgoViewList[i].Visible = true;
                AlgoViewList[i]._Port = Config.Algorithm.AlgoServs[i].Port.ToString();
                AlgoViewList[i]._Name = Config.Algorithm.AlgoServs[i].Name;
                AlgoViewList[i]._Status = false;
                AlgoViewList[i]._Id = Config.Algorithm.AlgoServs[i].Id;

                ValidAlgoServList.Add(AlgoViewList[i]);
                OnStartAlgoServ(AlgoViewList[i]._Port);
                HearbeatAdapter.Instance.AlgorithmStatusChange(Config.Cameras[0].CameraId.ToString(), Constant.AlgorithmUnNormal);
                AlgorithmAdapter.Instance.AddAlogServ(AlgoViewList[i]._Port);
                TimeUtils.Yield(200);
            }
            AlgoServMonitorTimer = new System.Threading.Timer(OnAlgoServMonitorTimeout, "", 1000, 2000);
        }

        private readonly string AlgoServName = @"C:\AlgoServApp\AlgoHttpServer.exe";

        private void OnStartAlgoServ(string port)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = AlgoServName;
                startInfo.Arguments = string.Format("{0}", port);
                System.Diagnostics.Process.Start(startInfo);

                LoggerHelper.Info("启动算法服务，" + port);
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn(string.Format("算法启动失败,port={0}, {1}", port, ex.Message));
            } 
        }

        protected System.Threading.Timer AlgoServMonitorTimer;
        /// <summary>
        /// 定时器判断算法服务程序连接状态
        /// </summary>
        /// <param name="state"></param>
        private void OnAlgoServMonitorTimeout(object state)
        {
            try
            {
                int count = 0;
                //Console.WriteLine(DateTime.Now.ToString());
                IPEndPoint[] endPoints = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                foreach (AlgoServView view in ValidAlgoServList)
                {
                    view._Status = IsAlgoServAlive(endPoints, int.Parse(view._Port));
                    if (!view._Status) count++;
                }

                HearbeatAdapter.Instance.AlgorithmStatusChange(Config.Cameras[0].CameraId.ToString(), count > 0 ? 0 : 1);
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn("算法服务监测异常," + ex.Message);
            }

        }

        private bool IsAlgoServAlive(IPEndPoint[] endPoints, int port)
        {
            foreach (IPEndPoint point in endPoints)
            {
                if (point.Port == port) return true;
            }
            return false;
        }

        private void KillAllAlgoServ()
        {
            Process[] processes = Process.GetProcessesByName("AlgoHttpServer");
            foreach (Process proc in processes)
            {
                proc.Kill();
            }
        }
        #endregion
    }
}
