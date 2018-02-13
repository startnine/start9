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

namespace Start9.Windows
{
    /// <summary>
    /// Interaction logic for TaskbarStylesTest.xaml
    /// </summary>
    public partial class TaskbarStylesTest : PlexWindow
    {
        public TaskbarStylesTest()
        {
            InitializeComponent();
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
    }
}
