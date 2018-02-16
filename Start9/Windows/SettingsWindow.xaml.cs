using System.Windows;
using Start9.Api.Plex;
using Start9.Api.Tools;
using Start9.Pages;
using System.Windows.Interop;
using System;
using System.Windows.Controls;

namespace Start9.Windows
{
	/// <summary>
	///     Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : PlexWindow
	{
		public SettingsWindow()
        {
            InitializeComponent();
            Resources["TestBitmapImage"] = Start9.Api.Tools.MiscTools.GetBitmapImageFromBitmapSource(Start9.Api.Tools.MiscTools.GetBitmapSourceFromSysDrawingBitmap(Start9.Api.Properties.Resources.FallbackImage));
            MainTools.SettingsWindow = this;
            SettingsFrame.Navigate(new Home());
        }

		private void ShowThumbnailButton_Click(object sender, RoutedEventArgs e)
        {
            //DwmTools.GetThumbnail(WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Progman"), ShowThumbnailButton);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsFrame.NavigationService.GoBack();
        }

        private void SettingsFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            /*if (SettingsFrame.CanGoBack)
            {
                BackButton.IsEnabled = true;
            }
            else
            {
                BackButton.IsEnabled = false;
            }*/
        }

        /*private void PlexTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(0, 0);
        }

        private void ModuleFrontEndTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(0, 1);
        }

        public void GoToPage(int layerIndex, int pageIndex)
        {
            PageLayers.Visibility = Visibility.Visible;
            HomePage.Visibility = Visibility.Hidden;
            foreach (Grid g in PageLayers.Children)
            {
                if (PageLayers.Children.IndexOf(g) == layerIndex)
                {
                    g.Visibility = Visibility.Visible;
                    foreach (Grid h in g.Children)
                    {
                        if (g.Children.IndexOf(h) == pageIndex)
                        {
                            h.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            h.Visibility = Visibility.Hidden;
                        }
                    }
                }
                else
                {
                    g.Visibility = Visibility.Hidden;
                }
            }
        }

        public void GoHome()
        {
            PageLayers.Visibility = Visibility.Hidden;
            HomePage.Visibility = Visibility.Visible;
        }

        private void TaskbarModule1Button_Click(object sender, RoutedEventArgs e)
        {
            new TaskbarStylesTest().Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoHome();
        }*/
    }
}