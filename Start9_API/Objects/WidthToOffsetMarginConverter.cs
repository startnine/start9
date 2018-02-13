using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Start9.Api.Objects
{
    public class WidthToOffsetMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string param = parameter.ToString();
            if (param == "Top")
            {
                return new Thickness(0, (double)value, 0, (double)value * -1);
            }
            else if (param == "Right")
            {
                return new Thickness((double)value * -1, 0, (double)value, 0);
            }
            else if (param == "Bottom")
            {
                return new Thickness(0, (double)value * -1, 0, (double)value);
            }
            else
            {
                return new Thickness((double)value, 0, (double)value * -1, 0);
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string param = parameter.ToString();
            if (param == "Top")
            {
                return (double)(((Thickness)(value)).Top);
            }
            else if (param == "Right")
            {
                return (double)(((Thickness)(value)).Right);
            }
            else if (param == "Bottom")
            {
                return (double)(((Thickness)(value)).Bottom);
            }
            else
            {
                return (double)(((Thickness)(value)).Left); //return new Thickness((double)value, 0, (double)value * -1, 0);
            }
        }
    }

    public class WidthToHalfWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((double)value) / 2;
            }
            else
            {
                string paramString = (string)parameter;
                return ((double)value) / System.Convert.ToDouble(paramString);
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((double)value) * 2;
            }
            else
            {
                string paramString = (string)parameter;
                return ((double)value) * System.Convert.ToDouble(paramString);
            }
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class GetResourceForImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //.IceIconFrame
            System.Drawing.Bitmap resource = Start9.Api.Properties.Resources.IceIconFrame;
            
            /*foreach(object o in .ResourceManager)
            {

            }*/

            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(resource.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            System.Windows.Media.ImageBrush brushFromResource = new System.Windows.Media.ImageBrush(bitmapSource);

            resource.Dispose();

            return brushFromResource.ImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DependencyPoint : DependencyObject
    {
        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, (value));
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(DependencyPoint), new PropertyMetadata((double)0));

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, (value));
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(DependencyPoint), new PropertyMetadata((double)0));
    }

    public class Dimensions : DependencyObject
    {
        public DependencyPoint BaseControlDimensions
        {
            get => (DependencyPoint)GetValue(BaseControlDimensionsProperty);
            set => SetValue(BaseControlDimensionsProperty, (value));
        }

        public static readonly DependencyProperty BaseControlDimensionsProperty =
            DependencyProperty.Register("BaseControlDimensions", typeof(DependencyPoint), typeof(Dimensions), new PropertyMetadata(null));


        public Thickness BaseControlThickness
        {
            get => (Thickness)GetValue(BaseControlThicknessProperty);
            set => SetValue(BaseControlThicknessProperty, (value));
        }

        public static readonly DependencyProperty BaseControlThicknessProperty =
            DependencyProperty.Register("BaseControlThickness", typeof(Thickness), typeof(Dimensions), new PropertyMetadata(null));


        public DependencyPoint TargetControlDimensions
        {
            get => (DependencyPoint)GetValue(TargetControlDimensionsProperty);
            set => SetValue(TargetControlDimensionsProperty, (value));
        }

        public static readonly DependencyProperty TargetControlDimensionsProperty =
            DependencyProperty.Register("TargetControlDimensions", typeof(DependencyPoint), typeof(Dimensions), new PropertyMetadata(null));
    }


    public class MarginToRelativeMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Dimensions paramDim = (Dimensions)parameter;
            if (((paramDim.BaseControlDimensions) != null) & ((paramDim.TargetControlDimensions) != null) & ((paramDim.BaseControlThickness) != null))
            {
                Thickness baseThickness = paramDim.BaseControlThickness;
                DependencyPoint baseControlDim = paramDim.BaseControlDimensions;
                DependencyPoint targetControlDim = paramDim.TargetControlDimensions;
                Thickness resultThickness = new Thickness((baseThickness.Left / baseControlDim.X) * targetControlDim.X, (baseThickness.Top / baseControlDim.Y) * targetControlDim.Y, (baseThickness.Right / baseControlDim.X) * targetControlDim.X, (baseThickness.Bottom / baseControlDim.Y) * targetControlDim.Y);
                Debug.WriteLine(resultThickness.ToString() + "     " + baseControlDim.X.ToString() + "," + baseControlDim.Y.ToString() + "     " + targetControlDim.X.ToString() + "," + targetControlDim.Y.ToString());
                return resultThickness;
            }
            else
            {
                Debug.WriteLine("RETURNING BASETHICKNESS...");
                return paramDim.BaseControlThickness;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((double)value) * 2;
            }
            else
            {
                string paramString = (string)parameter;
                return ((double)value) * System.Convert.ToDouble(paramString);
            }
        }
    }
}