using System.Windows;
using System.Windows.Controls;

namespace Start9.Pages
{
	/// <summary>
	///     Interaction logic for Home.xaml
	/// </summary>
	public partial class Home : Page
	{
		public Home()
		{
			InitializeComponent();
		}

		private void PlexTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new PlexStylesTestPage());
		}

		private void ModuleFrontEndTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new ModuleTestsPage());
		}
	}
}