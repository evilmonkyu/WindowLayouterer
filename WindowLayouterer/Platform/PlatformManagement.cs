using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowLayouterer.Domain;

namespace WindowLayouterer.Platform
{
    public class PlatformManagement
    {
        private PlatformInterface PlatformInterface;

        public PlatformManagement(PlatformInterface platformInterface)
        {
            PlatformInterface = platformInterface;
        }

        public List<Window> GetAllVisibleWindows()
        {
            var handles = new List<IntPtr>();
            EnumDesktopWindowsDelegate callback = (hWnd, lParam) =>
            {
                handles.Add(hWnd);
                return true;
            };
            PlatformInterface.EnumDesktopWindows(IntPtr.Zero, callback, IntPtr.Zero);
            return handles.Where(h =>
            {
                if (!PlatformInterface.IsWindowVisible(h)) 
                    return false;                
                var pwi = default(WINDOWINFO);
                PlatformInterface.GetWindowInfo(h, ref pwi);
                return (pwi.dwStyle & (uint)WindowStyles.WS_SIZEBOX) != 0;
            })
            .Select(h =>
            {
                var window = new Window { Handle = h };
                var sb = new StringBuilder(1024);
                PlatformInterface.GetWindowText(h, sb, 1024);
                window.Name = sb.ToString();
                uint pid;
                PlatformInterface.GetWindowThreadProcessId(h, out pid);
                window.ProcessId = pid;
                window.ProcessName = PlatformInterface.GetProcessName(pid);
                return window;
            }).ToList();
        }
    }
}
