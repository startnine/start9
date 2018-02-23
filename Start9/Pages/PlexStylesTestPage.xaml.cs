using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Start9.Api.Plex;

namespace Start9.Pages
{
	/// <summary>
	///     Interaction logic for PlexStylesTestPage.xaml
	/// </summary>
	public partial class PlexStylesTestPage : Page
	{
		private readonly PlexWindow ParentWindow;

		public PlexStylesTestPage()
		{
			InitializeComponent();
			ParentWindow = Window.GetWindow(this) as PlexWindow;
		}

		private void TitleBarCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				ParentWindow.ShowTitleBar = true;
			}
			catch
			{
			}
		}

		private void TitleBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			try
			{
				ParentWindow.ShowTitleBar = false;
			}
			catch
			{
			}
		}

		private void ToolBarCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				ParentWindow.ShowToolBar = true;
			}
			catch
			{
			}
		}

		private void ToolBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			try
			{
				ParentWindow.ShowToolBar = false;
			}
			catch
			{
			}
		}

		private void FooterCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				ParentWindow.ShowFooter = true;
			}
			catch
			{
			}
		}

		private void FooterCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			try
			{
				ParentWindow.ShowFooter = false;
			}
			catch
			{
			}
		}

		private void ColouresButton_Click(object sender, RoutedEventArgs e)
		{
			var coloures = new ResourceDictionary();

			if ((sender as ToggleButton).IsChecked == true)
				coloures.Source = new Uri("/Start9.Api;component/Themes/Colors/PlexGreen.xaml", UriKind.Relative);
			else
				coloures.Source = new Uri("/Start9.Api;component/Themes/Colors/PlexBlue.xaml", UriKind.Relative);

			Resources.MergedDictionaries.Add(coloures);
			//BodyBrush = (System.Windows.Media.Brush)(BodyRoot.Resources["DefaultWindowBodyBrush"]);
		}
	}
}