using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Start9.Api.Objects;
using Start9.Api.Tools;

namespace TestModule
{
	/// <summary>
	///     Interaction logic for Taskbar.xaml
	/// </summary>
	public partial class Taskbar : Window
	{
		public Taskbar()
		{
			InitializeComponent();
			Width = SystemParameters.PrimaryScreenWidth;
			Top = SystemParameters.PrimaryScreenHeight - Height;
			Left = 0;
			IsVisibleChanged += Taskbar_IsVisibleChanged;
			//Visibility = Visibility.Hidden;
			InitialPopulateTaskbar();
			ClockTimer.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					ClockTime.Content = DateTime.Now.ToShortTimeString();
					ClockDate.Content = DateTime.Now.ToShortDateString();
				}));
			};
			ClockTimer.Start();
			ProgramWindow.WindowOpened += InsertCreatedWindow;
			ProgramWindow.WindowClosed += RemoveClosedWindow;
			_activeWindowTimer.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					var active = MiscTools.GetForegroundWindow();
					foreach (TaskbarGroupStackPanel t in Taskband.Children)
						try
						{
							if (t.ForceCombine | (Config.GroupingMode == TaskbarGroupingMode.Combine))
							{
								var isAnythingActive = false;
								foreach (var b in t.ProgramWindowsList)
									if (b.Hwnd == active)
										isAnythingActive = true;
								if (isAnythingActive)
									t.RunningBackgroundButton.IsManipulationEnabled = true;
								else
									t.RunningBackgroundButton.IsManipulationEnabled = false;
							}
							else
							{
								foreach (TaskItemButton b in t.Buttons.Children)
									if ((b.Tag as ProgramWindow).Hwnd == active)
										b.IsActiveWindow = true;
									else
										b.IsActiveWindow = false;
							}
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
						}
				}));
			};
			_activeWindowTimer.Start();
			Debug.WriteLine(MainTools.GetFixedModule("Test Module").ModuleName + " FOUND!");
		}

		readonly Timer _activeWindowTimer = new Timer
		{
			Interval = 1
		};

		readonly SuperbarModule _thisModule = (SuperbarModule) MainTools.GetFixedModule("Test Module");

		public Timer ClockTimer = new Timer
		{
			Interval = 1
		};

		public List<string> RunningProcesses = new List<string>();

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			//Set the window style to noactivate.
			var helper = new WindowInteropHelper(this);
			MiscTools.SetWindowLong(helper.Handle, MiscTools.GwlExstyle,
				MiscTools.GetWindowLong(helper.Handle, MiscTools.GwlExstyle) | 0x08000000);
		}

		void Taskbar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
			{
				//Hide the Explorer Taskbar here
			}
		}

		public void InitialPopulateTaskbar()
		{
			foreach (var wind in ProgramWindow.ProgramWindows)
				try
				{
					if (!RunningProcesses.Contains(wind.Process.MainModule.FileName))
						RunningProcesses.Add(wind.Process.MainModule.FileName);
				}
				catch
				{
					Debug.WriteLine("Process not added to list");
				}

			foreach (var s in RunningProcesses)
			{
				var programStackPanel = new TaskbarGroupStackPanel
				{
					Background = new SolidColorBrush(Color.FromArgb(0x01, 0x0, 0x0, 0x0)),
					VerticalAlignment = VerticalAlignment.Stretch,
					Margin = new Thickness(2, 0, 2, 0),
					Tag = s
				};

				Taskband.Children.Add(programStackPanel);
			}

			foreach (var wind in ProgramWindow.ProgramWindows)
			foreach (TaskbarGroupStackPanel t in Taskband.Children)
				try
				{
					if (wind.Process.MainModule.FileName == t.Tag.ToString())
						t.ProgramWindowsList.Add(wind);
				}
				catch
				{
				}

			foreach (TaskbarGroupStackPanel t in Taskband.Children)
			{
				//if (Taskband.ActualWidth >= Width)
				if (t.ProgramWindowsList.Count > 3)
					t.ForceCombine = true;
				t.CreateButtons(Taskband);
			}
		}


		public void InsertCreatedWindow(WindowEventArgs e)
		{
			Taskband.Dispatcher.Invoke(new Action(() =>
			{
				{
					var addedToExistingGroup = false;
					foreach (TaskbarGroupStackPanel t in Taskband.Children)
						try
						{
							if (e.Window.Process.MainModule.FileName == t.Tag.ToString())
							{
								t.ProgramWindowsList.Add(e.Window);
								t.CreateButtons(Taskband);
								addedToExistingGroup = true;
							}
						}
						catch
						{
						}

					if (!addedToExistingGroup)
					{
						var programStackPanel = new TaskbarGroupStackPanel
						{
							Background = new SolidColorBrush(Color.FromArgb(0x01, 0x0, 0x0, 0x0)),
							VerticalAlignment = VerticalAlignment.Stretch,
							Margin = new Thickness(2, 0, 2, 0),
							Tag = e.Window.Process.MainModule.FileName
						};

						programStackPanel.ProgramWindowsList.Add(e.Window);
						programStackPanel.CreateButtons(Taskband);
					}
				}
			}));
		}

		public void RemoveClosedWindow(WindowEventArgs e)
		{
			Taskband.Dispatcher.Invoke(new Action(() =>
			{
				{
					foreach (TaskbarGroupStackPanel t in Taskband.Children)
						try
						{
							t.RemoveButtonByHwnd(e.Window.Hwnd);
						}
						catch
						{
						}
				}
			}));
		}

		void Start_Click(object sender, RoutedEventArgs e)
		{
			ModuleCommand.GetCommandByName("Start Button Command", _thisModule.Commands).FireCommand();
		}

		void Search_Click(object sender, RoutedEventArgs e)
		{
			ModuleCommand.GetCommandByName("Search Button Command", _thisModule.Commands).FireCommand();
		}

		void TaskView_Click(object sender, RoutedEventArgs e)
		{
			ModuleCommand.GetCommandByName("Task View Button Command", _thisModule.Commands).FireCommand();
		}

		void ActionCenter_Click(object sender, RoutedEventArgs e)
		{
			ModuleCommand.GetCommandByName("Action Center Button Command", _thisModule.Commands).FireCommand();
		}
	}

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