using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using Start9.Api.Plex;
using Point = System.Drawing.Point;
using System.IO;

namespace Start9.Api.Tools
{
	public static class MainTools
	{
        public static PlexWindow SettingsWindow;

		public static void ShowSettings()
		{
			SettingsWindow.Show();
			if (!SettingsWindow.IsActive)
				SettingsWindow.Focus();
		}
		
		static float GetScalingFactor()
		{
			var g = Graphics.FromHwnd(IntPtr.Zero);
			return g.DpiY / 96;
		}


		public static Point GetDpiScaledCursorPosition()
		{
			var p = Cursor.Position;
			p.X = DpiManager.ConvertPixelsToWpfUnits(p.X);
			p.Y = DpiManager.ConvertPixelsToWpfUnits(p.Y);
			return p;
		}

		public static Point GetDpiScaledGlobalControlPosition(UIElement uiElement)
		{
			var uiPoint = uiElement.PointToScreen(new System.Windows.Point(0, 0));

			var uiDrawPoint = new Point(DpiManager.ConvertPixelsToWpfUnits(uiPoint.X),
										DpiManager.ConvertPixelsToWpfUnits(uiPoint.Y));
			return uiDrawPoint;
		}

		public enum DeviceCap
		{
			Vertres = 10,
			Desktopvertres = 117,
			Logpixelsy = 90
		}
	}

	public static class MiscTools
	{
        public static Color GetColorFromImage(string sourceImagePath)
		{
			var outputColor = Color.Gray;
            var sourceBitmap = new Bitmap(1, 1);

            if (sourceImagePath == Environment.ExpandEnvironmentVariables(@"%windir%\Explorer.exe"))
            {
                outputColor = Color.FromArgb(255, 0, 130, 153);
            }
            else if (File.Exists(sourceImagePath) || Directory.Exists(sourceImagePath))
            {
                if (File.Exists(sourceImagePath))
                {
                    sourceBitmap = Bitmap.FromHicon(Icon.ExtractAssociatedIcon(sourceImagePath).Handle);
                }
                else if (Directory.Exists(sourceImagePath))
				{
					var shinfo = new WinApi.ShFileInfo();
                    WinApi.SHGetFileInfo(sourceImagePath, 0, ref shinfo, (uint) Marshal.SizeOf(shinfo), 256);
                    sourceBitmap = Bitmap.FromHicon(shinfo.hIcon);
                }

				Color destColor;
				using (var destBitmap = new Bitmap(1, 1))
				using (var g = Graphics.FromImage(destBitmap))
				{
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					g.DrawImage(sourceBitmap, 0, 0, 1, 1);

					destColor = destBitmap.GetPixel(0, 0);
				}
				outputColor = Color.FromArgb(255, destColor.R, destColor.G, destColor.B);

            }

            return outputColor;
        }
    }
}