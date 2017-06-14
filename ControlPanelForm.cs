using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DirectShowLib;

namespace WebcamViewer
{
    public partial class ControlPanelForm : Form
    {
        private Camera camera;

        public ControlPanelForm()
        {
            InitializeComponent();
        }

        public void SetCamera(Camera c)
        {
            this.camera = c;
            Panel controlPanelHolder = new Panel();
            controlPanelHolder.AutoSize = true;
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

                if ((prop.PossibleAutoFlag & CameraControlFlags.Auto) == CameraControlFlags.Auto)
                {
                    cp.EnableAuto();
                }

                if ((prop.AutoFlag & CameraControlFlags.Auto) == CameraControlFlags.Auto)
                {
                    cp.Auto = true;
                }
                else if ((prop.AutoFlag & CameraControlFlags.Manual) == CameraControlFlags.Manual)
                {
                    cp.Auto = false;
                }

                cp.Step = prop.Step;
                cp.Min = prop.Min;
                cp.Max = prop.Max;
                cp.PropertyType = prop.Name;
                cp.CurrentValue = prop.CurrentValue;
                cp.Location = new Point(0, yCoord);
                yCoord += cp.Size.Height;
                cp.ValueChangedEvent += this.ValueChanged;
                cp.AutoChangedEvent += this.AutoChanged;
                cp.PerformLayout();
                controlPanelHolder.Controls.Add(cp);
            }
            controlPanelHolder.PerformLayout();

            this.Controls.Add(controlPanelHolder);
            this.ResumeLayout(false);
            this.PerformLayout();
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
