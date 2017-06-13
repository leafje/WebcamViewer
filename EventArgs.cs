using DirectShowLib;

namespace WebcamViewer
{
    public class ValueEventArgs : System.EventArgs
    {
        public CameraControlProperty PropertyName { get; set; }
        public int Value { get; set; }

        public ValueEventArgs(CameraControlProperty p, int v)
        {
            this.PropertyName = p;
            this.Value = v;
        }
    }

    public class AutoEventArgs : System.EventArgs
    {
        public CameraControlProperty PropertyName { get; set; }
        public bool Value { get; set; }

        public AutoEventArgs(CameraControlProperty p, bool v)
        {
            this.PropertyName = p;
            this.Value = v;
        }
    }


}
