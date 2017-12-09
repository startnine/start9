using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using Start9.Api.Objects;

namespace Start9
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			//Globals.Taskbar = new Taskbar();
			Globals.SettingsWindow = new Windows.SettingsWindow();
			//ModuleManager.Test2();
			ModuleManager.Test();
		}

		~App()
		{
			Automation.RemoveAllEventHandlers();
		}
	}

	public static class TaskbarTools
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

		public enum TaskbarGroupingMode
		{
			Combine,
			CombineWhenFull,
			NeverCombine
		}

		public enum TaskbarGroupSideTabMode
		{
			None,
			GroupTabSingle,
			GroupTabDouble,
			JumpListButton
		}

		public enum TaskbarGroupStatus
		{
			Idle,
			Running,
			LookAtMeImOrange,
			Progress,
			ProgressPaused,
			ProgressRekt,
			ProgressIdekAnymore
		}
	}
}