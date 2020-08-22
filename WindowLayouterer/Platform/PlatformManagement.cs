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

        private Dictionary<HotKey, int> HotKeys = new Dictionary<HotKey, int>();

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

        public void RegisterHotKey(HotKey hotKey)
        {
            int id = 0;
            var existing = HotKeys.Keys.SingleOrDefault(k => k.Key == hotKey.Key && k.Modifiers == hotKey.Modifiers);
            if (existing == null)
            {
                while (HotKeys.ContainsValue(id))
                    id++;
                HotKeys.Add(hotKey, id);
            }
            PlatformInterface.RegisterHotKey(MainWindow.Handle, id, hotKey.Modifiers, hotKey.Key);
        }

        public void UnregisterHotKey(HotKey hotKey)
        {
            var existing = HotKeys.Keys.SingleOrDefault(k => k.Key == hotKey.Key && k.Modifiers == hotKey.Modifiers);
            if (existing != null)
            {
                PlatformInterface.UnregisterHotKey(MainWindow.Handle, HotKeys[existing]);
                HotKeys.Remove(existing);
            }
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
