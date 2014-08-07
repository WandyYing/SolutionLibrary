using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation
{
    class ScreenCaptureFactory
    {
        public static ScreenCapture Create()
        {
            return new ScreenCapture();
        }

    }
}
