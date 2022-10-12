using DALSA.SaperaLT.SapClassBasic;
using System;
using System.Collections.Generic;
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

        private SapAcquisition Acq = null;
        private SapAcqDevice AcqDevice = null;
        private SapFeature Feature = null;
        private SapBuffer Buffers = null;
        private SapTransfer Xfer = null;
        private SapLocation loc = null;
        private SapLocation loc2 = null;

        public string ConfigFileName { get; set; }
        public const string ServerName = "Xtium-CL_MX4_1";//相机硬件配置参数

        public void OpenDevice()
        {
            //TODO:???这个值是否会改变
            int ResourceIndex = 0;//相机硬件配置参数

            loc = new SapLocation(ServerName, ResourceIndex);

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

        public void CloseDevice()
        {
            if (IsCapturing) StopCapture();

            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer);
            loc.Dispose();
            loc2.Dispose();

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
