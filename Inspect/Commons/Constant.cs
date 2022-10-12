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

        #region 触发模式
        public const uint TriggerSourceLine0 = 0;
        public const uint TriggerSourceLine1 = 1;
        public const uint TriggerSourceLine2 = 2;
        public const uint TriggerSourceLine3 = 3;
        public const uint TriggerSourceCounter0 = 4;
        public const uint TriggerSourceSoftware = 7;
        public const uint TriggerSourceFrequencyConverter = 8;
        #endregion

        #region 相机状态
        public const int CameraStatusDisconnected = 0;
        public const int CameraStatusConnected = 1;
        public const int CameraStatusCapturing = 2;
        #endregion

        #region 算法状态
        public const int AlgorithmNormal = 1;
        public const int AlgorithmUnNormal = 2;
        #endregion

        #region 硬件模块编号
        public const int ModuleIdInspectCamera = 2;
        #endregion

        #region Judge
        public const int JudgeOK = 1;
        public const int JudgeNG = 2;
        #endregion
    }
}
