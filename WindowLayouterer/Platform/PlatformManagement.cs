using System;
using System.Collections.Generic;
using System.Text;

namespace WindowLayouterer.Platform
{
    public class PlatformManagement
    {
        private PlatformInterface PlatformInterface;

        public PlatformManagement(PlatformInterface platformInterface)
        {
            PlatformInterface = platformInterface;
        }
    }
}
