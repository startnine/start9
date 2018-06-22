using Start9.Host.View;
using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;

namespace Start9.Pages
{
	/// <summary>
	///     Interaction logic for ModuleTestsPage.xaml
	/// </summary>
	public partial class ModuleTestsPage : Page
	{
        Dictionary<AddInToken, IModule> ModuleRefs = new Dictionary<AddInToken, IModule>();
        public ModuleTestsPage()
		{
			InitializeComponent();

            var addInRoot = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%"), "Start9", "Pipeline");

            var warnings = AddInStore.Update(addInRoot);
            foreach (var warning in warnings)
            {
                MessageBox.Show(warning);
            }

            Modules.ItemsSource = AddInStore.FindAddIns(typeof(IModule), addInRoot);
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
		{
            var module = ((AddInToken)Modules.SelectedItem).Activate<IModule>(new AddInProcess(), AddInSecurityLevel.FullTrust);
            module.HostReceived(new Start9Host());
            ModuleRefs.Add(((AddInToken)Modules.SelectedItem), module);
        }

        private void Button_Click_1(Object sender, RoutedEventArgs e)
        {
            ModuleRefs[((AddInToken)Modules.SelectedItem)].SendMessage(new Message(MessageText.Text));
        }
    }
}