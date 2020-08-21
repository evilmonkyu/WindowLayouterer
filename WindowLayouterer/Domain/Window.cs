using System;
using System.Collections.Generic;
using System.Text;

namespace WindowLayouterer.Domain
{
    public class Window
    {
        public IntPtr Handle { get; set; }
        public string Name { get; set; }
        public string ProcessName { get; set; }
        public uint ProcessId { get; set; }
    }
}
