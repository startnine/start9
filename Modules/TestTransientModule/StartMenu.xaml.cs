using System;
using System.Windows;
using System.Windows.Interop;
using Start9.Api.Tools;

namespace TestTransientModule
{
	/// <summary>
	///     Interaction logic for StartMenu.xaml
	/// </summary>
	public partial class StartMenu : Window
	{
		public StartMenu()
		{
			InitializeComponent();
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var helper = new WindowInteropHelper(this);
			MiscTools.SetWindowLong(helper.Handle, MiscTools.GwlExstyle,
				MiscTools.GetWindowLong(helper.Handle, MiscTools.GwlExstyle) | 0x08000000);
		}

		public void ShowMenu()
		{
			Show();
			Left = SystemParameters.WorkArea.Left;
			Top = SystemParameters.WorkArea.Bottom - ActualHeight;
		}

		public void HideMenu()
		{
			Hide();
		}
	}
}