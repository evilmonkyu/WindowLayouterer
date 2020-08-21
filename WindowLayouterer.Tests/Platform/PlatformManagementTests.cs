using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Text;
using WindowLayouterer.Platform;

namespace WindowLayouterer.Tests.Platform
{
    [TestClass]
    public class PlatformManagementTests
    {
        private PlatformInterface PlatformInterface;
        private PlatformManagement PlatformManagement;

        [TestInitialize]
        public void Setup()
        {
            PlatformInterface = Substitute.For<PlatformInterface>();
            PlatformManagement = new PlatformManagement(PlatformInterface);
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
            PlatformInterface.EnumDesktopWindows(IntPtr.Zero, null, IntPtr.Zero).ReturnsForAnyArgs(x => 
            {
                windows.Select(y => (x[1] as EnumDesktopWindowsDelegate)(y.ptr, ((IntPtr)x[2]).ToInt32())).ToList();
                return true;
            });
            PlatformInterface.IsWindowVisible(IntPtr.Zero).ReturnsForAnyArgs(x => { return (IntPtr)x[0] != windows[2].ptr; });
            PlatformInterface.GetWindowText(IntPtr.Zero, null, 0).ReturnsForAnyArgs(x => 
            {
                var w = windows.Single(y => y.ptr == (IntPtr)x[0]);
                ((StringBuilder)x[1]).Append(w.name);
                return w.name.Length;
            });
            var info = default (WINDOWINFO);
            PlatformInterface.GetWindowInfo(IntPtr.Zero, ref info).ReturnsForAnyArgs(x =>
            {                
                x[1] = new WINDOWINFO
                {
                    dwStyle = (uint)windows.Single(y => y.ptr == (IntPtr)x[0]).style
                };
                return true;
            });
            uint procId;
            PlatformInterface.GetWindowThreadProcessId(IntPtr.Zero, out procId).ReturnsForAnyArgs(x =>
            {
                x[1] = windows.Single(y => y.ptr == (IntPtr)x[0]).pid;
                return 0u;
            });
            PlatformInterface.GetProcessName(0).ReturnsForAnyArgs(x => windows.Single(y => y.pid == (uint)x[0]).pname);            

            var result = PlatformManagement.GetAllVisibleWindows();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(windows[0].ptr, result[0].Handle);
            Assert.AreEqual(windows[0].name, result[0].Name);
            Assert.AreEqual(windows[0].pname, result[0].ProcessName);
            Assert.AreEqual(windows[0].pid, result[0].ProcessId);
        }
    }
}
