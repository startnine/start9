using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace Start9.Api.Objects
{
    public class ImageBrushNineGridInfo : DependencyObject
    {
        public Bitmap SizingBitmap
        {
            get => (Bitmap)GetValue(SizingMarginProperty);
            set => SetValue(SizingMarginProperty, value);
        }

        public static readonly DependencyProperty SizingBitmapProperty =
            DependencyProperty.Register("SizingBitmap", typeof(Bitmap), typeof(ImageBrushNineGridInfo), new PropertyMetadata(new Bitmap(10, 10)));

        public Thickness SizingMargin
        {
            get => (Thickness)GetValue(SizingMarginProperty);
            set => SetValue(SizingMarginProperty, value);
        }

        public static readonly DependencyProperty SizingMarginProperty =
            DependencyProperty.Register("SizingMargin", typeof(Thickness), typeof(ImageBrushNineGridInfo), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        public double TargetWidth
        {
            get => (double)GetValue(TargetWidthProperty);
            set => SetValue(TargetWidthProperty, value);
        }

        public static readonly DependencyProperty TargetWidthProperty =
            DependencyProperty.Register("TargetWidth", typeof(double), typeof(ImageBrushNineGridInfo), new PropertyMetadata((double)10));

        public double TargetHeight
        {
            get => (double)GetValue(TargetHeightProperty);
            set => SetValue(TargetHeightProperty, value);
        }

        public static readonly DependencyProperty TargetHeightProperty =
            DependencyProperty.Register("TargetHeight", typeof(double), typeof(ImageBrushNineGridInfo), new PropertyMetadata((double)10));
    }

    public class ImageBrushNineGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var fromControl = value as System.Windows.Controls.Control;
            // Do the conversion from bool to visibility
            //InsetResize()
            if (parameter is ImageBrushNineGridInfo)
            {
                var paramNineGridInfo = parameter as ImageBrushNineGridInfo;
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream memory = new MemoryStream())
                {
                    (InsetResize(paramNineGridInfo.SizingBitmap, new System.Drawing.Size((int)paramNineGridInfo.TargetWidth, (int)paramNineGridInfo.TargetHeight), paramNineGridInfo.SizingMargin)).Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmap.BeginInit();
                    bitmap.StreamSource = memory;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }

                return new ImageBrush(bitmap);
            }
            else
            {
                return new ImageBrush();
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var fromControl = value as System.Windows.Controls.Control;
            return fromControl.Background;
        }

        //huge thanccs to dejco
        private System.Drawing.Bitmap InsetResize(System.Drawing.Bitmap sourceBitmap, System.Drawing.Size destSize, Thickness insetThickness)
        {
            System.Drawing.Bitmap outputBitmap = new System.Drawing.Bitmap(destSize.Width, destSize.Height);
            try
            {
                Int32Rect inset = new Int32Rect((int)insetThickness.Left, (int)insetThickness.Top, (int)insetThickness.Right, (int)insetThickness.Bottom);

                using (Graphics g = Graphics.FromImage(outputBitmap))
                {
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 0, 0, 0)), 0, 0, destSize.Width, destSize.Height);

                    g.DrawImage(sourceBitmap,
                      new System.Drawing.Rectangle(inset.X, inset.Y, destSize.Width - (inset.X + inset.Width), destSize.Height - (inset.Y + inset.Height)),     //destination
                      new System.Drawing.Rectangle(inset.X, inset.Y, sourceBitmap.Width - (inset.X + inset.Width), sourceBitmap.Height - (inset.Y + inset.Height)), //source
                      GraphicsUnit.Pixel);

                    if (inset.Y > 0) //Top
                    {
                        g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(inset.X, 0, destSize.Width - (inset.X + inset.Width), inset.Y), //destination
                            new System.Drawing.Rectangle(inset.X, 0, sourceBitmap.Width - (inset.X + inset.Width), inset.Y), //source
                            GraphicsUnit.Pixel);
                    }

                    if (inset.Height > 0)
                    {
                        g.DrawImage(sourceBitmap,
                       new System.Drawing.Rectangle(inset.X, destSize.Height - inset.Height, destSize.Width - (inset.X + inset.Width), inset.Height),     //destination
                       new System.Drawing.Rectangle(inset.X, sourceBitmap.Height - inset.Height, sourceBitmap.Width - (inset.X + inset.Width), inset.Height), //source
                       GraphicsUnit.Pixel);
                    }

                    if (inset.X > 0)
                    {
                        //left
                        g.DrawImage(sourceBitmap,
                           new System.Drawing.Rectangle(0, inset.Y, inset.X, destSize.Height - (inset.Y + inset.Height)),     //destination
                           new System.Drawing.Rectangle(0, inset.Y, inset.X, sourceBitmap.Height - (inset.Y + inset.Height)), //source
                           GraphicsUnit.Pixel);

                        //top left
                        if (inset.Y > 0)
                        {
                            g.DrawImage(sourceBitmap,
                                new System.Drawing.Rectangle(0, 0, inset.X, inset.Y), //destination
                                new System.Drawing.Rectangle(0, 0, inset.X, inset.Y), //source
                                GraphicsUnit.Pixel);
                        }

                        //bottom left
                        if (inset.Height > 0)
                        {
                            g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(0, destSize.Height - inset.Height, inset.X, inset.Height),     //destination
                            new System.Drawing.Rectangle(0, sourceBitmap.Height - inset.Height, inset.X, inset.Height), //source
                            GraphicsUnit.Pixel);
                        }
                    }

                    if (inset.Width > 0) //Right
                    {
                        g.DrawImage(sourceBitmap,
                           new System.Drawing.Rectangle(destSize.Width - inset.Width, inset.Y, inset.Width, destSize.Height - (inset.Y + inset.Height)),     //destination
                           new System.Drawing.Rectangle(sourceBitmap.Width - inset.Width, inset.Y, inset.Width, sourceBitmap.Height - (inset.Y + inset.Height)), //source
                           GraphicsUnit.Pixel);

                        //top right
                        if (inset.Y > 0)
                        {
                            g.DrawImage(sourceBitmap,
                                new System.Drawing.Rectangle(destSize.Width - inset.Width, 0, inset.Width, inset.Y), //destination
                                new System.Drawing.Rectangle(sourceBitmap.Width - inset.Width, 0, inset.Width, inset.Y), //source
                                GraphicsUnit.Pixel);
                        }

                        //bottom right
                        if (inset.Height > 0)
                        {
                            g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(destSize.Width - inset.Width, destSize.Height - inset.Height, inset.Width, inset.Height),     //destination
                            new System.Drawing.Rectangle(sourceBitmap.Width - inset.Width, sourceBitmap.Height - inset.Height, inset.Width, inset.Height), //source
                            GraphicsUnit.Pixel);
                        }
                    }
                }
            }
            catch { }
            return outputBitmap;
        }
    }
}
