using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using Start9.Api.Objects;
using Start9.Api.Tools;

namespace Start9.Api.Programs
{
	public class ProgramWindow : IProgramItem
	{
		static ProgramWindow()
		{
			Automation.AddAutomationEventHandler(
				WindowPattern.WindowOpenedEvent,
				AutomationElement.RootElement,
				TreeScope.Children,
				(sender, e) => WindowOpened?.Invoke(null,
													new WindowEventArgs(
														new ProgramWindow(new IntPtr(((AutomationElement) sender).Current.NativeWindowHandle)))));

			Automation.AddAutomationEventHandler(
				WindowPattern.WindowClosedEvent,
				AutomationElement.RootElement,
				TreeScope.Subtree,
				(sender, e) => WindowClosed?.Invoke(null,
													new WindowEventArgs(
														new ProgramWindow(new IntPtr(((AutomationElement) sender).Cached.NativeWindowHandle)))));
		}

		public ProgramWindow(IntPtr hwnd) { Hwnd = hwnd; }

		public IntPtr Hwnd { get; }

		public Process Process
		{
			get
			{
				WinApi.GetWindowThreadProcessId(Hwnd, out var pid);
				return Process.GetProcessById((int) pid);
			}
		}

		public static IEnumerable<ProgramWindow> ProgramWindows
		{
			get
			{
				var collection = new List<IntPtr>();

				bool Filter(IntPtr hWnd, int lParam)
				{
					var strbTitle = new StringBuilder(WinApi.GetWindowTextLength(hWnd));
					WinApi.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
					var strTitle = strbTitle.ToString();


					if (WinApi.IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
						collection.Add(hWnd);

					return true;
				}

				if (!WinApi.EnumDesktopWindows(IntPtr.Zero, Filter, IntPtr.Zero)) yield break;

				foreach (var hwnd in collection)
					yield return new ProgramWindow(hwnd);
			}
		}

        /*public static IEnumerable<ProgramWindow> UserPerceivedProgramWindows => ProgramWindows.Where(
			hwnd => TASKSTYLE == (TASKSTYLE & WinApi.GetWindowLong(hwnd.Hwnd, GWL_STYLE).ToInt32()) &
					(WinApi.GetWindowLong(hwnd.Hwnd, GWL_EXSTYLE).ToInt32() & WS_EX_TOOLWINDOW) != WS_EX_TOOLWINDOW);*/
        public static IEnumerable<ProgramWindow> UserPerceivedProgramWindows
        {
            get
            {
                var collection = new List<IntPtr>();

                bool Filter(IntPtr hWnd, int lParam)
                {
                    var strbTitle = new StringBuilder(WinApi.GetWindowTextLength(hWnd));
                    WinApi.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                    var strTitle = strbTitle.ToString();


                    if (WinApi.IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                        collection.Add(hWnd);

                    return true;
                }

                if (!WinApi.EnumDesktopWindows(IntPtr.Zero, Filter, IntPtr.Zero)) yield break;

                foreach (var hwnd in collection)
                {
                    try
                    {
                        if ((TASKSTYLE == (TASKSTYLE & WinApi.GetWindowLong(hwnd, GWL_STYLE).ToInt32())) && ((WinApi.GetWindowLong(hwnd, GWL_EXSTYLE).ToInt32() & WS_EX_TOOLWINDOW) != WS_EX_TOOLWINDOW))
                        {
                            yield return new ProgramWindow(hwnd);
                        }
                    }
                    finally
                    {
                        
                    }
                }
            }
        }

        static void AddClosedHandler(IntPtr handle)
		{
			Automation.AddAutomationEventHandler(
				WindowPattern.WindowClosedEvent,
				AutomationElement.FromHandle(handle),
				TreeScope.Subtree,
				(sender, e) =>
				{
					Debug.WriteLine(handle.ToString() + " IS KILL");
					WindowClosed?.Invoke(null, new WindowEventArgs(new ProgramWindow(handle)));
				});
		}

		public static event EventHandler<WindowEventArgs> WindowOpened;

		public static event EventHandler<WindowEventArgs> WindowClosed;

		/// <summary>
		///     Makes this window the active window.
		/// </summary>
		public void Show()
		{
			try
			{
				WinApi.ShowWindow(Hwnd, 10);
				WinApi.SetForegroundWindow(Hwnd);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Window not focused!\r\n" + ex);
			}
		}

		/// <summary>
		///     Minimizes the window to the taskbar.
		/// </summary>
		public void Minimize()
		{
			try
			{
				WinApi.ShowWindow(Hwnd, 11);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Window not minimized!\r\n" + ex);
			}
		}

		/// <summary>
		///     Un-minimizes the window.
		/// </summary>
		public void Maximize() { throw new NotImplementedException(); }

		/// <summary>
		///     Closes the window.
		/// </summary>
		public void Close() { WinApi.SendMessage(Hwnd, 0x0010, 0, 0); }

		public string Name
		{
			get
			{
				var strbTitle = new StringBuilder(WinApi.GetWindowTextLength(Hwnd));
				WinApi.GetWindowText(Hwnd, strbTitle, strbTitle.Capacity + 1);
				return strbTitle.ToString();
			}
		}

		public Icon Icon
		{
			get
			{
				var iconHandle = WinApi.SendMessage(Hwnd, WmGeticon, IconSmall2, 0);

				if (iconHandle == IntPtr.Zero)
					iconHandle = WinApi.SendMessage(Hwnd, WmGeticon, IconSmall, 0);
				if (iconHandle == IntPtr.Zero)
					iconHandle = WinApi.SendMessage(Hwnd, WmGeticon, IconBig, 0);
				if (iconHandle == IntPtr.Zero)
					iconHandle = WinApi.GetClassLongPtr(Hwnd, GclHicon);
				if (iconHandle == IntPtr.Zero)
					iconHandle = WinApi.GetClassLongPtr(Hwnd, GclHiconsm);

				if (iconHandle == IntPtr.Zero)
					return null;

				try
				{
					return Icon.FromHandle(iconHandle);
				}
				finally
				{
					WinApi.DestroyIcon(iconHandle);
				}
			}
		}

		/// <summary>
		///     Opens a new instance of the application that owns this window.
		/// </summary>
		public void Open()
		{
			try
			{
				if (File.Exists(Process.MainModule.FileName))
					Process.Start(Process.MainModule.FileName);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("New Instance not started:\r\n" + ex);
			}
		}

		#region p/invoke stuff don't touch lpz

		const int GclHiconsm       = -34;
		const int GclHicon         = -14;
		const int IconSmall        = 0;
		const int IconBig          = 1;
		const int IconSmall2       = 2;
		const int WmGeticon        = 0x7F;
		const int GWL_STYLE        = -16;
		const int GWL_EXSTYLE      = -20;
		const int TASKSTYLE        = 0x10000000 | 0x00800000;
		const int WS_EX_TOOLWINDOW = 0x00000080;

		#endregion
	}
}