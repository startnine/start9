using System;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows;
using static Start9.Api.Tools.WinApi;
using Rect = System.Windows.Rect;

namespace Start9.Api.Tools
{
    public static class DwmTools
	{
		const int GWL_STYLE = -16;

		const ulong WS_VISIBLE = 0x10000000L,
					WS_BORDER           = 0x00800000L,
					TARGETWINDOW        = WS_BORDER | WS_VISIBLE;

        public static IntPtr GetThumbnail(IntPtr hWnd, System.Windows.UIElement targetElement)
        {
            Debug.WriteLine($"{hWnd} {targetElement}");
			DwmRegisterThumbnail(new WindowInteropHelper(Window.GetWindow(targetElement)).Handle, hWnd, out var thumbHandle);
            var targetElementPoint = MainTools.GetDpiScaledGlobalControlPosition(targetElement);
            Point targetElementOppositePoint;
            if (targetElement.GetType().IsAssignableFrom(typeof(System.Windows.Controls.Control)))
            {
                var targetControl = (targetElement as System.Windows.Controls.Control);
				targetElementOppositePoint = new Point(targetElementPoint.X + targetControl.ActualWidth,
													   targetElementPoint.Y + targetControl.ActualHeight);
			}
            else
            {
                targetElementOppositePoint = new Point(targetElementPoint.X, targetElementPoint.Y);
            }

            var targetRect = new Rect(targetElementPoint.X, targetElementPoint.Y, (int)(targetElementOppositePoint.X), (int)(targetElementOppositePoint.Y));

            DwmQueryThumbnailSourceSize(thumbHandle, out var size);

            var props = new DwmThumbnailProperties
			{
                fVisible = true,
                dwFlags = DwmTnpVisible | DwmTnpRectdestination | DwmTnpOpacity,
                opacity = 255,
                rcDestination = new WinApi.Rect(0, 0, 100, 100)
            };


			DwmUpdateThumbnailProperties(thumbHandle, ref props);
            return thumbHandle;
        }
    }
}
