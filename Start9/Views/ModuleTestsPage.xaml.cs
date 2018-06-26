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
            ((Module) Modules.SelectedItem).LaunchUninitialized();
        }

        private void Button_Click_1(Object sender, RoutedEventArgs e)
        {

        }
    }

    public sealed class InstanceToProcessIdConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var module = (IModule) value;

            return AddInController.GetAddInController(module).AddInEnvironment.Process.ProcessId;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("InstanceToTextConverter can only be used for one way conversion.");
        }
    }
}