using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Start9.Api.Objects;
using Start9.Api.Tools;

namespace TestModule
{
	/// <summary>
	///     Interaction logic for TaskItemButton.xaml
	/// </summary>
	public partial class TaskItemButton : UserControl
	{
		public static readonly DependencyProperty WindowTitleProperty =
			DependencyProperty.Register("WindowTitle", typeof(string), typeof(TaskItemButton), new PropertyMetadata(""));

		public static readonly DependencyProperty IsActiveWindowProperty =
			DependencyProperty.Register("IsActiveWindow", typeof(bool), typeof(TaskItemButton), new PropertyMetadata(false));

		public static readonly DependencyProperty WindowIconProperty = DependencyProperty.Register("WindowIcon",
			typeof(ImageBrush), typeof(TaskItemButton), new PropertyMetadata(new ImageBrush()));


		public TaskItemButton()
		{
			InitializeComponent();
		}

		public TaskItemButton(ProgramWindow programWindow)
		{
			InitializeComponent();
			try
			{
				WindowIcon = new ImageBrush(programWindow.Icon.ToBitmap().ToBitmapSource());
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			WindowTitle = programWindow.Name;
			Tag = programWindow;
			var doesThisWindowExist = new Timer
			{
				Interval = 100
			};
			var thisModule = (SuperbarModule) MainTools.GetFixedModule("Test Module");
			doesThisWindowExist.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					{
						if (!IsWindow(programWindow.Hwnd))
							foreach (TaskbarGroupStackPanel t in thisModule.Taskbars[0].Taskband.Children)
							foreach (var p in t.ProgramWindowsList)
								if (p.Hwnd == programWindow.Hwnd)
									t.RemoveButtonByHwnd(programWindow.Hwnd);
					}
				}));
			};
		}

		public string WindowTitle
		{
			get => (string) GetValue(WindowTitleProperty);
			set => SetValue(WindowTitleProperty, value);
		}

		public bool IsActiveWindow
		{
			get => (bool) GetValue(IsActiveWindowProperty);
			set => SetValue(IsActiveWindowProperty, value);
		}

		public ImageBrush WindowIcon
		{
			get => (ImageBrush) GetValue(WindowIconProperty);
			set => SetValue(WindowIconProperty, value);
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindow(IntPtr hWnd);

		public void IsActiveWindowChangedCallBack(object defaultValue, PropertyChangedCallback propertyChangedCallback)
		{
		}
	}
}