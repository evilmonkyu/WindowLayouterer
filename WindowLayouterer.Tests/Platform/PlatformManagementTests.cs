using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Windows.Forms;
using WindowLayouterer.Domain;
using WindowLayouterer.Platform;
using WindowLayouterer.UserInterface;

namespace WindowLayouterer.Tests.Platform
{
    [TestClass]
    public class PlatformManagementTests
    {
        private PlatformInterface PlatformInterface;
        private MainWindow MainWindow;
        private PlatformManagement PlatformManagement;

        [TestInitialize]
        public void Setup()
        {
            PlatformInterface = Substitute.For<PlatformInterface>();
            MainWindow = Substitute.For<MainWindow>();
            PlatformManagement = new PlatformManagement(PlatformInterface, MainWindow);
        }

        [TestMethod]
        public void CanGetAllVisibleWindows()
        {
            var windows = new[]
            {
                new { ptr = new IntPtr(1), name = "1", pname = "x", pid = 30u, style = WindowStyles.WS_SIZEBOX | WindowStyles.WS_MINIMIZE },
                new { ptr = new IntPtr(2), name = "2", pname = "y", pid = 31u, style = WindowStyles.WS_POPUPWINDOW | WindowStyles.WS_MINIMIZE },
                new { ptr = new IntPtr(3), name = "", pname = "z", pid = 32u, style = WindowStyles.WS_SIZEBOX | WindowStyles.WS_MINIMIZE }
            };
            PlatformInterface.EnumDesktopWindows(default, null, default).ReturnsForAnyArgs(x => 
            {
                windows.Select(y => (x[1] as EnumDesktopWindowsDelegate)(y.ptr, ((IntPtr)x[2]).ToInt32())).ToList();
                return true;
            });
            PlatformInterface.IsWindowVisible(default).ReturnsForAnyArgs(x => { return (IntPtr)x[0] != windows[2].ptr; });
            PlatformInterface.GetWindowText(default, null, 0).ReturnsForAnyArgs(x => 
            {
                var w = windows.Single(y => y.ptr == (IntPtr)x[0]);
                ((StringBuilder)x[1]).Append(w.name);
                return w.name.Length;
            });
            var info = default (WINDOWINFO);
            PlatformInterface.GetWindowInfo(default, ref info).ReturnsForAnyArgs(x =>
            {                
                x[1] = new WINDOWINFO
                {
                    dwStyle = (uint)windows.Single(y => y.ptr == (IntPtr)x[0]).style
                };
                return true;
            });
            uint procId;
            PlatformInterface.GetWindowThreadProcessId(default, out procId).ReturnsForAnyArgs(x =>
            {
                x[1] = windows.Single(y => y.ptr == (IntPtr)x[0]).pid;
                return 0u;
            });
            PlatformInterface.GetProcessName(0).ReturnsForAnyArgs(x => windows.Single(y => y.pid == (uint)x[0]).pname);            
            var result = PlatformManagement.GetAllVisibleWindows();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(windows[0].ptr, result[0].Handle);
            Assert.AreEqual(windows[0].name, result[0].Title);
            Assert.AreEqual(windows[0].pname, result[0].ProcessName);
            Assert.AreEqual(windows[0].pid, result[0].ProcessId);
        }

        [TestMethod]
        public void CanGetForegroundWindow()
        {
            var window = new { ptr = new IntPtr(1), name = "1", pname = "x", pid = 30u, style = WindowStyles.WS_SIZEBOX | WindowStyles.WS_MINIMIZE };
            PlatformInterface.GetForegroundWindow().Returns(window.ptr);
            PlatformInterface.GetWindowText(default, null, 0).ReturnsForAnyArgs(x =>
            {
                ((StringBuilder)x[1]).Append(window.name);
                return window.name.Length;
            });
            var info = default(WINDOWINFO);
            PlatformInterface.GetWindowInfo(default, ref info).ReturnsForAnyArgs(x =>
            {
                x[1] = new WINDOWINFO
                {
                    dwStyle = (uint)window.style
                };
                return true;
            });
            uint procId;
            PlatformInterface.GetWindowThreadProcessId(default, out procId).ReturnsForAnyArgs(x =>
            {
                x[1] = window.pid;
                return 0u;
            });
            PlatformInterface.GetProcessName(0).ReturnsForAnyArgs(x => window.pname);
            var result = PlatformManagement.GetForegroundWindow();
            Assert.AreEqual(window.ptr, result.Handle);
            Assert.AreEqual(window.name, result.Title);
            Assert.AreEqual(window.pname, result.ProcessName);
            Assert.AreEqual(window.pid, result.ProcessId);
        }

        [TestMethod]
        public void CanResizeWindows()
        {
            var windowArgs = new[]
            {
                new Window { Handle = new IntPtr(1), x = 100, y = 200, cx = 450, cy = 350 },
                new Window { Handle = new IntPtr(2), x = 800, y = 900, cx = 250, cy = 150 }
            };
            var hWinPosInfo = new IntPtr();
            PlatformInterface.BeginDeferWindowPos(2).Returns(hWinPosInfo);
            PlatformInterface.DeferWindowPos(default, default, default, 0, 0, 0, 0, 0).ReturnsForAnyArgs(hWinPosInfo);
            PlatformInterface.EndDeferWindowPos(hWinPosInfo).Returns(true);
            PlatformManagement.ResizeWindows(windowArgs.ToList());
            PlatformInterface.Received().DeferWindowPos(Arg.Is(hWinPosInfo), Arg.Is(windowArgs[0].Handle), Arg.Any<IntPtr>(), windowArgs[0].x, windowArgs[0].y, 
                windowArgs[0].cx, windowArgs[0].cy, (uint)(DeferWindowPosCommands.SWP_NOZORDER & DeferWindowPosCommands.SWP_NOACTIVATE));
            PlatformInterface.Received().DeferWindowPos(Arg.Is(hWinPosInfo), Arg.Is(windowArgs[1].Handle), Arg.Any<IntPtr>(), windowArgs[1].x, windowArgs[1].y,
                windowArgs[1].cx, windowArgs[1].cy, (uint)(DeferWindowPosCommands.SWP_NOZORDER & DeferWindowPosCommands.SWP_NOACTIVATE));
            PlatformInterface.Received().EndDeferWindowPos(hWinPosInfo);
        }

        [TestMethod]
        public void CanRegisterHotkey()
        {
            var ptr = new IntPtr(1);
            MainWindow.Handle.Returns(ptr);
            PlatformInterface.RegisterHotKey(default, 0, default, default).ReturnsForAnyArgs(true);
            PlatformManagement.RegisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.X });
            PlatformManagement.RegisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.X });
            PlatformManagement.RegisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.Y });
            PlatformInterface.Received(2).RegisterHotKey(ptr, 0, KeyModifiers.Alt, Keys.X);
            PlatformInterface.Received(1).RegisterHotKey(ptr, 1, KeyModifiers.Alt, Keys.Y);
        }

        [TestMethod]
        public void CanUnregisterHotkey()
        {
            var ptr = new IntPtr(1);
            MainWindow.Handle.Returns(ptr);
            PlatformInterface.RegisterHotKey(IntPtr.Zero, 0, default, default).Returns(true);
            PlatformManagement.RegisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.X });
            PlatformManagement.RegisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.Y });
            PlatformManagement.UnregisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.X });
            PlatformManagement.UnregisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.Y });
            PlatformManagement.UnregisterHotKey(new HotKey { Modifiers = KeyModifiers.Alt, Key = Keys.Z });
            PlatformInterface.Received().UnregisterHotKey(ptr, 0);
            PlatformInterface.Received().UnregisterHotKey(ptr, 1);
            PlatformInterface.ReceivedWithAnyArgs(2).UnregisterHotKey(default, 0);
        }
    }
}
