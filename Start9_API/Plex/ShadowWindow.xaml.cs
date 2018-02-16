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

        public ShadowWindow(PlexWindow window)
		{
            Opacity = 0;
			InitializeComponent();
            PlexWindow = window;

            Binding renderTransformBinding = new Binding()
            {
                Source = PlexWindow,
                Path = new PropertyPath("RenderTransform"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, ShadowWindow.RenderTransformProperty, renderTransformBinding);

            Binding opacityBinding = new Binding()
            {
                Source = PlexWindow,
                Path = new PropertyPath("Opacity"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(ShadowGrid, System.Windows.Controls.Grid.OpacityProperty, opacityBinding);

            Binding shadowOpacityBinding = new Binding()
            {
                Source = PlexWindow,
                Path = new PropertyPath("ShadowOpacity"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, ShadowWindow.OpacityProperty, shadowOpacityBinding);

            Binding topmostBinding = new Binding()
            {
                Source = PlexWindow,
                Path = new PropertyPath("Topmost"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, ShadowWindow.TopmostProperty, topmostBinding);

            Binding visibilityBinding = new Binding()
            {
                Source = PlexWindow,
                Path = new PropertyPath("Visibility"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, ShadowWindow.VisibilityProperty, visibilityBinding);

            Loaded += (sender, e) =>
            {
                PlexWindow.ShiftShadowBehindWindow();
            };
            
            IsVisibleChanged += (sender, e) =>
            {
                PlexWindow.ShiftShadowBehindWindow();
            };
        }

        

        protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			//Set the window style to noactivate.
			var helper = new WindowInteropHelper(this);
			WinApi.SetWindowLong(helper.Handle, WinApi.GwlExstyle, (int)(WinApi.GetWindowLong(helper.Handle, WinApi.GwlExstyle)) | 0x00000080 | 0x00000020);
            PlexWindow.ShiftShadowBehindWindow();
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