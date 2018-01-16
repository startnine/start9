using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Start9.Api.Tools;
using System.Windows.Interop;
using System.Diagnostics;

namespace Start9.Api.Tools
{
    public static class DwmTools
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DWM_THUMBNAIL_PROPERTIES
        {
            public int dwFlags;
            public Rect rcDestination;
            public Rect rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        const int DWM_TNP_VISIBLE = 0x8,
            DWM_TNP_OPACITY = 0x4,
            DWM_TNP_RECTDESTINATION = 0x1;

        const string DwmApiString = "dwmapi";

        [DllImport(DwmApiString)]
        static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport(DwmApiString)]
        static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport(DwmApiString)]
        static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out PSIZE size);

        [DllImport(DwmApiString)]
        static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct PSIZE
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

            public int Width => Right - Left;

            public int Height => Bottom - Top;
        }

        const int GWL_STYLE = -16;

        const ulong WS_VISIBLE = 0x10000000L,
            WS_BORDER = 0x00800000L,
            TARGETWINDOW = WS_BORDER | WS_VISIBLE;

        const string UserThirtyTwoString = "user32";

        [DllImport(UserThirtyTwoString)]
        static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport(UserThirtyTwoString)]
        static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);

        [DllImport(UserThirtyTwoString)]
        static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /*public static void GetThumbnail(IntPtr hWnd, System.Windows.UIElement targetElement)
        {
            
            GetThumbnail(hWnd, new Rect(targetElementPoint.X, targetElementPoint.Y, (int)(targetElementOppositePoint.X), (int)(targetElementOppositePoint.Y)));
        }*/

        public static IntPtr GetThumbnail(IntPtr hWnd, System.Windows.UIElement targetElement)
        {
            Debug.WriteLine(hWnd.ToString() + " " + targetElement.ToString());
            IntPtr thumbHandle;
            DwmRegisterThumbnail(new WindowInteropHelper(System.Windows.Window.GetWindow(targetElement)).Handle, hWnd, out thumbHandle);
            var targetElementPoint = MainTools.GetDpiScaledGlobalControlPosition(targetElement);
            System.Windows.Point targetElementOppositePoint = new System.Windows.Point();
            if (targetElement.GetType().IsAssignableFrom(typeof(System.Windows.Controls.Control)))
            {
                var targetControl = (targetElement as System.Windows.Controls.Control);
                targetElementOppositePoint = new System.Windows.Point(targetElementPoint.X + targetControl.ActualWidth, targetElementPoint.Y + targetControl.ActualHeight);
            }
            else
            {
                targetElementOppositePoint = new System.Windows.Point(targetElementPoint.X, targetElementPoint.Y);
            }

            Rect targetRect = new Rect(targetElementPoint.X, targetElementPoint.Y, (int)(targetElementOppositePoint.X), (int)(targetElementOppositePoint.Y));

            DwmQueryThumbnailSourceSize(thumbHandle, out PSIZE size);

            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = DWM_TNP_VISIBLE | DWM_TNP_RECTDESTINATION | DWM_TNP_OPACITY,
                opacity = 255,
                //rcDestination = targetRect
                rcDestination = new Rect(0, 0, 100, 100)
            };

            /*if (size.x < targetRect.Width)
                props.rcDestination.Right = props.rcDestination.Left + size.x;

            if (size.y < targetRect.Height)
                props.rcDestination.Bottom = props.rcDestination.Top + size.y;*/

            DwmUpdateThumbnailProperties(thumbHandle, ref props);
            return thumbHandle;
        }
    }
}
