using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Timers;
using Timer = System.Windows.Forms.Timer;
using Start9.Api;
using Start9.Api.Tools;
using Start9.Api.Controls;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace Start9.UserControls
{
    /// <summary>
    /// Interaction logic for MarketItem.xaml
    /// </summary>
    public partial class MarketItem : UserControl
    {
        Timer RotateTimer = new Timer()
        {
            Interval = 10
        };

        TimeSpan rotateTime = TimeSpan.FromMilliseconds(400);

        QuinticEase rotateEase = new QuinticEase()
        {
            EasingMode = EasingMode.EaseInOut
        };

        DoubleAnimation resetRotationAnim = new DoubleAnimation()
        {
            To = 0
        };

        TimeSpan scaleTime = TimeSpan.FromMilliseconds(200);

        QuinticEase scaleEase = new QuinticEase()
        {
            EasingMode = EasingMode.EaseOut
        };

        DoubleAnimation scaleDownAnim = new DoubleAnimation()
        {
            To = 0.875
        };

        DoubleAnimation scaleUpAnim = new DoubleAnimation()
        {
            To = 1
        };

        public MarketItem()
        {
            InitializeComponent();
            RotateTimer.Tick += delegate
            {
                var point = GetPerspectiveRotation();
                Perspective.RotationX = point.X;
                Perspective.RotationY = point.Y;
                if (!IsMouseOver)
                    MarketItem_MouseLeave(this, null);
            };
            resetRotationAnim.EasingFunction = rotateEase;
            resetRotationAnim.Duration = rotateTime;
            resetRotationAnim.Completed += delegate
            {
                Perspective.BeginAnimation(Planerator.RotationXProperty, null);
                Perspective.BeginAnimation(Planerator.RotationYProperty, null);
                Perspective.RotationX = 0;
                Perspective.RotationY = 0;
            };
            scaleUpAnim.EasingFunction = scaleEase;
            scaleUpAnim.Duration = scaleTime;
            scaleDownAnim.EasingFunction = scaleEase;
            scaleDownAnim.Duration = scaleTime;
        }

        public Point GetPerspectiveRotation()
        {
            Point point = this.GetOffsetFromCursor();//).X, MainTools.GetDpiScaledGlobalControlPosition(Perspective).Y);
            //Point center = new Point(point.X + (ActualWidth / 2), point.Y + (ActualHeight / 2));
            //return new Point((SystemScaling.CursorPosition.Y - center.Y) / -4, (SystemScaling.CursorPosition.X - center.X) / -4);
            return new Point((point.Y + ((ActualHeight / 2) * -1)) / -4, (point.X + ((ActualWidth / 2) * -1)) / -4);
        }

        private void ResetPerspective()
        {
            Perspective.BeginAnimation(Planerator.RotationXProperty, resetRotationAnim);
            Perspective.BeginAnimation(Planerator.RotationYProperty, resetRotationAnim);
        }

        private void MarketItem_Loaded(Object sender, RoutedEventArgs e)
        {
            SetBorder();
        }

        private void MarketItem_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            SetBorder();
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            SetBorder();
        }

        public void SetBorder()
        {
            Double width = this.ActualWidth - (24);
            Double height = this.ActualHeight - (24);

            PathSegmentCollection pathSegments = new PathSegmentCollection()
            {
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(2.5,2.5),
                    Point2 = new Point(10,0)
                },
                new LineSegment()
                {
                    Point = new Point(115,0)
                },
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(122.5,2.5),
                    Point2 = new Point(125,5)
                },
                new LineSegment()
                {
                    Point = new Point(150,35)
                },
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(152.5,36),
                    Point2 = new Point(160,37.5)
                },
                new LineSegment()
                {
                    Point = new Point(width - 10,37.5)
                },
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(width - 2.5,39.5),
                    Point2 = new Point(width,47.5)
                },
                new LineSegment()
                {
                    Point = new Point(width,height - 10)
                },
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(width - 2.5,height - 2.5),
                    Point2 = new Point(width - 10,height)
                },
                new LineSegment()
                {
                    Point = new Point(10,height)
                },
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(2.5,height - 2.5),
                    Point2 = new Point(0,height - 10)
                },
                new LineSegment()
                {
                    Point = new Point(0,10)
                },
                new QuadraticBezierSegment()
                {
                    Point1 = new Point(2.5,2.5),
                    Point2 = new Point(10,0)
                }
            };

            foreach (PathSegment p in pathSegments)
            {
                p.IsStroked = true;
                p.IsSmoothJoin = true;
            }

            (Resources["OuterPathGeometry"] as PathGeometry).Figures = new PathFigureCollection()
            {
                new PathFigure()
                {
                    StartPoint = new Point(0,10),
                    Segments = pathSegments
                }
            };
        }

        private void MarketItem_MouseEnter(Object sender, MouseEventArgs e)
        {
            var point = GetPerspectiveRotation();
            DoubleAnimation xRotationAnim = new DoubleAnimation()
            {
                To = point.X,
                Duration = rotateTime,
                EasingFunction = rotateEase
            };
            DoubleAnimation yRotationAnim = new DoubleAnimation()
            {
                To = point.Y,
                Duration = rotateTime,
                EasingFunction = rotateEase
            };
            yRotationAnim.Completed += delegate
            {
                RotateTimer.Start();
                Perspective.BeginAnimation(Planerator.RotationXProperty, null);
                Perspective.BeginAnimation(Planerator.RotationYProperty, null);
            };
            Perspective.BeginAnimation(Planerator.RotationXProperty, xRotationAnim);
            Perspective.BeginAnimation(Planerator.RotationYProperty, yRotationAnim);
        }

        private void MarketItem_MouseLeave(Object sender, MouseEventArgs e)
        {
            RotateTimer.Stop();
            ResetPerspective();
        }

        private void MarketItem_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            Scaler.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDownAnim);
            Scaler.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDownAnim);
        }

        private void MarketItem_MouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            Scaler.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUpAnim);
            Scaler.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUpAnim);
        }
    }
}
