﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WindowLayouterer.Domain
{
    public class Window
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public string ProcessName { get; set; }
        public uint ProcessId { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int cx { get; set; }
        public int cy { get; set; }

    }
}
