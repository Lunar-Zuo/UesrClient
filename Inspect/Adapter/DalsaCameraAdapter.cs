using CwCommon.Utils;
using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspect.Adapter
{
    public class DalsaCameraAdapter
    {
        public delegate void CameraConnectionStatusDelegate(bool status);
        public delegate void CameraCapturingStatusDelegate(bool status);
        public delegate void CameraCapuredDataDelegate(IntPtr pData, uint len, int width, int height);

        public event CameraConnectionStatusDelegate CameraConnectionStatusHandler;
        public event CameraCapturingStatusDelegate CameraCapturingStatusHandler;
        public event CameraCapuredDataDelegate CameraCapuredDataHandler;

        public bool IsConnected { get; set; }
        public bool IsCapturing { get; set; }

        private SapLocation loc = null;
        private SapAcqDevice AcqDevice = null;
        private SapBuffer Buffers = null;

        private SapAcquisition Acq = null;
        private SapFeature Feature = null;
        private SapTransfer Xfer = null;
        private SapLocation loc2 = null;

        private SapAcqDeviceToBuf m_Xfer = null;
        Dictionary<int, string> ServerNames = new Dictionary<int, string>();
        Dictionary<int, string> DeviceNames = new Dictionary<int, string>();
        public string ConfigFileName { get; set; }//相机配置文件路径
        public const string ServerName = "Xtium-CL_MX4_1";//相机硬件配置参数，显卡的名称
        /// <summary>
        /// 采集卡连接相机
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void OpenDevice()
        {
            //TODO:???这个值是否会改变
            int ResourceIndex = 0;//相机硬配置参数，选择模式

            loc = new SapLocation(ServerName, ResourceIndex);//第一位是显卡的名称；第二位是选择模式的第几项

            if (SapManager.GetResourceCount(ServerName, SapManager.ResourceType.Acq) > 0)
            {
                Acq = new SapAcquisition(loc, ConfigFileName);
                Buffers = new SapBuffer(2, Acq, SapBuffer.MemoryType.ScatterGather);
                Xfer = new SapAcqToBuf(Acq, Buffers);

                // Create acquisition object
                if (!Acq.Create())
                {
                    throw new Exception("SapAcquisition Create Failed");
                }
            }
            else
            {
                //TODO：给主程序发消息，主程序弹窗显示
                LoggerHelper.Warn($"相机{ConfigFileName}丢失！");
            }

            loc2 = new SapLocation("CameraLink_1", 0);//相机硬件配置参数
            AcqDevice = new SapAcqDevice(loc2, false);
            Feature = new SapFeature(loc2);
            Feature.Create();

            // Create acquisition object
            if (!AcqDevice.Create())
            {
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
                throw new Exception("SapAcqDevice Create Failed");
            }

            // End of frame event
            Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            Xfer.XferNotifyContext = this;

            // Create buffer object
            if (!Buffers.Create())
            {
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
                throw new Exception("Error during SapBuffer creation");
            }

            // Create transfer object
            if (!Xfer.Create())
            {
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
                throw new Exception("Error during SapTransfer creation!");
            }

            CameraConnectionStatus(true);
        }
        /// <summary>
        /// 网口连接相机
        /// </summary>
        /// <param name="CameraId"></param>
        /// <exception cref="Exception"></exception>
        public void GigeOpenDevice(int serverIndex)
        {
            string CameraName;
            int Index;
            bool bFind = GetNameInfo(serverIndex, out CameraName, out Index);
            //string DeviceUserID;
            //bool bFind = GetNameInfo(CameraId, out CameraName, out DeviceUserID);
            loc = new SapLocation(CameraName, 0);
            AcqDevice = new SapAcqDevice(loc, ConfigFileName);
            if (SapBuffer.IsBufferTypeSupported(loc, SapBuffer.MemoryType.ScatterGather))
                Buffers = new SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGather);
            else
                Buffers = new SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);

            // End of frame event 创建传输对象
            m_Xfer = new SapAcqDeviceToBuf(AcqDevice, Buffers);
            m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;
            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
            //Feature = new SapFeature(loc);
            //Feature.Create();

            // Create acquisition object
            if (!AcqDevice.Create())
            {
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
                throw new Exception("SapAcqDevice Create Failed");
            }

            // Create buffer object
            if (!Buffers.Create())
            {
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
                throw new Exception("Error during SapBuffer creation");
            }

            // Create transfer object
            if (!m_Xfer.Create())
            {
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
                throw new Exception("Error during SapTransfer creation!");
            }
            CameraConnectionStatus(true);
        }
        public bool GetNameInfo(int serverIndex, out string sCameraName, out string nDeviceUserID)
        {
            sCameraName = "";
            nDeviceUserID = "";
            bool bFind = false;
            string serverName = "";

            if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
            {
                serverName = SapManager.GetServerName(serverIndex);
                //listServerNames.Add(serverName);
                ServerNames.Add(serverIndex, serverName);
                bFind = true;
            }
            string deviceName = "";
            //Device User ID(相机大师中可以重新命名)
            deviceName = SapManager.GetResourceName(serverName, SapManager.ResourceType.AcqDevice, 0);
            DeviceNames.Add(serverIndex, deviceName);

            sCameraName = serverName;
            nDeviceUserID = deviceName;
            return bFind;
        }
        /// <summary>
        /// 获取相机连接信息
        /// </summary>
        /// <param name="serverIndex">相机排序号1-X</param>
        /// <param name="sCameraName">相机型号（相机大师中Device）</param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool GetNameInfo(int serverIndex, out string sCameraName, out int nIndex)
        {
            sCameraName = "";
            nIndex = 0;
            int serverCount = SapManager.GetServerCount();
            int GenieIndex = 0;
            //System.Collections.ArrayList listServerNames = new System.Collections.ArrayList();
            bool bFind = false;
            string serverName = "";
            //for (int serverIndex = 1; serverIndex <= num; serverIndex++)
            //{
            if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
            {
                serverName = SapManager.GetServerName(serverIndex);
                //listServerNames.Add(serverName);
                ServerNames.Add(serverIndex, serverName);
                GenieIndex++;
                bFind = true;
            }
            //}
            int count = 1;
            string deviceName = "";
            //foreach (string sName in listServerNames)
            //{
            //Device User ID(相机大师中可以重新命名)
            deviceName = SapManager.GetResourceName(serverName, SapManager.ResourceType.AcqDevice, 0);
            DeviceNames.Add(serverIndex, deviceName);
            count++;
            //}
            sCameraName = serverName;
            nIndex = GenieIndex;
            return bFind;
        }
        public void CloseDevice()
        {
            if (IsCapturing) StopCapture();

            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
            loc.Dispose();
            //loc2.Dispose();

            CameraConnectionStatus(false);
        }

        public void StartCapture()
        {
            if (Xfer != null)
            {
                Xfer.Grab();
                CameraCapturingStatus(true);
            }
        }

        public void StopCapture()
        {
            try
            {
                if (Xfer != null)
                {
                    Xfer.Freeze();
                    CameraCapturingStatus(false);
                }
            }
            catch (Exception) { }
        }

        private void xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            if (args.Trash) return;

            IntPtr addr;
            Buffers.GetAddress(out addr);

            int size = Buffers.Width * Buffers.Height * (Buffers.PixelDepth / 8);

            ImageDataCaptured(addr, (uint)size, Buffers.Width, Buffers.Height);
        }

        static void DestroysObjects(SapAcquisition acq, SapFeature feature, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer)
        {
            if (xfer != null)
            {
                xfer.Destroy();
                xfer.Dispose();
            }

            if (buf != null)
            {
                buf.Destroy();
                buf.Dispose();
            }

            if (acq != null)
            {
                acq.Destroy();
                acq.Dispose();
            }

            if (feature != null)
            {
                feature.Destroy();
                feature.Dispose();
            }

            if (camera != null)
            {
                camera.Destroy();
                camera.Dispose();
            }
        }

        protected void CameraConnectionStatus(bool status)
        {
            IsConnected = status;
            CameraConnectionStatusHandler?.Invoke(status);
        }
        protected void CameraCapturingStatus(bool status)
        {
            IsCapturing = status;
            CameraCapturingStatusHandler?.Invoke(status);
        }
        protected void ImageDataCaptured(IntPtr pData, uint len, int width, int height)
        {
            CameraCapuredDataHandler?.Invoke(pData, len, width, height);
        }

        public const string FeatureNameGain = "Gain";
        public const string FeatureNameExposureTime = "ExposureTime";

        public double GetFeatureValueDouble(string name)
        {
            double value;
            if (!AcqDevice.GetFeatureValue(name, out value))
                throw new Exception("获取参数值错误");
            return value;
        }

        public void SetFeatureValueDouble(string name, double value)
        {
            AcqDevice.SetFeatureValue(name, value);
        }
    }
}
