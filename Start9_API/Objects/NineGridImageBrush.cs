using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.IO;
using System.Drawing;

namespace Start9.Api.Objects
{
    //Partially from http://blog.csdn.net/xeonol/article/details/1943067
    [MarkupExtensionReturnType(typeof(System.Windows.Media.Brush))]
    public class NineGridImageBrush : MarkupExtension, INotifyPropertyChanged
    {
        double targetWidth = 100;

        public double TargetWidth
        {
            get => targetWidth;
            set => targetWidth = value;
        }

        double targetHeight = 1;

        public double TargetHeight
        {
            get { return targetHeight; }
            set { targetHeight = value; OnPropertyChanged("TargetHeight"); }
        }

        Thickness sizingMargins = new Thickness(0, 0, 0, 0);

        public Thickness SizingMargins
        {
            get { return sizingMargins; }
            set { sizingMargins = value; OnPropertyChanged("SizingMargins"); }
        }

        string brushImageSource = @"_";

        public string BrushImageSource
        {
            get { return brushImageSource; }
            set { brushImageSource = value; OnPropertyChanged("BrushImageSource"); }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (File.Exists(BrushImageSource))
            {
                BitmapImage bitmap = new BitmapImage();

                using (MemoryStream memory = new MemoryStream())
                {
                    (InsetResize(new System.Drawing.Bitmap(brushImageSource), new System.Drawing.Size((int)TargetWidth, (int)TargetHeight), SizingMargins)).Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmap.BeginInit();
                    bitmap.StreamSource = memory;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }

                ImageBrush brush = new ImageBrush(bitmap);

                return brush;
            }
            else
            {
                return new ImageBrush();
            }
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


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}