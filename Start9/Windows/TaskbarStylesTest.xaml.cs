using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Start9.Api.Plex;
using System.Windows.Controls.Primitives;
using Start9.Api.Tools;
using Start9.Api.Programs;
using System.Diagnostics;
using Start9.Api.Controls;

namespace Start9.Windows
{
    /// <summary>
    /// Interaction logic for TaskbarStylesTest.xaml
    /// </summary>
    public partial class TaskbarStylesTest : Window
    {
        public List<string> RunningProcesses = new List<string>();

        public System.Timers.Timer ClockTimer = new System.Timers.Timer(1);

        public TaskbarStylesTest()
        {
            InitializeComponent();
            Left = 0;
            Top = SystemParameters.PrimaryScreenHeight - 40;
            Width = SystemParameters.PrimaryScreenWidth;
            WinApi.ShowWindow(WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null), 0);

            ClockTimer.Start();

            ClockTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var active = WinApi.GetForegroundWindow();
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
            };
            ClockTimer.Start();

            InitialPopulateTaskbar();
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
                var programStackPanel = new TaskItemGroup(s);
                Taskband.Children.Add(programStackPanel);
            }

            foreach (var wind in ProgramWindow.ProgramWindows) //ProgramWindow.UserPerceivedProgramWindows is broken or something, I think
                foreach (TaskItemGroup t in Taskband.Children)
                    try
                    {
                        if (wind.Process.MainModule.FileName == t.Tag.ToString())
                            t.ProcessWindows.Add(wind);
                    }
                    catch
                    {
                    }

            foreach (TaskItemGroup t in Taskband.Children)
            {
                //if (Taskband.ActualWidth >= Width)
                if (t.ProcessWindows.Count > 3)
                {
                    t.CombineButtons = true;
                }
                //t.CreateButtons();
            }
        }

        private void TextClock_Loaded(object sender, RoutedEventArgs e)
        {
            var clockTimer = new System.Timers.Timer(1);
            clockTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    TextClock.Text = DateTime.Now.ToShortTimeString() + "\n" + DateTime.Now.ToShortDateString();
                }
                ));
            };
            clockTimer.Start();
        }

        private void TrayFlyoutToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //var targetToggleButton = sender as ToggleButton;
            //var targetWindow = targetToggleButton.Tag as PlexWindow;
            if (TrayFlyoutToggleButton.IsChecked == true)
            {
                var nonScaledButtonPoint = TrayFlyoutToggleButton.PointToScreen(new Point(0, 0));
                var buttonPoint = new Point(DpiManager.ConvertPixelsToWpfUnits(nonScaledButtonPoint.X), DpiManager.ConvertPixelsToWpfUnits(nonScaledButtonPoint.Y));
                double targetLeftPos = (buttonPoint.X + (TrayFlyoutToggleButton.Width / 2)) - (TrayFlyout.ActualWidth / 2);
                double targetTopPos = ((DpiManager.ConvertPixelsToWpfUnits(TaskbarRootGrid.PointToScreen(new Point(0, 0)).Y)) - 10) - TrayFlyout.ActualHeight;
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

        private void TrayFlyout_Deactivated(object sender, EventArgs e)
        {
            TrayFlyout.Hide();
            TrayFlyoutToggleButton.IsChecked = false;
        }

        private void TrayFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            TrayFlyout.Resources.Add(Resources["TrayIconButton"], Resources["TrayIconButton"]);
        }
    }
}
