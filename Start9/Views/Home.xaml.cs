using System.Printing;
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

		private void PlexTestCommandLinkButton_Click(System.Object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new PlexStylesTestPage());
        }

        private void ModuleFrontEndTestCommandLinkButton_Click(System.Object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new ModuleTestsPage());
		}

        private void MarketTestCommandLinkButton_Click(System.Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MarketplaceTestPage());
        }
    }
}