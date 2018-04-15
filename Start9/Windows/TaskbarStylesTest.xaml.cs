using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using Start9.Api.Controls;
using Start9.Api.Programs;
using Start9.Api.Tools;

namespace Start9.Windows
{
	/// <summary>
	///     Interaction logic for TaskbarStylesTest.xaml
	/// </summary>
	public partial class TaskbarStylesTest : Window
	{
		public Timer ClockTimer = new Timer(1);
		public List<String> RunningProcesses = new List<String>();

		public TaskbarStylesTest()
		{
			InitializeComponent();
			Left = 0;
			Top = SystemParameters.PrimaryScreenHeight - 40;
			Width = SystemParameters.PrimaryScreenWidth;
			WinApi.ShowWindow(WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null), 0);

			ClockTimer.Start();

			ClockTimer.Elapsed += (sender, args) => Dispatcher.Invoke(new Action(() =>
			{
				IntPtr active = WinApi.GetForegroundWindow();
				/*foreach (TaskbarGroupStackPanel t in Taskband.Children)
				        try
				        {
				            if (t.ForceCombine | (Config.GroupingMode == TaskbarGroupingMode.Combine))
				            {
				                var isAnythingActive = false;
				                foreach (var b in t.ProgramWindowsList)
				                    if (b.Hwnd == active)
				                        isAnythingActive = true;
				                if (isAnythingActive)
				                    t.RunningBackgroundButton.IsActiveWindow = true;
				                else
				                    t.RunningBackgroundButton.IsActiveWindow = false;
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
				        }*/
			}));
			ClockTimer.Start();

			InitialPopulateTaskbar();
		}

		public void InitialPopulateTaskbar()
		{
			foreach (ProgramWindow wind in ProgramWindow.ProgramWindows)
			{
				try
				{
					if (!RunningProcesses.Contains(wind.Process.MainModule.FileName))
						RunningProcesses.Add(wind.Process.MainModule.FileName);
				}
				catch
				{
					Debug.WriteLine("Process not added to list");
				}
			}

			foreach (String s in RunningProcesses)
			{
				var programStackPanel = new TaskItemGroup(s);
				Taskband.Children.Add(programStackPanel);
			}

			foreach (ProgramWindow wind in ProgramWindow.ProgramWindows
			) //ProgramWindow.UserPerceivedProgramWindows is broken or something, I think
			foreach (TaskItemGroup t in Taskband.Children)
			{
				try
				{
					if (wind.Process.MainModule.FileName == t.Tag.ToString())
						t.ProcessWindows.Add(wind);
				}
				catch
				{
				}
			}

			foreach (TaskItemGroup t in Taskband.Children)
				//if (Taskband.ActualWidth >= Width)
			{
				if (t.ProcessWindows.Count > 3)
					t.CombineButtons = true;
			}

			//t.CreateButtons();
		}

		private void TextClock_Loaded(Object sender, RoutedEventArgs e)
		{
			var clockTimer = new Timer(1);
			clockTimer.Elapsed += (o, args) => Dispatcher.Invoke(new Action(
				() => { TextClock.Text = DateTime.Now.ToShortTimeString() + "\n" + DateTime.Now.ToShortDateString(); }
			));
			clockTimer.Start();
		}

		private void TrayFlyoutToggleButton_Click(Object sender, RoutedEventArgs e)
		{
			//var targetToggleButton = sender as ToggleButton;
			//var targetWindow = targetToggleButton.Tag as PlexWindow;
			if (TrayFlyoutToggleButton.IsChecked == true)
			{
				Point nonScaledButtonPoint = TrayFlyoutToggleButton.PointToScreen(new Point(0, 0));
				var buttonPoint = new Point(DpiManager.ConvertPixelsToWpfUnits(nonScaledButtonPoint.X),
					DpiManager.ConvertPixelsToWpfUnits(nonScaledButtonPoint.Y));
                Double targetLeftPos = buttonPoint.X + TrayFlyoutToggleButton.Width / 2 - TrayFlyout.ActualWidth / 2;
                Double targetTopPos =
					DpiManager.ConvertPixelsToWpfUnits(TaskbarRootGrid.PointToScreen(new Point(0, 0)).Y) - 10 -
					TrayFlyout.ActualHeight;
				TrayFlyout.Left = targetLeftPos;
				TrayFlyout.Top = targetTopPos;
				TrayFlyout.Show();
				TrayFlyout.Left = targetLeftPos;
				TrayFlyout.Top = targetTopPos;
				//targetWindow.Top = (buttonPoint.Y + (targetToggleButton.Height / 2)) - (targetWindow.Height / 2);
			}
			else
			{
				TrayFlyout.Hide();
			}
		}

		private void TrayFlyout_Deactivated(Object sender, EventArgs e)
		{
			TrayFlyout.Hide();
			TrayFlyoutToggleButton.IsChecked = false;
		}

		private void TrayFlyout_Loaded(Object sender, RoutedEventArgs e)
		{
			TrayFlyout.Resources.Add(Resources["TrayIconButton"], Resources["TrayIconButton"]);
		}
	}
}