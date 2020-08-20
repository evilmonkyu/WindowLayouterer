using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowLayouterer.Tests
{
    [TestClass]
    public class PlatformInterfaceTests
    {
        private static Process TestAppProcess;
        private static IntPtr TestAppWindowHandle;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            StartTestApp();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            StopTestApp();
        }

        //[TestMethod]
        //public void Can_GetWorkStation()
        //{
        //    var handle = PlatformInterface.GetDesktopWindow();

        //    Assert.IsTrue(handle.ToInt64() > 0);
        //}

        [TestMethod]
        public void Does_EnumDesktopWindows()
        {
            var windows = new List<int>();
            EnumDesktopWindowsDelegate callback = (IntPtr hWnd, int lParam) =>
            {
                windows.Add(hWnd.ToInt32());
                return true;
            };

            var result = PlatformInterface.EnumDesktopWindows(IntPtr.Zero, callback, IntPtr.Zero);

            Assert.IsTrue(result);
            Assert.IsTrue(windows.Count > 0);
        }

        [TestMethod]
        public void Does_GetLastError()
        {
            PlatformInterface.GetLastError();
        }

        [TestMethod]
        public void Does_GetWindowThreadProcessId()
        {
            uint processId;
            PlatformInterface.GetWindowThreadProcessId(TestAppWindowHandle, out processId);
            Assert.AreEqual(TestAppProcess.Id, (int)processId);
        }

        [TestMethod]
        public void Does_GetWindowText()
        {
            var sb = new StringBuilder();
            PlatformInterface.GetWindowText(TestAppWindowHandle, sb, 2000);
            Assert.AreEqual("Test App", sb.ToString());
        }

        [TestMethod]
        public void Does_GetWindowPlacement()
        {
            var windowPlacement = new WINDOWPLACEMENT();
            PlatformInterface.GetWindowPlacement(TestAppWindowHandle, ref windowPlacement);
            Assert.AreNotEqual(0, windowPlacement.NormalPosition.Right);
        }

        [TestMethod]
        public void Does_GetWindowInfo()
        {
            var windowInfo = new WINDOWINFO();
            PlatformInterface.GetWindowInfo(TestAppWindowHandle, ref windowInfo);
            Assert.AreNotEqual(0u, windowInfo.dwStyle);
        }

        [TestMethod]
        public void Can_ResizeWindow()
        {
            var posInfo = PlatformInterface.BeginDeferWindowPos(1);
            PlatformInterface.DeferWindowPos(posInfo, TestAppWindowHandle, IntPtr.Zero, 100, 100, 1000, 1000, (uint)(DeferWindowPosCommands.SWP_NOZORDER & DeferWindowPosCommands.SWP_NOACTIVATE));
            PlatformInterface.EndDeferWindowPos(posInfo);
        }

        //[TestMethod]
        //public void CanEnumerateWindowProcesses()
        //{
        //    var windows = new List<IntPtr>();
        //    EnumDesktopWindowsDelegate callback = (IntPtr hWnd, int lParam) =>
        //    {
        //        windows.Add(hWnd);
        //        return true;
        //    };

        //    PlatformInterface.EnumDesktopWindows(IntPtr.Zero, callback, IntPtr.Zero);

        //    IEnumerable<string> strs = null;
        //    try
        //    {
        //        var winfo = new WINDOWINFO();
        //        var stringBuilder = new StringBuilder();
        //        strs = windows.Select(w =>
        //        {
        //            uint processId;
        //            PlatformInterface.GetWindowThreadProcessId(w, out processId);
        //            stringBuilder.Clear();
        //            //PlatformInterface.GetWindowText(w, stringBuilder, 2048);
        //            winfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(winfo));
        //            if (!PlatformInterface.GetWindowInfo(w, ref winfo)) return "";
        //            return processId + " " + stringBuilder.ToString() + " = " + (winfo.dwStyle & 0x10000000u);
        //        }).ToList();
        //    }
        //    catch (Exception e)
        //    { 
        //    }

        //    foreach (var s in strs.Where(x => !x.EndsWith("= 0")))
        //    {
        //        Console.WriteLine(s);
        //    }
        //    Assert.IsTrue(windows.Count > 0);
        //}

        private static void StartTestApp()
        {
            if (File.Exists("pid.txt"))
            {
                File.Delete("pid.txt");
            }
            var info = new ProcessStartInfo
            {
                FileName = @"..\..\..\..\WindowLayouterer.TestApp\bin\Debug\netcoreapp3.1\WindowLayouterer.TestApp.exe",
                UseShellExecute = true               
            };
            TestAppProcess = Process.Start(info);
            Thread.Sleep(500);
            var textId = File.ReadAllText(@"pid.txt");
            TestAppWindowHandle = new IntPtr(long.Parse(textId));
        }

        private static void StopTestApp()
        {
            TestAppProcess.Kill();
        }
    }
}
