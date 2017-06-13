The Webcam Viewer allows you to control and view the live feed captured by a camera connected to a PC.

This program should work with any webcam (or other USB camera) that can be recognized using DirectShow APIs. Many cameras use the built-in Microsoft USB camera driver, which exposes the functionality of the camera in a DirectShow compatible way.

This is a very early release and is still a work in progress. The code does not check for invalid states or errors in many cases. 

Feedback, issues, and pull requests are welcomed.

**Dependencies**
- .NET Framework 4 or higher
- DirectShow.NET (http://directshownet.sourceforge.net/)

**License**

MIT

**TODO**
- The code does not handle desktop scaling well. For example, I have tested this software on a high DPI laptop display with a scaling factor of 250%. The webcam display is also scaled by 250%, leading to a very pixellated display when selecting higher resolutions.
