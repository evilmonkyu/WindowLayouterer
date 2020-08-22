using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowLayouterer.Domain;
using WindowLayouterer.UserInterface;

namespace WindowLayouterer.Platform
{
    public class PlatformManagement
    {
        private PlatformInterface PlatformInterface;
        private MainWindow MainWindow;

        private Dictionary<int, Hotkey> Hotkeys = new Dictionary<int, Hotkey>();

        public PlatformManagement(PlatformInterface platformInterface, MainWindow mainWindow)
        {
            PlatformInterface = platformInterface;
            MainWindow = mainWindow;
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
            .Select(GetWindow).ToList();
        }

        public Window GetForegroundWindow() => GetWindow(PlatformInterface.GetForegroundWindow());        

        public void ResizeWindows(List<Window> windows)
        {
            var posInfo = PlatformInterface.BeginDeferWindowPos(windows.Count);
            windows.Select(w => PlatformInterface.DeferWindowPos(posInfo, w.Handle, new IntPtr((int)DeferWindowPosInsertAfter.HWND_TOP),
                    w.x, w.y, w.cx, w.cy, (uint)(DeferWindowPosCommands.SWP_NOZORDER & DeferWindowPosCommands.SWP_NOACTIVATE))).ToList();
            PlatformInterface.EndDeferWindowPos(posInfo);
        }

        public void RegisterHotKey(Hotkey hotkey)
        {
            int id = 0;
            var existing = Hotkeys.Values.SingleOrDefault(k => k.Key == hotkey.Key && k.Modifiers == hotkey.Modifiers);
            if (existing == null)
            {
                while (Hotkeys.ContainsKey(id))
                    id++;
                Hotkeys.Add(id, hotkey);
            }
            PlatformInterface.RegisterHotKey(MainWindow.Handle, id, hotkey.Modifiers, hotkey.Key);
        }

        private Window GetWindow(IntPtr handle)
        {
            var window = new Window { Handle = handle };
            var sb = new StringBuilder(1024);
            PlatformInterface.GetWindowText(handle, sb, 1024);
            window.Name = sb.ToString();
            uint pid;
            PlatformInterface.GetWindowThreadProcessId(handle, out pid);
            window.ProcessId = pid;
            window.ProcessName = PlatformInterface.GetProcessName(pid);
            return window;
        }
    }
}
