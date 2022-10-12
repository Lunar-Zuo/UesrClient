using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspect.Commons
{
    public class CountUtils : IDisposable
    {
        public delegate void CountTimeoutDelegate(string id);
        public event CountTimeoutDelegate CountTimeoutHandler;

        private string strId;
        private int iCount;
        private System.Threading.Timer timer = null;
        public void Start(string id, int timeout)
        {
            strId = id;
            iCount = 0;
            timer = new System.Threading.Timer(OnTimeout, "", timeout * 1000, timeout * 1000);
        }

        public int CountPlusOne()
        {
            return Interlocked.Increment(ref iCount);
        }

        private void OnTimeout(object state)
        {
            timer.Dispose();
            CountTimeoutHandler?.Invoke(strId);
        }

        public void Dispose()
        {
            try { timer.Dispose(); } catch (Exception) { }
        }
    }
}
