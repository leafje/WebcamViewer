using DirectShowLib;
using System.Text;

namespace WebcamViewer
{
    public class CameraProperty
    {
        public CameraControlProperty Name { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Step { get; set; }
        public CameraControlFlags AutoFlag { get; set; }
        public CameraControlFlags PossibleAutoFlag { get; set; }
        public int CurrentValue { get; set; }
        public int DefaultValue { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Property name: {0}", this.Name.ToString());
            sb.AppendLine();
            sb.AppendFormat("Available auto settings: {0}", this.PossibleAutoFlag);
            sb.AppendLine();
            sb.AppendFormat("Is automatically set: {0}", this.AutoFlag);
            sb.AppendLine();
            sb.AppendFormat("Min value: {0}", this.Min);
            sb.AppendLine();
            sb.AppendFormat("Max value: {0}", this.Max);
            sb.AppendLine();
            sb.AppendFormat("Step: {0}", this.Step);
            sb.AppendLine();
            sb.AppendFormat("Current value: {0}", this.CurrentValue);
            sb.AppendLine();
            sb.AppendFormat("Default value: {0}", this.DefaultValue);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
