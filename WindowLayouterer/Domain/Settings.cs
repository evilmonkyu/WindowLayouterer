using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WindowLayouterer.Domain
{
    public class Settings
    {
        public List<LayoutSetting> Layouts { get; set; }
    }

    public class WindowSetting
    {
        public string Name { get; set; }
        public string Process { get; set; }
        public string Title { get; set; }
    }

    public class ScreenAreaSetting
    {
        public string Name { get; set; }
        public HotKey HotKey { get; set; }
        public List<WindowSetting> Windows { get; set; }
        public decimal Left { get; set; }
        public decimal Top { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }

    public class LayoutSetting 
    {
        public string Name { get; set; }
        public HotKey HotKey { get; set; }
        public bool IsDefault { get; set; }
        public List<ScreenAreaSetting> ScreenAreas { get; set; }
    }
}
