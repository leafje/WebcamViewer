using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace WebcamViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> names = VideoStream.GetDevices();
            this.toolStripComboBoxDevices.Items.AddRange(names.ToArray());
        }

        private void toolStripComboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index = this.toolStripComboBoxDevices.SelectedIndex;
            //string deviceName = (string)this.toolStripComboBoxDevices.Items[index];
            //VideoStream.SetDevice(deviceName);
            //List<VideoStreamInfo> resolutions = VideoStream.GetResolutions();
            //this.toolStripComboBoxResolutions.Items.Clear();
            //this.toolStripComboBoxResolutions.Items.AddRange(resolutions.ToArray());
            //this.toolStripComboBoxResolutions.SelectedIndex = 0;
            //this.panel1.Width = resolutions[0].Width;
            //this.panel1.Height = resolutions[0].Height;

            //VideoStream.InitializeStream(this.panel1.Handle);
            //VideoStream.ResizeVideo(this.panel1.Size);
        }

        protected override void WndProc(ref Message m)
        {
            // TODO there is a memory leak here - I should be freeing memory for events I care
            // about. but I don't care.
            if (m.Msg == VideoStream.WM_GRAPHNOTIFY)
            {
                Debug.WriteLine(String.Format("Received Windows event message: {0}", m.Msg));
            }
            //VideoStream.SystemMessage(m.HWnd, m.Msg, m.WParam, m.LParam);

            base.WndProc(ref m);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //// Stop graph when Form is iconic
            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    VideoStream.StopVideo();
            //}

            //// Restart Graph when window come back to normal state
            //if (this.WindowState == FormWindowState.Normal)
            //{
            //    VideoStream.StartVideo();
            //}

            //VideoStream.ResizeVideo(this.Size);
        }
    }
}
