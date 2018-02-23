using System.AddIn.Hosting;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Start9.Host.AddInView;
using Start9.Windows;

namespace Start9.Pages
{
	/// <summary>
	///		Interaction logic for ModuleTestsPage.xaml
	/// </summary>
	public partial class ModuleTestsPage : Page
	{
		public ModuleTestsPage()
		{
			InitializeComponent();

			foreach (AddInToken calculator in _calculators)
			{
				Calculators.Items.Add(new ListViewItem { Content = calculator.Name });
			}
		}

		private readonly Collection<AddInToken> _calculators = AddInManager.LoadAddins();

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var calculator = _calculators[Calculators.SelectedIndex is -1 ? 0 : Calculators.SelectedIndex].Activate<ICalculator>(AddInSecurityLevel.Internet);

			(double result, bool success) = AddInManager.RunCalculator(calculator, Calculation.Text);

			if (success)
			{
				MessageBox.Show($"The result is {result}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show("Invalid calculation");
			}
		}
	}
}