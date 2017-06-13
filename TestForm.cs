using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WebcamViewer
{
    public partial class TestForm : Form
    {
        private Camera camera = null;

        public TestForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            // TODO there is a memory leak here - I should be freeing memory for events I care
            // about. but I don't care.
            if (m.Msg == VideoStream.WM_GRAPHNOTIFY)
            {
                Debug.WriteLine(String.Format("Received Windows event message: {0}", m.Msg));
            }
            if (this.camera != null)
            {
                this.camera.SystemMessage(m.HWnd, m.Msg, m.WParam, m.LParam);
            }

            base.WndProc(ref m);
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            List<string> names = VideoStream.GetDevices();
            this.comboBoxCamera.Items.AddRange(names.ToArray());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.comboBoxCamera.SelectedIndex;
            string deviceName = (string)this.comboBoxCamera.Items[index];
            this.camera = VideoStream.SetDevice(deviceName);
            this.camera.InitializeStream(this.panelVideo.Handle);
            
            // Now, populate the resolution list.
            List<VideoStreamInfo> resolutions = this.camera.GetResolutions();
            this.comboBoxResolution.Items.Clear();
            this.comboBoxResolution.Items.AddRange(resolutions.ToArray());
            this.comboBoxResolution.SelectedIndex = 2;
            //this.panel1.ClientSize = new Size(resolutions[2].Width, resolutions[2].Height);
            //this.camera.ResizeVideoWindow(this.panel1.ClientSize);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            VideoStreamInfo selectedResolution = (VideoStreamInfo)this.comboBoxResolution.SelectedItem;
            this.camera.SetResolution(selectedResolution);
            //this.panel1.ClientSize = new Size(selectedResolution.Width, selectedResolution.Height);
            this.ClientSize = new Size(System.Math.Max(1225, selectedResolution.Width) + this.panelVideo.Margin.Horizontal,
                selectedResolution.Height + this.panelSettings.Height + this.panelVideo.Margin.Vertical);
            this.panelVideo.Width = selectedResolution.Width;
            this.panelVideo.Height = selectedResolution.Height;

            this.camera.ResizeVideoWindow(this.panelVideo.ClientSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TestForm_ResizeBegin(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }

        private void TestForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }

        private void buttonFullScreen_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void buttonControlPanel_Click(object sender, EventArgs e)
        {
            Panel controlPanel = new Panel();
            controlPanel.AutoSize = true;
            int yCoord = 0;

            List<CameraProperty> properties = this.camera.GetCameraInfo();
            foreach (CameraProperty prop in properties)
            {
                // Skip properties that aren't user controllable.
                if (prop.AutoFlag == CameraControlFlags.None)
                {
                    continue;
                }

                ControlPanel cp = new ControlPanel();
                cp.PropertyName = prop.Name.ToString();
                
                if (prop.AutoFlag == CameraControlFlags.Auto)
                {
                    cp.Auto = true;
                }
                else if (prop.AutoFlag == CameraControlFlags.Manual)
                {
                    cp.Auto = false;
                }

                cp.Step = prop.Step;
                cp.Min = prop.Min;
                cp.Max = prop.Max;
                cp.CurrentValue = prop.CurrentValue;
                cp.PropertyType = prop.Name;
                cp.Location = new Point(0, yCoord);
                yCoord += cp.Size.Height;
                cp.ValueChangedEvent += this.ValueChanged;
                cp.AutoChangedEvent += this.AutoChanged;
                controlPanel.Controls.Add(cp);
            }
            controlPanel.PerformLayout();

            Form f = new Form();
            f.AutoSize = true;
            f.Controls.Add(controlPanel);
            f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            f.TopMost = true;
            f.ResumeLayout(false);
            f.PerformLayout();

            f.Show();
        }

        public void ValueChanged(object o, ValueEventArgs e)
        {
            this.camera.SetProperty(e.PropertyName, CameraControlFlags.None, e.Value);
        }

        public void AutoChanged(object o, AutoEventArgs e)
        {
            if (e.Value)
            {
                this.camera.SetProperty(e.PropertyName, CameraControlFlags.Auto);
            }
            else
            {
                this.camera.SetProperty(e.PropertyName, CameraControlFlags.Manual);
                ControlPanel cp = (ControlPanel)o;
                cp.CurrentValue = this.camera.GetProperty(e.PropertyName);
            }
        }
    }
}
