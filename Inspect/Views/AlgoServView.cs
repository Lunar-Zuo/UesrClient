using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inspect.Views
{
    public delegate void StartAlgoServDelegate(string port);
    public partial class AlgoServView : UserControl
    {
        public event StartAlgoServDelegate StartAlgoServHandler;

        private int id = 0;
        public int _Id
        {
            get { return id; }
            set { id = value; }
        }
        public string _Name
        {
            get { return labelName.Text; }
            set { labelName.Text = value; }
        }

        public string _Port
        {
            get { return labelPort.Text; }
            set { labelPort.Text = value; }
        }

        private bool status = false;
        public bool _Status
        {
            get { return status; }
            set
            {
                if (status == value) return;
                status = value;
                SetStatus(status);
            }
        }

        public AlgoServView()
        {
            InitializeComponent();
        }

        public void SetStatus(bool on)
        {
            Action act = (() =>
            {
                if (on)
                {
                    labelStatus.BackColor = Color.SeaGreen;
                    labelStatus.Text = "连接";
                }
                else
                {
                    labelStatus.BackColor = Color.Red;
                    labelStatus.Text = "断开";
                    //在断开时自启动---2022.9.11
                    StartAlgoServHandler?.Invoke(_Port);
                    btnStart.Enabled = false;
                }
                btnStart.Enabled = !on;
            });
            if (labelStatus.InvokeRequired) labelStatus.Invoke(act);
            else act();
        }

        private void AlgoServView_Load(object sender, EventArgs e)
        {
            SetStatus(false);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartAlgoServHandler?.Invoke(_Port);
            btnStart.Enabled = false;
        }
    }
}
