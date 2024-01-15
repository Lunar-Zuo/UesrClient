using CwCommon.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Inspect.Adapter
{
    public class HearbeatAdapter
    {
        public static HearbeatAdapter Instance = new HearbeatAdapter();

        private object csLock = new object();
        private Dictionary<int, int> CameraStatusList = new Dictionary<int, int>();
        private object asLock = new object();
        private Dictionary<string, int> AlgorithmStatusList = new Dictionary<string, int>();

        public bool Enable { get; set; }

        private static System.Timers.Timer RUtimer;
        public void Create(int programId, int duration)
        {
            RUtimer = new System.Timers.Timer(duration);
            RUtimer.Elapsed += OnTimerdEvent;
            RUtimer.AutoReset = true;
            RUtimer.Enabled = true;
            RUtimer.Start();
        }



        public void Destroy()
        {
            RUtimer.Stop();
        }

        public void CameraStatusChange(int camId, int status)
        {
            lock (csLock)
            {
                if (CameraStatusList.ContainsKey(camId))
                    CameraStatusList[camId] = status;
                else
                    CameraStatusList.Add(camId, status);
            }
        }

        public void AlgorithmStatusChange(string name, int status)
        {
            lock (asLock)
            {
                if (AlgorithmStatusList.ContainsKey(name)) AlgorithmStatusList[name] = status;
                else AlgorithmStatusList.Add(name, status);
            }
        }

        private object GetCameraStatus()
        {
            List<object> res = new List<object>();
            lock (csLock)
            {
                foreach (var item in CameraStatusList)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map["id"] = item.Key;
                    map["status"] = item.Value;
                    res.Add(map);
                }
            }
            return res;
        }

        private void OnTimerdEvent(object sender, ElapsedEventArgs e)
        {
            if (!Enable) return;
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("camera", GetCameraStatus());
                MessageClient.Instance.SendMessageToServerAsync(CwCommon.Commons.WsCmdName.WsCmdHeartbeat, data);
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn("心跳定时处理异常," + ex.Message);
            }
        }
    }
}
