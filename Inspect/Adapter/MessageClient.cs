using CwCommon.Commons;
using CwCommon.Utils;
using Inspect.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Adapter
{
    public class MessageClient : WsClientHelper
    {
        public static MessageClient Instance = new MessageClient();

        public delegate void InspectStartDelegate(string sn, string panelId, int recipe, string path);
        public delegate void InspectTerminateDelegate();
        public delegate void UpdateParamsDelegate();
        public delegate void RecipeSwitchDelegate(int recipe);
        public delegate void RecipeNoticeDelegate(int recipe);
        public delegate void WsConnectedDelegate(bool status);
        public event InspectStartDelegate InspectStartHandler;
        public event InspectTerminateDelegate InspectTerminateHandler;
        public event UpdateParamsDelegate UpdateParamsHandler;
        public event RecipeSwitchDelegate RecipeSwitchHandler;
        public event RecipeNoticeDelegate RecipeNoticeHandler;
        public event WsConnectedDelegate WsConnectedHandler;

        public void Init(string url, int programId)
        {
            ConnectServer(url, programId);
        }

        public void UnInit()
        {
            DisConnectServer();
        }

        /// <summary>
        /// 检测异常信息给主程序
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="sliceId"></param>
        /// <param name="edgeId"></param>
        /// <param name="imageId"></param>
        public void SendInspectErrorToServer(string sn, string panelId, int sliceId, int edgeId, int imageId)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("serialNumber", sn);
            body.Add("panelId", panelId);
            body.Add("sliceId", sliceId);
            body.Add("edgeId", edgeId);
            body.Add("imageId", imageId);
            SendMessageToServerAsync(CwCommon.Commons.WsCmdName.WsCmdInspectError, JsonConvert.SerializeObject(body));
        }
        /// <summary>
        /// 检测完成信号
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="errCode"></param>
        public void SendInspectCompleteToServer(string sn, string panelId, int errCode)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("serialNumber", sn);
            body.Add("panelCode", panelId);
            body.Add("errCode", errCode);
            SendMessageToServerAsync(CwCommon.Commons.WsCmdName.WsCmdInspectComplete, JsonConvert.SerializeObject(body));
        }
        /// <summary>
        /// 缺陷信息给主程序
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="panelId"></param>
        /// <param name="sliceId"></param>
        /// <param name="cameraId"></param>
        /// <param name="imageId"></param>
        /// <param name="data"></param>
        /// <param name="result"></param>
        public void SendDefectDataToSever(string sn, string panelId, int sliceId, int cameraId, int imageId, object data, int result)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("serialNumber", sn);
            body.Add("panelCode", panelId);
            body.Add("sliceId", sliceId);
            body.Add("cameraId", cameraId);
            body.Add("imageId", imageId);
            body.Add("defectData", data);
            body.Add("judge", result);
            LoggerHelper.Info("发送缺陷数据： " + JsonConvert.SerializeObject(data).Count());
            SendMessageToServerAsync(WsCmdName.WsCmdDefectData, JsonConvert.SerializeObject(body));
        }
        /// <summary>
        /// 回应主程序，主程序通知PLC允许检测
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="sn"></param>
        public void SendInspectStartResToServer(int errCode, string sn)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("errCode", errCode);
            body.Add("serialNumber", sn);
            SendMessageToServerAsync(WsCmdName.WsCmdInspectStartRes, JsonConvert.SerializeObject(body));
        }
        /// <summary>
        /// 2.15配置切换应答
        /// </summary>
        /// <param name="errCode"></param>
        public void SendUpdateCompleteToServer(int errCode)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("errCode", errCode);
            SendMessageToServerAsync(WsCmdName.WsCmdUpdateParamsRes, JsonConvert.SerializeObject(body));
        }
        /// <summary>
        /// 2.13配方更新应答
        /// </summary>
        /// <param name="errCode"></param>
        public void SendRecipeCompleteToServer(int errCode)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("errCode", errCode);
            SendMessageToServerAsync(WsCmdName.WsCmdRecipeSwitchRes, JsonConvert.SerializeObject(body));
        }
        
        protected override void OnConnectedtoServer(int id)
        {
            WsConnectedHandler?.Invoke(true);
        }

        protected override void OnDisconnectedFromServer(int id)
        {
            WsConnectedHandler?.Invoke(false);
        }

        protected override void OnMessageRecv(int id, string cmd, object data)
        {
            try
            {
                switch (cmd)
                {
                    case WsCmdName.WsCmdInspectStart:
                        ProcessInspectStartMessage(data);
                        break;
                    case WsCmdName.WsCmdInspectTerminate:
                        InspectTerminateHandler?.Invoke();
                        break;
                    case WsCmdName.WsCmdUpdateParams:
                        UpdateParamsHandler?.Invoke();
                        break;
                    case WsCmdName.WsCmdRecipeSwitch:
                        ProcessRecipeSwitch(data);
                        break;
                    case WsCmdName.WsCmdRecipeNotice:
                        ProcessRecipeNotice(data);
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn(string.Format("解析{0}异常,{1}", cmd, ex.Message));
            }
        }
        /// <summary>
        /// 检测开始应答
        /// </summary>
        /// <param name="data"></param>

        private void ProcessInspectStartMessage(object data)
        {
            InspectStartEntity entity = JsonConvert.DeserializeObject<InspectStartEntity>(data.ToString());
            InspectStartHandler?.Invoke(entity.SerialNumber, entity.PanelId, entity.Recipe, entity.Path);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ProcessRecipeSwitch(object data)
        {
            RecipeSwitchReqEntity entity = JsonConvert.DeserializeObject<RecipeSwitchReqEntity>(data.ToString());
            RecipeSwitchHandler?.Invoke(entity.Recipe);
        }

        private void ProcessRecipeNotice(object data)
        {
            RecipeSwitchReqEntity entity = JsonConvert.DeserializeObject<RecipeSwitchReqEntity>(data.ToString());
            RecipeNoticeHandler?.Invoke(entity.Recipe);
        }

    }
}
