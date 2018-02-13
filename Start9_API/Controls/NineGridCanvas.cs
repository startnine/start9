using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Start9;
using Start9.Api;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Start9.Api.Controls
{
    public partial class NineGridCanvas : Canvas
    {
        /*public Thickness SizingMargin
        {
            get => (Thickness)GetValue(SizingMarginProperty);
            set => SetValue(SizingMarginProperty, value);
        }

        public static readonly DependencyProperty SizingMarginProperty = DependencyProperty.RegisterAttached(
        "SizingMargin", typeof(Thickness), typeof(NineGridCanvas), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, OnSizingMarginChanged));

        private static void OnSizingMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as NineGridCanvas).InsetResizeBackground();
        }

        new public BitmapImage Background
        {

            get =>  (BitmapImage)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        new public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
        "Background", typeof(BitmapImage), typeof(NineGridCanvas), new FrameworkPropertyMetadata(Start9.Api.Tools.MiscTools.GetBitmapImageFromSysDrawingBitmap(Start9.Api.Properties.Resources.FallbackImage), FrameworkPropertyMetadataOptions.AffectsRender, OnBackgroundChanged));

        private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as NineGridCanvas).InsetResizeBackground();
        }

        private void InsetResizeBackground()
        {
            base.Background = new ImageBrush(Start9.Api.Tools.MiscTools.GetBitmapImageFromSysDrawingBitmap(InsetResize(Background)));
        }

        private void InsetResizeOpacityMask()
        {
            base.OpacityMask = new ImageBrush(Start9.Api.Tools.MiscTools.GetBitmapImageFromSysDrawingBitmap(InsetResize(Background)));
        }

        private Bitmap InsetResize(BitmapImage sourceBitmapImage)
        {
            int width = (int)Width;
            if (width <= 0)
            {
                width = 1;
            }

            int height = (int)Height;
            if (height <= 0)
            {
                height = 1;
            }

            //Rect sizingMargin = new Rect(SizingMargin.Left, SizingMargin.Top, SizingMargin.Right, SizingMargin.Bottom);

            Bitmap outputBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(outputBitmap))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 0, 0, 0)), 0, 0, height, height);

                var inset = new Rectangle((int)SizingMargin.Left, (int)SizingMargin.Top, width, height);

                var sourceBitmap = Start9.Api.Tools.MiscTools.ConvertBitmapImageToBitmap(sourceBitmapImage);

                g.DrawImage(sourceBitmap,
                  new System.Drawing.Rectangle(inset.Left, inset.Top, width - (inset.Left + inset.Right), height - (inset.Top + inset.Bottom)),     //destination
                  new System.Drawing.Rectangle(inset.Left, inset.Top, sourceBitmap.Width - (inset.Left + inset.Right), sourceBitmap.Height - (inset.Top + inset.Bottom)), //source
                  GraphicsUnit.Pixel);

                if (inset.Top > 0) //Top
                {
                    g.DrawImage(sourceBitmap,
                        new System.Drawing.Rectangle(inset.Left, 0, width - (inset.Left + inset.Right), inset.Top), //destination
                        new System.Drawing.Rectangle(inset.Left, 0, sourceBitmap.Width - (inset.Left + inset.Right), inset.Top), //source
                        GraphicsUnit.Pixel);
                }

                if (inset.Bottom > 0)
                {
                    g.DrawImage(sourceBitmap,
                   new System.Drawing.Rectangle(inset.Left, height - inset.Bottom, width - (inset.Left + inset.Right), inset.Bottom),     //destination
                   new System.Drawing.Rectangle(inset.Left, sourceBitmap.Height - inset.Bottom, sourceBitmap.Width - (inset.Left + inset.Right), inset.Bottom), //source
                   GraphicsUnit.Pixel);
                }

                if (inset.Left > 0)
                {
                    //left
                    g.DrawImage(sourceBitmap,
                       new System.Drawing.Rectangle(0, inset.Top, inset.Left, height - (inset.Top + inset.Bottom)),     //destination
                       new System.Drawing.Rectangle(0, inset.Top, inset.Left, sourceBitmap.Height - (inset.Top + inset.Bottom)), //source
                       GraphicsUnit.Pixel);

                    //top left
                    if (inset.Top > 0)
                    {
                        g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(0, 0, inset.Left, inset.Top), //destination
                            new System.Drawing.Rectangle(0, 0, inset.Left, inset.Top), //source
                            GraphicsUnit.Pixel);
                    }

                    //bottom left
                    if (inset.Bottom > 0)
                    {
                        g.DrawImage(sourceBitmap,
                        new System.Drawing.Rectangle(0, height - inset.Bottom, inset.Left, inset.Bottom),     //destination
                        new System.Drawing.Rectangle(0, sourceBitmap.Height - inset.Bottom, inset.Left, inset.Bottom), //source
                        GraphicsUnit.Pixel);
                    }
                }

                if (inset.Right > 0) //Right
                {
                    g.DrawImage(sourceBitmap,
                       new System.Drawing.Rectangle(width - inset.Right, inset.Top, inset.Right, height - (inset.Top + inset.Bottom)),     //destination
                       new System.Drawing.Rectangle(sourceBitmap.Width - inset.Right, inset.Top, inset.Right, sourceBitmap.Height - (inset.Top + inset.Bottom)), //source
                       GraphicsUnit.Pixel);

                    //top right
                    if (inset.Top > 0)
                    {
                        g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(width - inset.Right, 0, inset.Right, inset.Top), //destination
                            new System.Drawing.Rectangle(sourceBitmap.Width - inset.Right, 0, inset.Right, inset.Top), //source
                            GraphicsUnit.Pixel);
                    }

                    //bottom right
                    if (inset.Bottom > 0)
                    {
                        g.DrawImage(sourceBitmap,
                        new System.Drawing.Rectangle(width - inset.Right, height - inset.Bottom, inset.Right, inset.Bottom),     //destination
                        new System.Drawing.Rectangle(sourceBitmap.Width - inset.Right, sourceBitmap.Height - inset.Bottom, inset.Right, inset.Bottom), //source
                        GraphicsUnit.Pixel);
                    }
                }
            }
            return outputBitmap;
        }*/

        System.Drawing.Color CanvasColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);

        public NineGridCanvas()
        {

        }

        public Thickness SizingMargin
        {
            get => (Thickness)GetValue(SizingMarginProperty);
            set => SetValue(SizingMarginProperty, value);
        }

        public static readonly DependencyProperty SizingMarginProperty = DependencyProperty.RegisterAttached(
        "SizingMargin", typeof(Thickness), typeof(NineGridCanvas), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, OnSizingMarginChanged));

        private static void OnSizingMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as NineGridCanvas).SetCanvasInfo((sender as NineGridCanvas).SizingImage, (Thickness)e.NewValue, new System.Windows.Size(0, 0));
        }

        public BitmapImage SizingImage
        {
            get
            {
                return (BitmapImage)GetValue(SizingImageProperty);
            }
            set
            {
                try
                {
                    SetValue(SizingImageProperty, value);
                }
                catch
                {
                    SetValue(SizingImageProperty, Start9.Api.Tools.MiscTools.GetBitmapImageFromSysDrawingBitmap(Start9.Api.Properties.Resources.FallbackImage));
                }
            }
        }
        public static readonly DependencyProperty SizingImageProperty = DependencyProperty.RegisterAttached(
        "SizingImage", typeof(BitmapImage), typeof(NineGridCanvas), new FrameworkPropertyMetadata(Start9.Api.Tools.MiscTools.GetBitmapImageFromSysDrawingBitmap(Start9.Api.Properties.Resources.FallbackImage), FrameworkPropertyMetadataOptions.AffectsRender, OnSizingImageChanged));


        private static void OnSizingImageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as NineGridCanvas).SetCanvasInfo((sender as NineGridCanvas).SizingImage, (sender as NineGridCanvas).SizingMargin, new System.Windows.Size(0, 0));
        }

        public double TargetWidth
        {
            get => (double)GetValue(TargetWidthProperty);
            set
            {
                if ((int)value > 0)
                {
                    SetValue(TargetWidthProperty, value);
                }
            }
        }

        public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.RegisterAttached(
        "TargetWidth", typeof(double), typeof(NineGridCanvas), new FrameworkPropertyMetadata((double)1, FrameworkPropertyMetadataOptions.AffectsRender, OnTargetWidthChanged));


        private static void OnTargetWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as NineGridCanvas).SetCanvasInfo((sender as NineGridCanvas).SizingImage, (sender as NineGridCanvas).SizingMargin, new System.Windows.Size((double)e.NewValue, (sender as NineGridCanvas).TargetHeight));
        }

        public double TargetHeight
        {
            get => (double)GetValue(TargetHeightProperty);
            set
            {
                if ((int)value > 0)
                {
                    SetValue(TargetHeightProperty, value);
                }
            }
        }

        public static readonly DependencyProperty TargetHeightProperty = DependencyProperty.RegisterAttached(
        "TargetHeight", typeof(double), typeof(NineGridCanvas), new FrameworkPropertyMetadata((double)1, FrameworkPropertyMetadataOptions.AffectsRender, OnTargetHeightChanged));


        private static void OnTargetHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as NineGridCanvas).SetCanvasInfo((sender as NineGridCanvas).SizingImage, (sender as NineGridCanvas).SizingMargin, new System.Windows.Size((sender as NineGridCanvas).TargetWidth, (double)e.NewValue));
        }

        private void SetCanvasInfo(BitmapImage Image, Thickness ThicknessSizing, System.Windows.Size ControlSize)
        {
            if ((Image != null) & (ThicknessSizing != null) & (ControlSize != null))
                SetCanvasInfo(ConvertBitmapImageToBitmap(Image), ThicknessSizing, ControlSize);
        }

        private void SetCanvasInfo(System.Drawing.Bitmap Image, Thickness ThicknessSizing, System.Windows.Size ControlSize)
        {
            if ((Image != null) & (ThicknessSizing != null) & (ControlSize != null))
            {
                Bitmap PostNineGridBitmap = InsetResize(Image, new System.Drawing.Size((int)TargetWidth, (int)TargetHeight), ThicknessSizing);
                Background = new ImageBrush(GetBitmapImageFromSysDrawingBitmap(PostNineGridBitmap));
            }
        }

        private BitmapImage GetBitmapImageFromSysDrawingBitmap(System.Drawing.Bitmap SourceSysDrawingBitmap)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                SourceSysDrawingBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = memory;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            return bitmap;
        }

        private BitmapSource GetBitmapSourceFromSysDrawingBitmap(System.Drawing.Bitmap SourceSysDrawingBitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            SourceSysDrawingBitmap.GetHbitmap(),
            IntPtr.Zero,
            System.Windows.Int32Rect.Empty,
            BitmapSizeOptions.FromWidthAndHeight(SourceSysDrawingBitmap.Width, SourceSysDrawingBitmap.Height));
        }

        private void NineGridCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetCanvasInfo(ConvertBitmapImageToBitmap(SizingImage), (sender as NineGridCanvas).SizingMargin, new System.Windows.Size(ActualWidth, ActualHeight));
        }

        //from https://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa
        private Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {

                try
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                    enc.Save(outStream);
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                    return new Bitmap(bitmap);
                }
                catch
                {
                    return null;
                }
            }
        }

        //Needed for dejco's thing to work
        private struct Inset
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        };

        //Huge thanccs to dejco for this part
        private Bitmap InsetResize(Bitmap sourceBitmap, System.Drawing.Size destSize, Thickness insetThickness)
        {
            Bitmap outputBitmap = new Bitmap(destSize.Width, destSize.Height);
            try
            {
                Inset inset = new Inset()
                {
                    Left = (int)insetThickness.Left,
                    Top = (int)insetThickness.Top,
                    Right = (int)insetThickness.Right,
                    Bottom = (int)insetThickness.Bottom
                };

                using (Graphics g = Graphics.FromImage(outputBitmap))
                {
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 0, 0, 0)), 0, 0, destSize.Width, destSize.Height);

                    g.DrawImage(sourceBitmap,
                      new System.Drawing.Rectangle(inset.Left, inset.Top, destSize.Width - (inset.Left + inset.Right), destSize.Height - (inset.Top + inset.Bottom)),     //destination
                      new System.Drawing.Rectangle(inset.Left, inset.Top, sourceBitmap.Width - (inset.Left + inset.Right), sourceBitmap.Height - (inset.Top + inset.Bottom)), //source
                      GraphicsUnit.Pixel);

                    if (inset.Top > 0) //Top
                    {
                        g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(inset.Left, 0, destSize.Width - (inset.Left + inset.Right), inset.Top), //destination
                            new System.Drawing.Rectangle(inset.Left, 0, sourceBitmap.Width - (inset.Left + inset.Right), inset.Top), //source
                            GraphicsUnit.Pixel);
                    }

                    if (inset.Bottom > 0)
                    {
                        g.DrawImage(sourceBitmap,
                       new System.Drawing.Rectangle(inset.Left, destSize.Height - inset.Bottom, destSize.Width - (inset.Left + inset.Right), inset.Bottom),     //destination
                       new System.Drawing.Rectangle(inset.Left, sourceBitmap.Height - inset.Bottom, sourceBitmap.Width - (inset.Left + inset.Right), inset.Bottom), //source
                       GraphicsUnit.Pixel);
                    }

                    if (inset.Left > 0)
                    {
                        //left
                        g.DrawImage(sourceBitmap,
                           new System.Drawing.Rectangle(0, inset.Top, inset.Left, destSize.Height - (inset.Top + inset.Bottom)),     //destination
                           new System.Drawing.Rectangle(0, inset.Top, inset.Left, sourceBitmap.Height - (inset.Top + inset.Bottom)), //source
                           GraphicsUnit.Pixel);

                        //top left
                        if (inset.Top > 0)
                        {
                            g.DrawImage(sourceBitmap,
                                new System.Drawing.Rectangle(0, 0, inset.Left, inset.Top), //destination
                                new System.Drawing.Rectangle(0, 0, inset.Left, inset.Top), //source
                                GraphicsUnit.Pixel);
                        }

                        //bottom left
                        if (inset.Bottom > 0)
                        {
                            g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(0, destSize.Height - inset.Bottom, inset.Left, inset.Bottom),     //destination
                            new System.Drawing.Rectangle(0, sourceBitmap.Height - inset.Bottom, inset.Left, inset.Bottom), //source
                            GraphicsUnit.Pixel);
                        }
                    }

                    if (inset.Right > 0) //Right
                    {
                        g.DrawImage(sourceBitmap,
                           new System.Drawing.Rectangle(destSize.Width - inset.Right, inset.Top, inset.Right, destSize.Height - (inset.Top + inset.Bottom)),     //destination
                           new System.Drawing.Rectangle(sourceBitmap.Width - inset.Right, inset.Top, inset.Right, sourceBitmap.Height - (inset.Top + inset.Bottom)), //source
                           GraphicsUnit.Pixel);

                        //top right
                        if (inset.Top > 0)
                        {
                            g.DrawImage(sourceBitmap,
                                new System.Drawing.Rectangle(destSize.Width - inset.Right, 0, inset.Right, inset.Top), //destination
                                new System.Drawing.Rectangle(sourceBitmap.Width - inset.Right, 0, inset.Right, inset.Top), //source
                                GraphicsUnit.Pixel);
                        }

                        //bottom right
                        if (inset.Bottom > 0)
                        {
                            g.DrawImage(sourceBitmap,
                            new System.Drawing.Rectangle(destSize.Width - inset.Right, destSize.Height - inset.Bottom, inset.Right, inset.Bottom),     //destination
                            new System.Drawing.Rectangle(sourceBitmap.Width - inset.Right, sourceBitmap.Height - inset.Bottom, inset.Right, inset.Bottom), //source
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
