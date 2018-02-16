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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Start9.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
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
