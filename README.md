The Webcam Viewer allows you to control and view the live feed captured by a camera connected to a PC.

This program should work with any webcam (or other USB camera) that can be recognized using DirectShow APIs. Many cameras use the built-in Microsoft USB camera driver, which exposes the functionality of the camera in a DirectShow compatible way.

This is a very early release and is still a work in progress. The code does not check for invalid states or errors in many cases. 

Feedback, issues, and pull requests are welcomed.

**Dependencies**
- .NET Framework 2 or higher
- DirectShow.NET (http://directshownet.sourceforge.net/) You may need to alter the project file to point at the correct location of the DirectShowLib-2005.dll file.
- Visual Studio 2017 - there isn't anything VS 2017 specific in the code or project files, I just haven't tried loading them in any other VS version.

**License**

MIT

**TODO**
- The code does not handle desktop scaling well. For example, I have tested this software on a high DPI laptop display with a scaling factor of 250% - the screen is 3200x1800 pixels, so it has an effective resolution of 1280x720. The webcam display is also scaled by 250%, leading to a very pixellated display when selecting higher resolutions.
- Before adding too many more features, migrate the UI to WPF. I think in the long run WPF will be a more robust platform for building the UI, but Windows Forms works well enough for a quick prototype.