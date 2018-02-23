using System.Windows;
using System.Windows.Controls;
using Start9.Windows;

namespace Start9.Pages
{
    /// <summary>
    ///     Interaction logic for ModuleTestsPage.xaml
    /// </summary>
    public partial class ModuleTestsPage : Page
    {
        public ModuleTestsPage()
        {
            InitializeComponent();
        }

        private void TaskbarModule1Button_Click(object sender, RoutedEventArgs e)
        {
            new TaskbarStylesTest().Show();
        }
    }
}