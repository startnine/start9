using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Timer = System.Timers.Timer;
using System.Windows.Media.Animation;
using Start9.Api.Controls;
using System.Windows.Media;
using static Start9.Api.Extensions;
using System.Windows.Input;
using System.Diagnostics;

namespace Start9.Host.Controls
{
    [TemplatePart(Name = PartPerspective, Type = typeof(Planerator))]
    public class MarketItem : Button
    {
        const String PartPerspective = "PART_Perspective";
        const String PartOuterPathGeometry = "PART_OuterPathGeometry";

        public String Title
        {
            get => (String)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(MarketItem), new PropertyMetadata(""));

        public String Author
        {
            get => (String)GetValue(AuthorProperty);
            set => SetValue(AuthorProperty, value);
        }

        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(String), typeof(MarketItem), new PropertyMetadata(""));

        public String Description
        {
            get => (String)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(String), typeof(MarketItem), new PropertyMetadata(""));

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(MarketItem), new PropertyMetadata());

        double RotationMultiplier
        {
            get => (double)GetValue(RotationMultiplierProperty);
            set => SetValue(RotationMultiplierProperty, value);
        }

        public static readonly DependencyProperty RotationMultiplierProperty = DependencyProperty.Register("RotationMultiplier", typeof(double), typeof(MarketItem), new PropertyMetadata((double)0));

        Timer RotateTimer = new Timer()
        {
            Interval = 10
        };

        public MarketItem()
        {
            DefaultStyleKey = typeof(MarketItem);
            Loaded += MarketItem_Loaded;
            Unloaded += MarketItem_Unloaded;
            IsVisibleChanged += MarketItem_IsVisibleChanged;
            SizeChanged += MarketItem_SizeChanged;
        }

        Planerator _perspective;
        PathGeometry _outerPathGeometry;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _perspective = GetTemplateChild(PartPerspective) as Planerator;

            try
            {
                _outerPathGeometry = (PathGeometry)(Template.Resources[PartOuterPathGeometry]);
            }
            catch
            {
                _outerPathGeometry = null;
            }
        }

        private void MarketItem_Loaded(Object sender, RoutedEventArgs e)
        {
            SetBorder();
            RotateTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if ((_perspective != null) && (IsVisible))
                    {
                        Point offset = this.GetOffsetFromCursor();
                        var point = new Point((offset.Y + ((ActualHeight / 2) * -1)) * RotationMultiplier, (offset.X + ((ActualWidth / 2) * -1)) * RotationMultiplier);
                        _perspective.RotationX = point.X;
                        _perspective.RotationY = point.Y;
                    }
                    else
                    {
                        RotateTimer.Stop();
                    }
                }));
            };
            RotateTimer.Start();
        }

        private void MarketItem_Unloaded(object sender, RoutedEventArgs e)
        {
            RotateTimer.Stop();
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
            if (_outerPathGeometry != null)
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
                    },////
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

                var figure = new PathFigure()
                {
                    StartPoint = new Point(0, 10),
                    Segments = pathSegments
                };


                string s = "Figure: ";
                foreach (var p in pathSegments)
                {
                    if (p is LineSegment)
                        s += "L " + (p as LineSegment).Point + " ";
                    else if (p is QuadraticBezierSegment)
                        s += "Q " + (p as QuadraticBezierSegment).Point1 + " " + (p as QuadraticBezierSegment).Point2 + " ";

                    s += "Z";
                }

                Debug.WriteLine(s); //Figure: (2.5,2.5,10,0) (115,0) (122.5,2.5,125,5) (150,35) (152.5,36,160,37.5) (233,37.5) (240.5,39.5,243,47.5) (243,166) (240.5,173.5,233,176) (10,176) (2.5,173.5,0,166) (0,10) (2.5,2.5,10,0)

                var collection = new PathFigureCollection()
                {
                    figure
                };

                _outerPathGeometry.Figures = collection;
            }
        }

        private void MarketItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                RotateTimer.Start();
                SetBorder();
            }
        }
    }
}
