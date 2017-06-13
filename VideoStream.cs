using DirectShowLib;
using System;
using System.Collections.Generic;

namespace WebcamViewer
{
    class VideoStream
    {
        static object deviceLock = new object();
        private static List<DsDevice> deviceCache;

        // Application-defined message to notify app of filtergraph events
        public static readonly int WM_GRAPHNOTIFY = 0x8000 + 1;

        public static List<string> GetDevices(bool invalidate = false)
        {
            lock (deviceLock)
            {
                if (invalidate || deviceCache == null)
                {
                    // Get the collection of video devices
                    DsDevice[] capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
                    deviceCache = new List<DsDevice>(capDevices);
                }
            }

            List<string> displayNames = new List<string>();
            foreach (DsDevice d in deviceCache)
            {
                displayNames.Add(d.Name);
            }

            return displayNames;
        }

        public static Camera SetDevice(string displayName)
        {
            lock (deviceLock)
            {
                if (deviceCache == null)
                {
                    // Haven't loaded the device list, so no point in continuing.
                    throw new Exception("todo");
                }

                foreach (DsDevice d in deviceCache)
                {
                    if (d.Name == displayName)
                    {
                        Camera c = new Camera(d);
                        return c;
                    }
                }
            }

            // None of the known devices has the given display name.
            throw new Exception("todo");
        }
    }
}
