using Start9.Host.Views;
using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

        private void Button_Click(Object sender, RoutedEventArgs e)
		{
            ((Module) Modules.SelectedItem).Activate();
        }

        private void Button_Click_1(Object sender, RoutedEventArgs e)
        {

        }
    }

}