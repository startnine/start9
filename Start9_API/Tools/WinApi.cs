using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Start9.Api.Tools
{
    public static class WinApi
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong) => IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : SetWindowLong32(hWnd, nIndex, dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) => IntPtr.Size == 8
            ? GetWindowLong64(hWnd, nIndex)
            : GetWindowLong32(hWnd, nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        public const UInt32 SW_HIDE = 0;
        public const UInt32 SW_SHOWNORMAL = 1;
        public const UInt32 SW_NORMAL = 1;
        public const UInt32 SW_SHOWMINIMIZED = 2;
        public const UInt32 SW_SHOWMAXIMIZED = 3;
        public const UInt32 SW_MAXIMIZE = 3;
        public const UInt32 SW_SHOWNOACTIVATE = 4;
        public const UInt32 SW_SHOW = 5;
        public const UInt32 SW_MINIMIZE = 6;
        public const UInt32 SW_SHOWMINNOACTIVE = 7;
        public const UInt32 SW_SHOWNA = 8;
        public const UInt32 SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern bool EnumWindowStations(EnumWindowStationsDelegate lpEnumFunc, IntPtr lParam);

        public delegate bool EnumWindowStationsDelegate(string windowsStation, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("coredll.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex) => IntPtr.Size > 4
            ? GetClassLongPtr64(hWnd, nIndex)
            : new IntPtr(GetClassLongPtr32(hWnd, nIndex));

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop,
                                                     EnumDelegate lpfn,
                                                     IntPtr lParam);

        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("dwmapi.dll", SetLastError = true)]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out Psize size);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnail, ref DwmThumbnailProperties props);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref ShFileInfo psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockWorkStation();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(ExitWindowsAction uFlags, ShutdownReason dwReason);

        [DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        public enum ExitWindowsAction : uint
        {
            Logoff = 0,
            Shutdown = 1,
            Reboot = 2,
            Force = 4,
            Poweroff = 8
        }

        [Flags]
        public enum ShutdownReason : uint
        {
            MajorApplication = 0x00040000,
            MajorHardware = 0x00010000,
            MajorLegacyApi = 0x00070000,
            MajorOperatingSystem = 0x00020000,
            MajorOther = 0x00000000,
            MajorPower = 0x00060000,
            MajorSoftware = 0x00030000,
            MajorSystem = 0x00050000,

            MinorBlueScreen = 0x0000000F,
            MinorCordUnplugged = 0x0000000b,
            MinorDisk = 0x00000007,
            MinorEnvironment = 0x0000000c,
            MinorHardwareDriver = 0x0000000d,
            MinorHotfix = 0x00000011,
            MinorHung = 0x00000005,
            MinorInstallation = 0x00000002,
            MinorMaintenance = 0x00000001,
            MinorMMC = 0x00000019,
            MinorNetworkConnectivity = 0x00000014,
            MinorNetworkCard = 0x00000009,
            MinorOther = 0x00000000,
            MinorOtherDriver = 0x0000000e,
            MinorPowerSupply = 0x0000000a,
            MinorProcessor = 0x00000008,
            MinorReconfig = 0x00000004,
            MinorSecurity = 0x00000013,
            MinorSecurityFix = 0x00000012,
            MinorSecurityFixUninstall = 0x00000018,
            MinorServicePack = 0x00000010,
            MinorServicePackUninstall = 0x00000016,
            MinorTermSrv = 0x00000020,
            MinorUnstable = 0x00000006,
            MinorUpgrade = 0x00000003,
            MinorWMI = 0x00000015,

            FlagUserDefined = 0x40000000,
            FlagPlanned = 0x80000000
        }

        public const int SwShownormal = 1;
        public const int SwShowminimized = 2;
        public const int SwShowmaximized = 3;

        public const int GwlStyle = -16;
        public const int GwlExstyle = -20;
        public const int Taskstyle = 0x10000000 | 0x00800000;
        public const int WsExTransparent = 0x00000020;
        public const int WsExToolwindow = 0x00000080;
        public const int WsExLayered = 0x00080000;
        public const int WsExAppWindow = 0x00040000;

        [StructLayout(LayoutKind.Sequential)]
        public struct ShFileInfo
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct DwmThumbnailProperties
        {
            public int dwFlags;
            public Rect rcDestination;
            public Rect rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        public const int DwmTnpVisible = 0x8,
                         DwmTnpOpacity = 0x4,
                         DwmTnpRectdestination = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        public struct Psize
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
