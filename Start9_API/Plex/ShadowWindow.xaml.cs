using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Start9.Api.Tools;

namespace Start9.Api.Plex
{
	/// <summary>
	///     Interaction logic for ShadowWindow.xaml
	/// </summary>
	public partial class ShadowWindow : Window
	{
		public ShadowWindow()
		{
			InitializeComponent();
			//ShadowGrid.Margin = new Thickness(Padding.Left * -1, Padding.Top * -1, Padding.Right * -1, Padding.Bottom * -1);
		}

		public ShadowWindow(PlexWindow window)
		{
			InitializeComponent();
            /*Tag = window;
			Activated += delegate
			{
				try
				{
					(Tag as PlexWindow).Focus();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			};*/
            //ShadowGrid.Margin = new Thickness(Padding.Left * -1, Padding.Top * -1, Padding.Right * -1, Padding.Bottom * -1);
        }

        protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			
			//Set the window style to noactivate.
			var helper = new WindowInteropHelper(this);
			WinApi.SetWindowLong(helper.Handle,
								 WinApi.GwlExstyle,
								 new IntPtr(WinApi.GetWindowLong(helper.Handle, WinApi.GwlExstyle).ToInt32() | 0x00000080 | 0x00000020));
		}
	}
}