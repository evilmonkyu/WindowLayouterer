using System;
using System.Collections.Generic;
using System.Text;

namespace WindowLayouterer.Platform
{
    public class PlatformInterface
    {
        public virtual bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam)
        {
            return PlatformInterfaceImports.EnumDesktopWindows(hDesktop, lpfn, lParam);
        }

        public virtual uint GetLastError()
        {
            return PlatformInterfaceImports.GetLastError();
        }

        public virtual uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId)
        {
            return PlatformInterfaceImports.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
        }

        public virtual int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount)
        {
            return PlatformInterfaceImports.GetWindowText(hWnd, lpString, nMaxCount);
        }

        public virtual bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl)
        {
            return PlatformInterfaceImports.GetWindowPlacement(hWnd, ref lpwndpl);
        }

        public virtual bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi)
        {
            return PlatformInterfaceImports.GetWindowInfo(hwnd, ref pwi);
        }

        public virtual IntPtr BeginDeferWindowPos(int nNumWindows)
        {
            return PlatformInterfaceImports.BeginDeferWindowPos(nNumWindows);
        }

        public virtual IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags)
        {
            return PlatformInterfaceImports.DeferWindowPos(hWinPosInfo, hWnd, hWndInsertAfter, x, y, cx, cy, flags);
        }

        public virtual bool EndDeferWindowPos(IntPtr hWinPosInfo)
        {
            return PlatformInterfaceImports.EndDeferWindowPos(hWinPosInfo);
        }
    }
}
