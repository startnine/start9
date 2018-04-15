//using Start9.Host.View;
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
        Collection<AddInToken> _addins;

        public ModuleTestsPage()
		{
			InitializeComponent();

            var addInRoot = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%"), "Start9", "Pipeline");

            // Update the cache files of the pipeline segments and add-ins.
            var warnings = AddInStore.Update(addInRoot);
            foreach (String warning in warnings)
            {
                MessageBox.Show(warning);
            }

            // Search for add-ins of type ICalculator (the host view of the add-in).
            _addins = AddInStore.FindAddIns(typeof(ICalculator), addInRoot);

            foreach(var token in _addins)
            {
                Addins.Items.Add(token.Name);
            }
        
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
		{

            // Activate the selected AddInToken in a new application domain 
            // with the Internet trust level.
            ICalculator calc = _addins[Addins.SelectedIndex].Activate<ICalculator>(AddInSecurityLevel.Internet);

            MessageBox.Show(calc.Add(2, 2).ToString());

        }
    }
}