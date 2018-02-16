using Start9.Windows;
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
    /// Interaction logic for ModuleTestsPage.xaml
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
