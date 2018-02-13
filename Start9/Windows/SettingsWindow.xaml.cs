using System.Windows;
using Start9.Api.Plex;
using Start9.Api.Tools;
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
        }

		void TitleBarCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			ShowTitleBar = true;
		}

		void TitleBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			ShowTitleBar = false;
		}

		void ToolBarCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			ShowToolBar = true;
		}

		void ToolBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			ShowToolBar = false;
		}

		void FooterCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			ShowFooter = true;
		}

		void FooterCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			ShowFooter = false;
		}

        private void ShowThumbnailButton_Click(object sender, RoutedEventArgs e)
        {
            //DwmTools.GetThumbnail(WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Progman"), ShowThumbnailButton);
        }

        private void ColouresButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary coloures = new ResourceDictionary();

            if ((sender as System.Windows.Controls.Primitives.ToggleButton).IsChecked == true)
            {
                coloures.Source = new Uri("/Start9.Api;component/Themes/Colors/PlexGreen.xaml", UriKind.Relative);
            }
            else
            {
                coloures.Source = new Uri("/Start9.Api;component/Themes/Colors/PlexBlue.xaml", UriKind.Relative);
            }

            Resources.MergedDictionaries.Add(coloures);
            //BodyBrush = (System.Windows.Media.Brush)(BodyRoot.Resources["DefaultWindowBodyBrush"]);
        }

        private void PlexTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
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
        }
    }
}