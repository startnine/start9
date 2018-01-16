using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using Start9.Api.Objects;
using Start9.Api.Plex;
using Point = System.Drawing.Point;
using System.IO;
using System.Diagnostics;

namespace Start9.Api.Tools
{
	public static class MainTools
	{
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        public static PlexWindow SettingsWindow;

		public static void ShowSettings()
		{
			SettingsWindow.Show();
			if (!SettingsWindow.IsActive)
				SettingsWindow.Focus();
		}

		public static IFixedModule GetFixedModule(string moduleName)
		{
			IFixedModule m = null;
			foreach (var i in ModuleManager.InstalledFixedModules)
				if (i.ModuleName == moduleName)
					m = i;
			return m;
		}


		public static void ShowTransientModule(int index)
		{
			/*Start9.ModuleManager.InstalledTransientModules[index].ShowModule();
			Debug.WriteLine(Start9.ModuleManager.InstalledTransientModules[index].ModuleName);*/
		}

		public static void ShowTransientModule(string moduleName)
		{
			/*foreach(ITransientModule f in Start9.ModuleManager.InstalledTransientModules)
			{
			    if (f.ModuleName == moduleName)
			    {
			        f.ShowModule();
			    }
			}*/
		}

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		static float GetScalingFactor()
		{
			var g = Graphics.FromHwnd(IntPtr.Zero);
			return g.DpiY / 96;
		}

		public static double ConvertWpfUnitsToPixels(double wpfUnits) => wpfUnits * GetScalingFactor();

		public static double ConvertPixelsToWpfUnits(double pixels) => pixels / GetScalingFactor();

		public static Point GetDpiScaledCursorPosition()
		{
			var p = Cursor.Position;
			p.X = (int) ConvertPixelsToWpfUnits(p.X);
			p.Y = (int) ConvertPixelsToWpfUnits(p.Y);
			return p;
		}

		public static Point GetDpiScaledGlobalControlPosition(UIElement uiElement)
		{
			var uiPoint = uiElement.PointToScreen(new System.Windows.Point(0, 0));

			var uiDrawPoint = new Point((int)ConvertPixelsToWpfUnits(uiPoint.X), (int)ConvertPixelsToWpfUnits(uiPoint.Y));
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
		public const int SwShownormal = 1;
		public const int SwShowminimized = 2;
		public const int SwShowmaximized = 3;

		public const int GwlStyle = -16;
		public const int GwlExstyle = -20;
		public const int Taskstyle = 0x10000000 | 0x00800000;
		public const int WsExToolwindow = 0x00000080;

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public static Color GetColorFromImage(string sourceImagePath)
        {
            Debug.WriteLine("sourceImagePath: " + sourceImagePath);
            Color OutputColor = Color.FromArgb((byte)255, (byte)128, (byte)128, (byte)128);
            System.Drawing.Image SourceBitmap = new System.Drawing.Bitmap(1, 1);
            if (sourceImagePath == Environment.ExpandEnvironmentVariables(@"%windir%\Explorer.exe"))
            {
                OutputColor = Color.FromArgb((byte)255, (byte)0, (byte)130, (byte)153);
            }
            else if ((File.Exists(sourceImagePath)) | (Directory.Exists(sourceImagePath)))
            {
                if (File.Exists(sourceImagePath))
                {
                    Debug.WriteLine("sourceImagePath File exists");
                    SourceBitmap = System.Drawing.Bitmap.FromHicon(System.Drawing.Icon.ExtractAssociatedIcon(sourceImagePath).Handle);
                }
                else if (Directory.Exists(sourceImagePath))
                {
                    Debug.WriteLine("sourceImagePath Dir exists");
                    SHFILEINFO shinfo = new SHFILEINFO();
                    SHGetFileInfo(sourceImagePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), 0x100 | 0x0);
                    SourceBitmap = System.Drawing.Bitmap.FromHicon(shinfo.hIcon);
                }

                System.Drawing.Bitmap DestBitmap = new System.Drawing.Bitmap(1, 1);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)DestBitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(SourceBitmap, 0, 0, 1, 1);
                }
                System.Drawing.Color DestColor = DestBitmap.GetPixel(0, 0);
                OutputColor = Color.FromArgb((byte)255, (byte)DestColor.R, (byte)DestColor.G, (byte)DestColor.B);
            }

            return OutputColor;
        }
    }
}