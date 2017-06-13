using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WebcamViewer
{
    // a small enum to record the graph state
    enum CameraStreamState
    {
        Stopped,
        Paused,
        Running,
        NotInitialized
    };

    class Camera
    {
        private IBaseFilter sourceFilter = null;
        private DsDevice dev = null;
        private IFilterGraph2 filterGraph = null;
        private ICaptureGraphBuilder2 captureGraphBuilder = null;
        private IMediaControl mediaControl = null;
        private IVideoWindow videoWindow = null;
        private IMediaEventEx mediaEventEx = null;
        private IAMStreamConfig streamConfig = null;

        private CameraStreamState state = CameraStreamState.NotInitialized;

        public Camera(DsDevice d)
        {
            this.dev = d;
            filterGraph = new FilterGraph() as IFilterGraph2;
            int hr = filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out this.sourceFilter);
            DsError.ThrowExceptionForHR(hr);

            this.captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            this.mediaControl = (IMediaControl)this.filterGraph;
            this.videoWindow = (IVideoWindow)this.filterGraph;
            this.mediaEventEx = (IMediaEventEx)this.filterGraph;

        }

        #region Cleanup

        ~Camera()
        {
            CloseInterfaces();
        }

        private void CloseInterface(object o)
        {
            if (o != null)
            {
                Marshal.ReleaseComObject(o);
                o = null;
            }
        }

        private void CloseInterfaces()
        {
            // Stop previewing data
            if (this.mediaControl != null)
            {
                this.mediaControl.StopWhenReady();
            }

            this.state = CameraStreamState.NotInitialized;

            // Stop receiving events
            if (this.mediaEventEx != null)
            {
                this.mediaEventEx.SetNotifyWindow(IntPtr.Zero, VideoStream.WM_GRAPHNOTIFY, IntPtr.Zero);
            }

            // Relinquish ownership (IMPORTANT!) of the video window.
            // Failing to call put_Owner can lead to assert failures within
            // the video renderer, as it still assumes that it has a valid
            // parent window.
            if (this.videoWindow != null)
            {
                this.videoWindow.put_Visible(OABool.False);
                this.videoWindow.put_Owner(IntPtr.Zero);
            }

            this.CloseInterface(this.filterGraph);
            this.CloseInterface(this.sourceFilter);
            this.CloseInterface(this.captureGraphBuilder);
            this.CloseInterface(this.mediaControl);
            this.CloseInterface(this.videoWindow);
            this.CloseInterface(this.mediaEventEx);

            GC.Collect();
        }

        #endregion

        public void SetResolution(VideoStreamInfo info)
        {
            if (this.streamConfig == null)
            {
                var pRaw2 = DsFindPin.ByCategory(this.sourceFilter, PinCategory.Capture, 0);
                this.streamConfig = (IAMStreamConfig)pRaw2;
            }

            AMMediaType mediaType;
            int hr = this.streamConfig.GetFormat(out mediaType);
            DsError.ThrowExceptionForHR(hr);

            VideoInfoHeader v = new VideoInfoHeader();
            Marshal.PtrToStructure(mediaType.formatPtr, v);
            v.BmiHeader.BitCount = info.BitCount;
            v.BmiHeader.Height = info.Height;
            v.BmiHeader.Width = info.Width;
            v.BmiHeader.Size = info.Size;
            Marshal.StructureToPtr(v, mediaType.formatPtr, true);
            this.streamConfig.SetFormat(mediaType);
        }

        public List<VideoStreamInfo> GetResolutions()
        {
            var pRaw2 = DsFindPin.ByCategory(this.sourceFilter, PinCategory.Capture, 0);
            this.streamConfig = (IAMStreamConfig)pRaw2;
            var availableResolutions = new List<VideoStreamInfo>();

            VideoInfoHeader v = new VideoInfoHeader();
            IEnumMediaTypes mediaTypeEnum;
            int hr = pRaw2.EnumMediaTypes(out mediaTypeEnum);


            AMMediaType[] mediaTypes = new AMMediaType[1];
            IntPtr fetched = IntPtr.Zero;
            hr = mediaTypeEnum.Next(1, mediaTypes, fetched);

            while (fetched != null && mediaTypes[0] != null)
            {
                Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                if (v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                {
                    VideoStreamInfo info = new VideoStreamInfo();
                    info.Width = v.BmiHeader.Width;
                    info.Height = v.BmiHeader.Height;
                    info.Size = v.BmiHeader.Size;
                    info.BitCount = v.BmiHeader.BitCount;
                    availableResolutions.Add(info);
                }
                hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
            }
            return availableResolutions;
        }

        public List<CameraProperty> GetCameraInfo()
        {
            List<CameraProperty> settings = new List<CameraProperty>();
            IAMCameraControl cc = (IAMCameraControl)this.sourceFilter;
            foreach (CameraControlProperty prop in Enum.GetValues(typeof(CameraControlProperty)))
            {
                CameraProperty info = new CameraProperty();
                CameraControlFlags flags = new CameraControlFlags();
                int def = 0, min = 0, max = 0, step = 0, val = 0;
                cc.GetRange(prop, out min, out max, out step, out def, out flags);
                info.Name = prop;
                info.Min = min;
                info.Max = max;
                info.Step = step;
                info.DefaultValue = def;
                info.AutoFlag = flags;

                cc.Get(prop, out val, out flags);
                info.CurrentValue = val;
                settings.Add(info);
            }

            return settings;
        }

        public int GetProperty(CameraControlProperty prop)
        {
            IAMCameraControl cc = (IAMCameraControl)this.sourceFilter;
            int val;
            CameraControlFlags flags = new CameraControlFlags();
            cc.Get(prop, out val, out flags);
            return val;
        }

        public void SetProperty(CameraControlProperty prop, CameraControlFlags flags, int val = 0)
        {
            IAMCameraControl cc = (IAMCameraControl)this.sourceFilter;
            int hr = cc.Set(prop, val, flags);
        }

        public void InitializeStream(IntPtr handle)
        {
            int hr = this.captureGraphBuilder.SetFiltergraph(this.filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Add Capture filter to our graph.
            hr = this.filterGraph.AddFilter(this.sourceFilter, "Video Capture");
            DsError.ThrowExceptionForHR(hr);

            // Render the preview pin on the video capture filter
            // Use this instead of this.graphBuilder.RenderFile
            hr = this.captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, this.sourceFilter, null, null);
            DsError.ThrowExceptionForHR(hr);

            hr = this.mediaEventEx.SetNotifyWindow(handle, VideoStream.WM_GRAPHNOTIFY, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            // Set the video window to be a child of the main window
            hr = this.videoWindow.put_Owner(handle);
            DsError.ThrowExceptionForHR(hr);

            hr = this.videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
            DsError.ThrowExceptionForHR(hr);

            // Make the video window visible, now that it is properly positioned
            hr = this.videoWindow.put_Visible(OABool.True);
            DsError.ThrowExceptionForHR(hr);

            this.StartVideo();
        }

        public void ResizeVideoWindow(Size size)
        {
            // Resize the video preview window to match owner window size
            if (this.videoWindow != null)
            {
                int hr = this.videoWindow.SetWindowPosition(0, 0, size.Width, size.Height);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public void StopVideo()
        {
            if (this.state == CameraStreamState.Running)
            {
                int hr = this.mediaControl.StopWhenReady();
                DsError.ThrowExceptionForHR(hr);
                this.state = CameraStreamState.Paused;
            }
        }

        public void StartVideo()
        {
            if (this.state != CameraStreamState.Running)
            {
                int hr = this.mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);
                this.state = CameraStreamState.Running;
            }
        }

        public void SystemMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (this.videoWindow != null)
            {
                this.videoWindow.NotifyOwnerMessage(hwnd, msg, wParam, lParam);
            }
        }
    }
}
