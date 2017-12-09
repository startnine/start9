using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using Start9.Api.Tools;

namespace Start9.Api.Objects
{
	public class ProgramWindow : IProgramItem
	{
		static ProgramWindow()
		{
			Automation.AddAutomationEventHandler(
				WindowPattern.WindowOpenedEvent,
				AutomationElement.RootElement,
				TreeScope.Children,
				(sender, e) =>
				{
					WindowOpened?.Invoke(
						new WindowEventArgs(new ProgramWindow(new IntPtr((sender as AutomationElement).Current.NativeWindowHandle))));
				});

			/*Automation.AddAutomationEventHandler(
			    WindowPattern.WindowClosedEvent,
			    AutomationElement.RootElement,
			    TreeScope.Subtree,
			    (sender, e) =>
			    {
			        WindowClosed?.Invoke(new WindowEventArgs(new ProgramWindow(new IntPtr((sender as AutomationElement).Current.NativeWindowHandle))));
			    });*/
		}

		public ProgramWindow(IntPtr hwnd)
		{
			Hwnd = hwnd;
			Interop.GetWindowThreadProcessId(hwnd, out var pid);
			Process = Process.GetProcessById((int) pid);

			var strbTitle = new StringBuilder(Interop.GetWindowTextLength(hwnd));
			Interop.GetWindowText(hwnd, strbTitle, strbTitle.Capacity + 1);
			Name = strbTitle.ToString();
		}

		public string Name { get; private set; }

		public Icon Icon
		{
			get
			{
				var iconHandle = Interop.SendMessage(Hwnd, WmGeticon, IconSmall2, 0);

				if (iconHandle == IntPtr.Zero)
					iconHandle = Interop.SendMessage(Hwnd, WmGeticon, IconSmall, 0);
				if (iconHandle == IntPtr.Zero)
					iconHandle = Interop.SendMessage(Hwnd, WmGeticon, IconBig, 0);
				if (iconHandle == IntPtr.Zero)
					iconHandle = Interop.GetClassLongPtr(Hwnd, GclHicon);
				if (iconHandle == IntPtr.Zero)
					iconHandle = Interop.GetClassLongPtr(Hwnd, GclHiconsm);

				if (iconHandle == IntPtr.Zero)
					return null;

				try
				{
					return Icon.FromHandle(iconHandle);
				}
				finally
				{
					Interop.DestroyIcon(iconHandle);
				}
			}
		}

		public IntPtr Hwnd { get; }
		public Process Process { get; }

		public static IEnumerable<ProgramWindow> ProgramWindows
		{
			get
			{
				var collection = new List<IntPtr>();

				bool Filter(IntPtr hWnd, int lParam)
				{
					var strbTitle = new StringBuilder(Interop.GetWindowTextLength(hWnd));
					Interop.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
					var strTitle = strbTitle.ToString();


					if (Interop.IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
						collection.Add(hWnd);

					return true;
				}

				if (!Interop.EnumDesktopWindows(IntPtr.Zero, Filter, IntPtr.Zero)) yield break;

				foreach (var hwnd in collection)
					yield return new ProgramWindow(hwnd);
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

		string IProgramItem.Name
		{
			get => Name;
			set => Name = value;
		}

		Icon IProgramItem.Icon => Icon;

		public static event WindowEventHandler WindowOpened;

		public static event WindowEventHandler WindowClosed;

		/// <summary>
		///     Makes this window the active window.
		/// </summary>
		public void Show()
		{
			try
			{
				MiscTools.ShowWindow(Hwnd, 10);
				MiscTools.SetForegroundWindow(Hwnd);
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
				MiscTools.ShowWindow(Hwnd, 11);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Window not minimized!\r\n" + ex);
			}
		}

		/// <summary>
		///     Un-minimizes the window.
		/// </summary>
		public void Maximize()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Closes the window.
		/// </summary>
		public void Close()
		{
			MiscTools.SendMessage(Hwnd, 0x0010, IntPtr.Zero, IntPtr.Zero);
		}

		public delegate void WindowEventHandler(WindowEventArgs e);

		static class Interop
		{
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool IsWindowVisible(IntPtr hWnd);


			[DllImport("user32.dll")]
			public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern int GetWindowTextLength(IntPtr hWnd);

			[DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
				ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

			[DllImport("user32.dll", SetLastError = true)]
			public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

			public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex) => IntPtr.Size > 4
				? GetClassLongPtr64(hWnd, nIndex)
				: new IntPtr(GetClassLongPtr32(hWnd, nIndex));

			[DllImport("user32.dll", EntryPoint = "GetClassLong")]
			static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

			[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
			static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool DestroyIcon(IntPtr hIcon);

			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
			public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

			public delegate bool EnumDelegate(IntPtr hWnd, int lParam);
		}

		#region p/invoke stuff don't touch lpz

		const int GclHiconsm = -34;
		const int GclHicon = -14;
		const int IconSmall = 0;
		const int IconBig = 1;
		const int IconSmall2 = 2;
		const int WmGeticon = 0x7F;

		#endregion
	}
}