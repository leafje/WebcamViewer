using System;
using System.Collections.Generic;

namespace WebcamViewer
{
    static class CameraLogger
    {
        public static void Main(string[] args)
        {
            CameraLogger.Log();
            Console.ReadLine();
        }

        public static void Log()
        {
            List<string> names = VideoStream.GetDevices();
            foreach (string name in names)
            {
                Camera c = VideoStream.SetDevice(name);
                Console.WriteLine(String.Format("Device: {0}", name));
                Console.WriteLine("----------------------");
                foreach (CameraProperty info in c.GetCameraInfo())
                {
                    Console.WriteLine(info.ToString());
                }
                foreach (VideoStreamInfo resolution in c.GetResolutions())
                {
                    Console.WriteLine(resolution.ToLogString());
                }
            }
            Console.WriteLine();
        }
    }
}
