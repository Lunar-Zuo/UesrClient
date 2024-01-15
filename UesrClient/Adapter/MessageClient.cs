using CwCommon.Utils;
using Inspect.Commons;
using Inspect.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UesrClient.Entities;

namespace Inspect.Adapter
{
    public class MessageClient : WsClientHelper
    {
        public static MessageClient Instance = new MessageClient();

        public delegate void UpdateParamsResDelegate(int errCode);
        public delegate void WsConnectedDelegate(bool status);
        public delegate void CarCallingResDelegate(int errCode);

        public event UpdateParamsResDelegate UpdateParamsResHandler;
        public event WsConnectedDelegate WsConnectedHandler;
        public event CarCallingResDelegate CarCallingResHandler;

        public void Init(string url, int programId)
        {
            ConnectServer(url, programId);
        }

        public void UnInit()
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            SendMessageToServerAsync("unInit", JsonConvert.SerializeObject(body));
            Thread.Sleep(50);
            DisConnectServer();
        }


        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="errCode"></param>
        public void SendUpdateParamsToServer(int errCode)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("errCode", errCode);
            SendMessageToServerAsync(WsCmdName.WsCmdUpdateParams, JsonConvert.SerializeObject(body));
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
                    case WsCmdName.WsCmdCarCallingRes:
                        ProcessCarCallingResMessage(data);
                        break;
                    case WsCmdName.WsCmdUpdateParamsRes:
                        ProcessUpdateParamsResMessage(data);
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Warn(string.Format("解析{0}异常,{1}", cmd, ex.Message));
            }
        }
        /// <summary>
        /// 手动叫车应答
        /// </summary>
        /// <param name="data"></param>
        private void ProcessCarCallingResMessage(object data)
        {
            MessageResEntity entity = JsonConvert.DeserializeObject<MessageResEntity>(data.ToString());
            UpdateParamsResHandler?.Invoke(entity.errCode);
        }
        /// <summary>
        /// 数据更新应答
        /// </summary>
        /// <param name="data"></param>
        private void ProcessUpdateParamsResMessage(object data)
        {
            MessageResEntity entity = JsonConvert.DeserializeObject<MessageResEntity>(data.ToString());
            UpdateParamsResHandler?.Invoke(entity.errCode);
        }

    }
}
