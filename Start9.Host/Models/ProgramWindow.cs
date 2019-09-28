using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;

namespace Start9.Host.Models
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
				    WindowOpened?.Invoke(new WindowEventArgs(new ProgramWindow(new IntPtr((sender as AutomationElement).Current.NativeWindowHandle))));
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

		public static event WindowEventHandler WindowOpened;

		public static event WindowEventHandler WindowClosed;

		public delegate void WindowEventHandler(WindowEventArgs e);

		public string Name { get; private set; }
 
		#region p/invoke stuff don't touch lpz
		int GCL_HICONSM = -34;
	    int GCL_HICON = -14;
	    int ICON_SMALL = 0;
	    int ICON_BIG = 1;
	    int ICON_SMALL2 = 2;
	    int WM_GETICON = 0x7F;
#endregion
	    
		public Icon Icon
	    {
		    get
		    {
			    var iconHandle = Interop.SendMessage(Hwnd, WM_GETICON, ICON_SMALL2, 0);

			    if (iconHandle == IntPtr.Zero)
				    iconHandle = Interop.SendMessage(Hwnd, WM_GETICON, ICON_SMALL, 0);
			    if (iconHandle == IntPtr.Zero)
				    iconHandle = Interop.SendMessage(Hwnd, WM_GETICON, ICON_BIG, 0);
			    if (iconHandle == IntPtr.Zero)
				    iconHandle = Interop.GetClassLongPtr(Hwnd, GCL_HICON);
			    if (iconHandle == IntPtr.Zero)
				    iconHandle = Interop.GetClassLongPtr(Hwnd, GCL_HICONSM);

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

	    private ProgramWindow(IntPtr hwnd)
	    {
		    Hwnd = hwnd;
		    Interop.GetWindowThreadProcessId(hwnd, out var pid);
		    Process = System.Diagnostics.Process.GetProcessById((int) pid);

            var strbTitle = new StringBuilder(Interop.GetWindowTextLength(hwnd));
            Interop.GetWindowText(hwnd, strbTitle, strbTitle.Capacity + 1);
            Name = strbTitle.ToString();
        }

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
		/// Opens a new instance of the application that owns this window.
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

	    /// <summary>
		/// Makes this window the active window.
		/// </summary>
        public void Show()
        {
            /*
            try
            {
                TaskbarTools.ShowWindow(Hwnd, 10);
                TaskbarTools.SetForegroundWindow(Hwnd);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Window not focused!\r\n" + ex);
            }
            */
        }

	    /// <summary>
		/// Minimizes the window to the taskbar.
		/// </summary>
        public void Minimize()
        {
            /*
            try
            {
                TaskbarTools.ShowWindow(Hwnd, 11);
            }
            catch (Exception ex)
			{
                Debug.WriteLine("Window not minimized!\r\n" + ex);
            }
            */
        }

	    /// <summary>
		/// Un-minimizes the window.
		/// </summary>
        public void Maximize()
        {
            throw new NotImplementedException();
        }

	    /// <summary>
		/// Closes the window.
		/// </summary>
        public void Close()
        {
            /*
            TaskbarTools.SendMessage(Hwnd, 0x0010, IntPtr.Zero, IntPtr.Zero);
            */
        }

	    string IProgramItem.Name
        {
            get => Name;
            set => Name = value;
        }

        Icon IProgramItem.Icon => Icon;

	    private static class Interop
	    {
		    public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

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
		    private static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

		    [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		    private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

		    [DllImport("user32.dll", SetLastError = true)]
		    public static extern bool DestroyIcon(IntPtr hIcon);
			
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
			public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		}
	}
}