using System;
using System.Text;

namespace WebcamViewer
{
    class VideoStreamInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public short BitCount { get; set; }

        public override string ToString()
        {
            return String.Format("{0}x{1}", this.Width, this.Height);
        }

        public string ToLogString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Width: {0}", this.Width);
            sb.AppendLine();
            sb.AppendFormat("Height: {0}", this.Height);
            sb.AppendLine();
            sb.AppendFormat("Size: {0}", this.Size);
            sb.AppendLine();
            sb.AppendFormat("Bit Count: {0}", this.BitCount);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
