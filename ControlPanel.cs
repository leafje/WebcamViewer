using DirectShowLib;
using System;
using System.Windows.Forms;

namespace WebcamViewer
{
    public partial class ControlPanel : UserControl
    {
        private string pName = "";
        private CameraControlProperty propType;

        public delegate void ValueChangedEventHandler(object sender, ValueEventArgs e);
        public delegate void AutoChangedEventHandler(object sender, AutoEventArgs e);

        public event ValueChangedEventHandler ValueChangedEvent;
        public event AutoChangedEventHandler AutoChangedEvent;

        public ControlPanel()
        {
            InitializeComponent();
        }

        public string PropertyName
        {
            set {
                this.pName = value;
                this.labelName.Text = value;
            }
        }

        public CameraControlProperty PropertyType
        {
            set
            {
                this.pName = value.ToString();
                this.labelName.Text = value.ToString();
                this.propType = value;
            }
        }

        public bool Auto
        {
            get { return this.checkBoxAuto.Checked; }
            set { this.checkBoxAuto.Checked = value; }
        }
        public int Min
        {
            get { return this.trackBarValue.Minimum; }
            set { this.trackBarValue.Minimum = value; }
        }
        public int Max
        {
            get { return this.trackBarValue.Maximum; }
            set { this.trackBarValue.Maximum = value; }
        }
        public int Step
        {
            get { return this.trackBarValue.SmallChange;  }
            set {
                this.trackBarValue.SmallChange = value;
                this.trackBarValue.LargeChange = value;
            }
        }
        public int CurrentValue
        {
            get { return this.trackBarValue.Value; }
            set { this.trackBarValue.Value = value; }
        }

        private void trackBarValue_ValueChanged(object sender, EventArgs e)
        {
            this.labelName.Text = String.Format("{0} ({1})", this.pName, this.trackBarValue.Value);
            if (this.ValueChangedEvent != null)
            {
                this.ValueChangedEvent(this, new ValueEventArgs(this.propType, this.trackBarValue.Value));
            }
        }

        private void checkBoxAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAuto.Checked)
            {
                this.trackBarValue.Enabled = false;
                this.labelName.Text = this.pName;
            }
            else
            {
                this.trackBarValue.Enabled = true;
            }

            if (this.AutoChangedEvent != null)
            {
                this.AutoChangedEvent(this, new AutoEventArgs(this.propType, this.checkBoxAuto.Checked));
            }
        }
    }
}
