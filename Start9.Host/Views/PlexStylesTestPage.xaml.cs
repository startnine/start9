using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
//using Start9.Api.Plex;

namespace Start9.Host.Pages
{
	/// <summary>
	///     Interaction logic for PlexStylesTestPage.xaml
	/// </summary>
	public partial class PlexStylesTestPage : Page
	{
		//private readonly DecoratableWindow ParentWindow;

		public PlexStylesTestPage()
		{
			InitializeComponent();
			//ParentWindow = Window.GetWindow(this) as PlexWindow;
		}

		private void TitleBarCheckBox_Checked(Object sender, RoutedEventArgs e)
		{
			try
			{
				//ParentWindow.ShowTitleBar = true;
			}
			catch
			{
			}
		}

		private void TitleBarCheckBox_Unchecked(Object sender, RoutedEventArgs e)
		{
			try
			{
				//ParentWindow.ShowTitleBar = false;
			}
			catch
			{
			}
		}

		private void ToolBarCheckBox_Checked(Object sender, RoutedEventArgs e)
		{
			try
			{
				//ParentWindow.ShowToolBar = true;
			}
			catch
			{
			}
		}

		private void ToolBarCheckBox_Unchecked(Object sender, RoutedEventArgs e)
		{
			try
			{
				//ParentWindow.ShowToolBar = false;
			}
			catch
			{
			}
		}

		private void FooterCheckBox_Checked(Object sender, RoutedEventArgs e)
		{
			try
			{
				//ParentWindow.ShowFooter = true;
			}
			catch
			{
			}
		}

		private void FooterCheckBox_Unchecked(Object sender, RoutedEventArgs e)
		{
			try
			{
				//ParentWindow.ShowFooter = false;
			}
			catch
			{
			}
		}

		private void ColouresButton_Click(Object sender, RoutedEventArgs e)
		{
			var coloures = new ResourceDictionary
			{
				Source = (sender as ToggleButton).IsChecked == true
				? new Uri("/Start9.Api;component/Themes/Colors/PlexGreen.xaml", UriKind.Relative)
				: new Uri("/Start9.Api;component/Themes/Colors/PlexBlue.xaml", UriKind.Relative)
			};

			Resources.MergedDictionaries.Add(coloures);
			//BodyBrush = (System.Windows.Media.Brush)(BodyRoot.Resources["DefaultWindowBodyBrush"]);
		}
	}
}