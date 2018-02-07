using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Start9.Api.Tools;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using Timer = System.Timers.Timer;

namespace Start9.Api.Plex
{
	/// <summary>
	///     Interaction logic for ShadowWindow.xaml
	/// </summary>
	public partial class ShadowWindow : Window
	{
        public PlexWindow PlexWindow;

        /*public double OpacityMultiplier
        {
            get => (double)GetValue(OpacityMultiplierProperty);
            set => SetValue(OpacityMultiplierProperty, value);
        }

        public static readonly DependencyProperty OpacityMultiplierProperty =
            DependencyProperty.Register("OpacityMultiplier", typeof(double), typeof(PlexWindow), new PropertyMetadata((double)1));*/

        public ShadowWindow()
		{
			InitializeComponent();
			//ShadowGrid.Margin = new Thickness(Padding.Left * -1, Padding.Top * -1, Padding.Right * -1, Padding.Bottom * -1);
		}

		public ShadowWindow(PlexWindow window)
		{
            Opacity = 0;
			InitializeComponent();
            PlexWindow = window;
            RenderTransform = PlexWindow.RenderTransform;
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
            Loaded += (sender, e) =>
            {
                PlexWindow.ShiftShadowBehindWindow();
            };
            Activated += ShadowWindow_Activated;
            IsVisibleChanged += (sender, e) =>
            {
                PlexWindow.ShiftShadowBehindWindow();
            };

            Timer _shadowOpacityTimer = new Timer
            {
                Interval = 1
            };

            _shadowOpacityTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    {
                        Opacity = PlexWindow.Opacity * PlexWindow.ShadowOpacity;
                    }
                }));
            };

            //_shadowOpacityTimer.Start();
            Binding opacityBinding = new Binding();
            opacityBinding.Source = PlexWindow;
            opacityBinding.Path = new PropertyPath("ShadowOpacity");
            opacityBinding.Mode = BindingMode.OneWay;
            opacityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(this, ShadowWindow.OpacityProperty, opacityBinding);
            /*Binding opacityBinding = new Binding();
            opacityBinding.Source = PlexWindow;
            opacityBinding.Path = new PropertyPath("Opacity");
            opacityBinding.Mode = BindingMode.OneWay;
            opacityBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            Binding opacityMultiplierBinding = new Binding();
            opacityMultiplierBinding.Source = PlexWindow;
            opacityMultiplierBinding.Path = new PropertyPath("ShadowOpacityMultiplier");
            opacityMultiplierBinding.Mode = BindingMode.OneWay;
            opacityMultiplierBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            MultiBinding multiOpacityBinding = new MultiBinding()
            {
                Converter = new RawOpacityToMultipliedOpacityConverter()
            };
            multiOpacityBinding.Bindings.Add(opacityBinding);
            multiOpacityBinding.Bindings.Add(opacityMultiplierBinding);

            BindingOperations.SetBinding(this, ShadowWindow.OpacityProperty, opacityBinding);*/
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ShadowWindow_Activated(null, null);
            
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            ShadowWindow_Activated(null, null);
        }

        private void ShadowWindow_Activated(object sender, EventArgs e)
        {
            //PlexWindow.Focus();
            //PlexWindow.Activate();
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

    /*public class RawOpacityToMultipliedOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (double)value * ((ShadowWindow)parameter).OpacityMultiplier;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (double)value / ((ShadowWindow)parameter).OpacityMultiplier;
        }
    }*/

    public class RawOpacityToMultipliedOpacityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)values[0] * (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}