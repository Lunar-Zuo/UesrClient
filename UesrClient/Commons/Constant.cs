using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Commons
{
    public class Constant
    {
        #region 本地配置文件
        public const string ConfigFileName = ".\\configure-insp.xml";
        #endregion

        #region 相机状态
        public const int CameraStatusDisconnected = 0;
        public const int CameraStatusConnected = 1;
        public const int CameraStatusCapturing = 2;
        #endregion

    }

    public class WsCmdName
    {
        /// <summary>
        /// 数据更新
        /// </summary>
        public const string WsCmdUpdateParams = "update_params";
        /// <summary>
        /// 数据更新应答
        /// </summary>
        public const string WsCmdUpdateParamsRes = "update_params_res";
        /// <summary>
        /// 手动叫车呼叫
        /// </summary>
        public const string WsCmdCarCalling = "car_calling";
        /// <summary>
        /// 手动叫车呼叫应答
        /// </summary>
        public const string WsCmdCarCallingRes = "car_calling_res";
    }
}
