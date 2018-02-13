using Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;
using Start9.Api.Tools;
using System.Text;

namespace Start9.Api.Plex
{
    [TemplatePart(Name = PartTitlebar, Type = typeof(Grid))]
    [TemplatePart(Name = PartMinimizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PartMaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PartRestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PartThumbBottom, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTop, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbBottomRightCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTopRightCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTopLeftCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbBottomLeftCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbRight, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbLeft, Type = typeof(Thumb))]
    public class PlexWindow : Window
    {
        const string PartTitlebar = "PART_Titlebar";
        const string PartMinimizeButton = "PART_MinimizeButton";
        const string PartMaximizeButton = "PART_MaximizeButton";
        const string PartRestoreButton = "PART_RestoreButton";
        const string PartCloseButton = "PART_CloseButton";
        const string PartThumbBottom = "PART_ThumbBottom";
        const string PartThumbTop = "PART_ThumbTop";
        const string PartThumbBottomRightCorner = "PART_ThumbBottomRightCorner";
        const string PartResizeGrip = "PART_ResizeGrip";
        const string PartThumbTopRightCorner = "PART_ThumbTopRightCorner";
        const string PartThumbTopLeftCorner = "PART_ThumbTopLeftCorner";
        const string PartThumbBottomLeftCorner = "PART_ThumbBottomLeftCorner";
        const string PartThumbRight = "PART_ThumbRight";
        const string PartThumbLeft = "PART_ThumbLeft";

        public bool AnimateOnShowHide
        {
            get => (bool)GetValue(AnimateOnShowHideProperty);
            set => SetValue(AnimateOnShowHideProperty, (value));
        }

        public static readonly DependencyProperty AnimateOnShowHideProperty =
            DependencyProperty.Register("AnimateOnShowHide", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public double ShadowOpacity
        {
            get
            {
                if (WindowState == WindowState.Normal)
                {
                    ShiftShadowBehindWindow();
                    return (double)GetValue(ShadowOpacityProperty);
                }
                else
                {
                    return (double)0;
                }
            }
            set => SetValue(ShadowOpacityProperty, (value * Opacity));
        }

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.RegisterAttached("ShadowOpacity", typeof(double), typeof(PlexWindow), new PropertyMetadata((double)1));

        new public PlexResizeMode ResizeMode
        {
            get => (PlexResizeMode)GetValue(ResizeModeProperty);
            set => SetValue(ResizeModeProperty, value);
        }

        new public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(PlexResizeMode), typeof(PlexWindow), new PropertyMetadata(PlexResizeMode.CanResize, OnResizeModePropertyChangedCallback));

        static void OnResizeModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int GWL_STYLE = (-16);
            var window = (PlexWindow)d;
            var hwnd = new WindowInteropHelper(window).Handle;

            if (((window.ResizeMode == PlexResizeMode.Manual) & (!(window.ShowMaxRestButton))) | ((window.ResizeMode == PlexResizeMode.CanResize) | (window.ResizeMode == PlexResizeMode.CanResizeWithGrip)))
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16CF0000);
            }
            else if (((window.ResizeMode == PlexResizeMode.Manual) & (window.ShowMinButton)) | (window.ResizeMode == PlexResizeMode.CanMinimize))
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16CA0000);
            }
            else
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16C80000);
            }
        }

        public static readonly DependencyProperty MaximizedProperty =
            DependencyProperty.Register("Maximized", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty MinimizedProperty =
            DependencyProperty.Register("Minimized", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty WindowRectProperty = DependencyProperty.Register("WindowRect", typeof(Rect),
            typeof(PlexWindow), new PropertyMetadata(new Rect(), OnWindowRectPropertyChangedCallback));

        static void OnWindowRectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinApi.MoveWindow(new WindowInteropHelper((PlexWindow)d).Handle, (int)((Rect)e.NewValue).Left, (int)((Rect)e.NewValue).Top, (int)((Rect)e.NewValue).Width, (int)((Rect)e.NewValue).Height, true);
        }

        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.RegisterAttached("TitleBarContent", typeof(object), typeof(PlexWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ToolBarContentProperty =
            DependencyProperty.RegisterAttached("ToolBarContent", typeof(object), typeof(PlexWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FooterContentProperty = DependencyProperty.RegisterAttached("FooterContent",
            typeof(object), typeof(PlexWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register("TitleBarHeight",
            typeof(double), typeof(PlexWindow),
            new FrameworkPropertyMetadata((double)24, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ToolBarHeightProperty = DependencyProperty.Register("ToolBarHeight",
            typeof(double), typeof(PlexWindow),
            new FrameworkPropertyMetadata((double)42, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FooterHeightProperty = DependencyProperty.Register("FooterHeight",
            typeof(double), typeof(PlexWindow),
            new FrameworkPropertyMetadata((double)36, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTitleBarProperty =
            DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowToolBarProperty =
            DependencyProperty.Register("ShowToolBar", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty ShowFooterProperty =
            DependencyProperty.Register("ShowFooter", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty BodyBrushProperty = DependencyProperty.Register("BodyBrush", typeof(Brush),
            typeof(PlexWindow), new PropertyMetadata(
                new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops = new GradientStopCollection(new List<GradientStop>
                    {
                        new GradientStop
                        {
                            Offset = 0,
                            Color = Colors.White
                        },
                        new GradientStop
                        {
                            Offset = 1,
                            Color = Color.FromArgb(0xFF, 0xC8, 0xD4, 0xE7)
                        }
                    })
                }
            ));

        public static readonly DependencyProperty FooterBrushProperty = DependencyProperty.Register("FooterBrush",
            typeof(Brush), typeof(PlexWindow),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x5E, 0x98, 0xD9))));

        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowMaxRestButton
        {
            get => (bool)GetValue(ShowMaxRestButtonProperty);
            set => SetValue(ShowMaxRestButtonProperty, value);
        }

        public static readonly DependencyProperty ShowMaxRestButtonProperty =
            DependencyProperty.Register("ShowMaxRestButton", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowMinButton
        {
            get => (bool)GetValue(ShowMinButtonProperty);
            set => SetValue(ShowMinButtonProperty, value);
        }

        public static readonly DependencyProperty ShowMinButtonProperty =
            DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowResizeEdges
        {
            get => (bool)GetValue(ShowResizeEdgesProperty);
            set => SetValue(ShowResizeEdgesProperty, value);
        }

        public static readonly DependencyProperty ShowResizeEdgesProperty =
            DependencyProperty.Register("ShowResizeEdges", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowResizeGrip
        {
            get => (bool)GetValue(ShowResizeGripProperty);
            set => SetValue(ShowResizeGripProperty, value);
        }

        public static readonly DependencyProperty ShowResizeGripProperty =
            DependencyProperty.Register("ShowResizeGrip", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        TimeSpan AnimateInDuration = TimeSpan.FromMilliseconds(500);
        TimeSpan AnimateMidDuration = TimeSpan.FromMilliseconds(500);
        TimeSpan AnimateOutDuration = TimeSpan.FromMilliseconds(1000);

        /*static PlexWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlexWindow), new FrameworkPropertyMetadata(typeof(PlexWindow)));
        }*/

        /// <summary>
        ///     Interaction logic for PlexWindow.xaml
        /// </summary>
        public PlexWindow()
        {
            /*ResourceDictionary genericStyles = new ResourceDictionary()
            {
                Source = new Uri("/Start9.Api;component/Themes/Generic.xaml", UriKind.Relative)
            };*/

            ResourceDictionary plexStyles = new ResourceDictionary()
            {
                Source = new Uri("/Start9.Api;component/Themes/Generic.xaml", UriKind.Relative)
            };

            if (!(Resources.MergedDictionaries.Contains(plexStyles)))
            {
                Resources.MergedDictionaries.Add(plexStyles);
                Style = (Style)Resources["PlexWindowStyle"];
            }

            /*bool doesNotHavePlexStyles = false;
            bool doesNotHaveGenericStyles = false;

            if (!(Resources.MergedDictionaries.Contains(plexStyles)))
            {
                Resources.MergedDictionaries.Add(plexStyles);
                doesNotHavePlexStyles = true;
            }

            if (!(Resources.MergedDictionaries.Contains(genericStyles)))
            {
                Resources.MergedDictionaries.Add(genericStyles);
                doesNotHaveGenericStyles = true;
            }

            if (doesNotHavePlexStyles | doesNotHaveGenericStyles | (doesNotHavePlexStyles & doesNotHaveGenericStyles))
            {

                Style = (Style)Resources["PlexWindowStyle"];
            }*/

            //DefaultStyleKeyProperty.OverrideMetadata(typeof(PlexWindow), new FrameworkPropertyMetadata(typeof(PlexWindow)));

            //Opacity = 0;
            _shadowWindow = new ShadowWindow(this);
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            ScaleTransform ScaleTransform = new ScaleTransform()
            {
                CenterX = (ActualWidth / 2),
                CenterY = (ActualHeight / 2),
                ScaleX = 1,
                ScaleY = 1
            };
            RenderTransform = ScaleTransform;
            //Opacity = 0;
            Loaded += PlexWindow_Loaded;
            IsVisibleChanged += PlexWindow_IsVisibleChanged;
            Activated += PlexWindow_WindowFocusChanged;
            Deactivated += PlexWindow_WindowFocusChanged;
            var restoreMinSettings = new RoutedCommand();
            restoreMinSettings.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Windows));
            CommandBindings.Add(new CommandBinding(restoreMinSettings, RestoreMinimizeWindow));
            Closing += PlexWindow_Closing;
            Closed += PlexWindow_Closed;
            MouseEnter += PlexWindow_MouseTransfer;
            MouseLeave += PlexWindow_MouseTransfer;
        }

        private void PlexWindow_MouseTransfer(object sender, MouseEventArgs e)
        {
            ShiftShadowBehindWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ShiftShadowBehindWindow();
        }

        public bool Maximized
        {
            get => (bool)GetValue(MaximizedProperty);
            set => SetValue(MaximizedProperty, value);
        }

        public bool Minimized
        {
            get => (bool)GetValue(MinimizedProperty);
            set => SetValue(MinimizedProperty, value);
        }

        public Rect WindowRect
        {
            get => (Rect)GetValue(WindowRectProperty);
            set => SetValue(WindowRectProperty, value);
        }

        public object TitleBarContent
        {
            get => GetValue(TitleBarContentProperty);
            set => SetValue(TitleBarContentProperty, value);
        }

        public object ToolBarContent
        {
            get => GetValue(ToolBarContentProperty);
            set => SetValue(ToolBarContentProperty, value);
        }

        public object FooterContent
        {
            get => GetValue(FooterContentProperty);
            set
            {
                SetCurrentValue(FooterContentProperty, value);
                SetValue(FooterContentProperty, value);
            }
        }

        public double TitleBarHeight
        {
            get => (double)GetValue(TitleBarHeightProperty);
            set => SetValue(TitleBarHeightProperty, value);
        }

        public double ToolBarHeight
        {
            get => (double)GetValue(ToolBarHeightProperty);
            set => SetValue(ToolBarHeightProperty, value);
        }

        public double FooterHeight
        {
            get => (double)GetValue(FooterHeightProperty);
            set => SetValue(FooterHeightProperty, value);
        }
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        public bool ShowTitleBar
        {
            get => (bool)GetValue(ShowTitleBarProperty);
            set => SetValue(ShowTitleBarProperty, value);
        }

        public bool ShowToolBar
        {
            get => (bool)GetValue(ShowToolBarProperty);
            set => SetValue(ShowToolBarProperty, value);
        }

        public bool ShowFooter
        {
            get => (bool)GetValue(ShowFooterProperty);
            set => SetValue(ShowFooterProperty, value);
        }

        public Brush BodyBrush
        {
            get => (Brush)GetValue(BodyBrushProperty);
            set => SetValue(BodyBrushProperty, value);
        }
        
        public Brush FooterBrush
        {
            get => (Brush)GetValue(FooterBrushProperty);
            set => SetValue(FooterBrushProperty, value);
        }

        readonly ShadowWindow _shadowWindow;

        LinearGradientBrush _bodyLinearGradientBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1),
            GradientStops = new GradientStopCollection(new List<GradientStop>
            {
                new GradientStop
                {
                    Offset = 0,
                    Color = Colors.White
                },
                new GradientStop
                {
                    Offset = 1,
                    Color = Color.FromArgb(0xFF, 0xC8, 0xD4, 0xE7)
                }
            })
        };

        Button _closeButton;
        Button _maxButton;
        Button _minButton;
        Button _restButton;
        
        Thickness _shadowOffsetThickness = new Thickness(49, 14, 14, 60);
        Thumb _thumbBottom;
        Thumb _thumbBottomLeftCorner;
        Thumb _thumbBottomRightCorner;
        Thumb _resizeGrip;
        Thumb _thumbLeft;
        Thumb _thumbRight;
        Thumb _thumbTop;
        Thumb _thumbTopLeftCorner;
        Thumb _thumbTopRightCorner;

        Grid _titlebar;

        void RestoreMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
            }
        }

        void PlexWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SyncShadowToWindow();
            SyncShadowToWindowSize();
            if (IsVisible)
            {
                Show();
            }
        }

        new public void Show()
        {
            ShiftShadowBehindWindow();
            base.Show();
            ShiftShadowBehindWindow();
            ShowWindow();
            ShiftShadowBehindWindow();
        }

        private void ShowWindow()
        {
            if (AnimateOnShowHide)
            {
                ShiftShadowBehindWindow();
                CircleEase circleEase = new CircleEase()
                {
                    EasingMode = EasingMode.EaseOut
                };
                var scaleTransform = (this.RenderTransform as ScaleTransform);
                scaleTransform.CenterX = (ActualWidth / 2);
                scaleTransform.CenterY = (ActualHeight / 2);

                DoubleAnimation windowOpacityAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = AnimateInDuration,
                    EasingFunction = circleEase
                };

                DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                {
                    From = 0.75,
                    To = 1,
                    Duration = AnimateInDuration,
                    EasingFunction = circleEase
                };
                windowOpacityAnimation.Completed += (sendurr, args) =>
                {
                    ShiftShadowBehindWindow();
                    PlexWindow_ResetTransformProperties();
                };

                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
                BeginAnimation(Window.OpacityProperty, windowOpacityAnimation);
                ShiftShadowBehindWindow();
            }
            else
            {
                ShiftShadowBehindWindow();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            try
            {
                _titlebar = GetTemplateChild(PartTitlebar) as Grid;
                _titlebar.MouseLeftButtonDown += Titlebar_MouseLeftButtonDown;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("TITLEBAR \n" + ex);
            }

            try
            {
            _minButton = GetTemplateChild(PartMinimizeButton) as Button;
            _minButton.Click += delegate { WindowState = WindowState.Minimized; };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("MINBUTTON \n" + ex);
            }

            try
            {
            _maxButton = GetTemplateChild(PartMaximizeButton) as Button;
            _maxButton.Click += delegate
            {
                WindowState = WindowState.Maximized;
            };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("MAXBUTTON \n" + ex);
            }

            try
            {
            _restButton = GetTemplateChild(PartRestoreButton) as Button;
            _restButton.Click += delegate
            {
                WindowState = WindowState.Normal;
            };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("RESTBUTTON \n" + ex);
            }

            try
            {
            _closeButton = GetTemplateChild(PartCloseButton) as Button;
            _closeButton.Click += delegate
            {
                Close();
            };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("CLOSEBUTTON \n" + ex);
            }


            try
            {
            _thumbBottom = GetTemplateChild(PartThumbBottom) as Thumb;
            _thumbBottom.DragDelta += ThumbBottom_DragDelta;


            _thumbTop = GetTemplateChild(PartThumbTop) as Thumb;
            _thumbTop.DragDelta += ThumbTop_DragDelta;


            _thumbBottomRightCorner = GetTemplateChild(PartThumbBottomRightCorner) as Thumb;
            _thumbBottomRightCorner.DragDelta += ThumbBottomRightCorner_DragDelta;


            _thumbTopRightCorner = GetTemplateChild(PartThumbTopRightCorner) as Thumb;
            _thumbTopRightCorner.DragDelta += ThumbTopRightCorner_DragDelta;


            _thumbTopLeftCorner = GetTemplateChild(PartThumbTopLeftCorner) as Thumb;
            _thumbTopLeftCorner.DragDelta += ThumbTopLeftCorner_DragDelta;


            _thumbBottomLeftCorner = GetTemplateChild(PartThumbBottomLeftCorner) as Thumb;
            _thumbBottomLeftCorner.DragDelta += ThumbBottomLeftCorner_DragDelta;


            _thumbRight = GetTemplateChild(PartThumbRight) as Thumb;
            _thumbRight.DragDelta += ThumbRight_DragDelta;


            _thumbLeft = GetTemplateChild(PartThumbLeft) as Thumb;
            _thumbLeft.DragDelta += ThumbLeft_DragDelta;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("THUMBS \n" + ex);
            }

            try
            {
            _resizeGrip = GetTemplateChild(PartResizeGrip) as Thumb;
            _resizeGrip.DragDelta += ThumbBottomRightCorner_DragDelta;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("RESIZEGRIP \n" + ex);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            SyncShadowToWindow();
        }

        private void HideWindow()
        {
            base.Hide();
        }

        new public void Hide()
        {
            ShiftShadowBehindWindow();
            if (AnimateOnShowHide)
            {
                QuinticEase circleEase = new QuinticEase()
                {
                    EasingMode = EasingMode.EaseInOut
                };
                var scaleTransform = (this.RenderTransform as ScaleTransform);
                scaleTransform.CenterX = (ActualWidth / 2);
                scaleTransform.CenterY = (ActualHeight / 2);

                DoubleAnimation windowOpacityAnimation = new DoubleAnimation()
                {
                    From = 1,
                    To = 0,
                    Duration = AnimateOutDuration,
                    EasingFunction = circleEase
                };

                DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                {
                    From = 1,
                    To = 0.75,
                    Duration = AnimateOutDuration,
                    EasingFunction = circleEase
                };
                windowOpacityAnimation.Completed += (sendurr, args) =>
                {
                    HideWindow();
                };
                BeginAnimation(Window.OpacityProperty, windowOpacityAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            }
            else
            {
                HideWindow();
            }
        }

        bool isHiding = false;

        private void PlexWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                ShiftShadowBehindWindow();
                Show();
                ShiftShadowBehindWindow();
            }
            else if (!isHiding)
            {
                Hide();
                isHiding = true;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }

            ShiftShadowBehindWindow();
        }

        bool IsClosingNow = false;

        private void PlexWindow_Closing(object sender, CancelEventArgs e)
        {
            bool eCancel = e.Cancel;

            if (!eCancel & !IsClosingNow)
            {
                e.Cancel = true;
                IsClosingNow = true;
            }

            QuinticEase circleEase = new QuinticEase()
            {
                EasingMode = EasingMode.EaseInOut
            };
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = (ActualHeight / 2);

            DoubleAnimation windowOpacityAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = AnimateOutDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0.75,
                Duration = AnimateOutDuration,
                EasingFunction = circleEase
            };
            windowOpacityAnimation.Completed += (sendurr, args) =>
            {
                Close();
            };

            if (!eCancel)
            {
                BeginAnimation(Window.OpacityProperty, windowOpacityAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            }
        }

        private void PlexWindow_Closed(object sender, EventArgs e)
        {
            _shadowWindow.Close();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            Screen s = Screen.FromHandle(hwnd);
            if (WindowState == WindowState.Maximized)
            {
                Maximized = true;
                _maxButton.Visibility = Visibility.Hidden;
                _restButton.Visibility = Visibility.Visible;
                MaxWidth = Tools.DpiManager.ConvertPixelsToWpfUnits(s.WorkingArea.Width);
                MaxHeight = Tools.DpiManager.ConvertPixelsToWpfUnits(s.WorkingArea.Height);
                CircleEase circleEase = new CircleEase()
                {
                    EasingMode = EasingMode.EaseOut
                };
                var scaleTransform = (this.RenderTransform as ScaleTransform);
                scaleTransform.CenterX = (ActualWidth / 2);
                scaleTransform.CenterY = (ActualHeight / 2);

                DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                {
                    From = 0.75,
                    To = 1,
                    Duration = AnimateMidDuration,
                    EasingFunction = circleEase
                };
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            }
            else
            {
                _maxButton.Visibility = Visibility.Visible;
                _restButton.Visibility = Visibility.Hidden;
                Padding = new Thickness(0, 0, 0, 0);
                MaxWidth = double.PositiveInfinity;
                MaxHeight = double.PositiveInfinity;

                if (WindowState == WindowState.Minimized)
                {
                    if (!Minimized)
                    {
                        if (Maximized)
                        {
                            WindowState = WindowState.Maximized;
                        }
                        else
                        {
                            WindowState = WindowState.Normal;
                        }
                        PlexWindow_AnimateMinimize();
                        Minimized = true;
                    }
                }
                else
                {
                    if (Minimized)
                    {
                        PlexWindow_AnimateRestoreUp();
                    }
                    else if (Maximized)
                    {
                        CircleEase circleEase = new CircleEase()
                        {
                            EasingMode = EasingMode.EaseOut
                        };
                        var scaleTransform = (this.RenderTransform as ScaleTransform);
                        scaleTransform.CenterX = (ActualWidth / 2);
                        scaleTransform.CenterY = (ActualHeight / 2);

                        DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                        {
                            From = 1.333,
                            To = 1,
                            Duration = AnimateMidDuration,
                            EasingFunction = circleEase
                        };
                        DoubleAnimation windowVertSizeAnimation = new DoubleAnimation()
                        {
                            From = 1.333,
                            To = 1,
                            Duration = AnimateMidDuration,
                            EasingFunction = circleEase
                        };
                        Rect windowRect = WindowRect;

                        windowVertSizeAnimation.Completed += delegate
                        {
                            WindowRect = new Rect(Left + 100, Top + 100, Width - 200, Height - 200);
                            ShiftShadowBehindWindow();
                        };


                        WindowRect = new Rect(Left - 100, Top - 100, Width + 200, Height + 200);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowVertSizeAnimation);
                    }
                }
                Maximized = false;
            }

            ShiftShadowBehindWindow();
        }

        Rect RestoreTo = new Rect(0, 0, 0, 0);

        public void PlexWindow_AnimateRestoreUp()
        {
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = ActualHeight;
            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = AnimateMidDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowTopAnimation = new DoubleAnimation()
            {
                To = RestoreTo.Y,
                From = Top,
                Duration = AnimateMidDuration,
                EasingFunction = circleEase
            };
            windowTopAnimation.Completed += delegate
            {
                Minimized = false;
                PlexWindow_ResetTransformProperties();
            };
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            BeginAnimation(PlexWindow.TopProperty, windowTopAnimation);
        }

        public void PlexWindow_AnimateMinimize()
        {
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            RestoreTo = new Rect(Left, Top, ActualWidth, ActualHeight);
            var windowRect = WindowRect;
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = ActualHeight;
            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseIn
            };

            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = AnimateMidDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowTopAnimation = new DoubleAnimation()
            {
                To = System.Windows.Forms.Screen.FromRectangle(System.Drawing.Rectangle.FromLTRB((int)WindowRect.X, (int)WindowRect.Y, (int)WindowRect.Right, (int)WindowRect.Bottom)).WorkingArea.Bottom,
                From = Top,
                Duration = AnimateMidDuration,
                EasingFunction = circleEase
            };
            windowTopAnimation.Completed += delegate
            {
                Minimized = true;
                PlexWindow_ResetTransformProperties();
                WindowRect = windowRect;
                WindowState = WindowState.Minimized;
            };
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            BeginAnimation(PlexWindow.TopProperty, windowTopAnimation);
        }

        void PlexWindow_ResetTransformProperties()
        {
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            BeginAnimation(PlexWindow.LeftProperty, null);
            BeginAnimation(PlexWindow.TopProperty, null);
            BeginAnimation(PlexWindow.WidthProperty, null);
            BeginAnimation(PlexWindow.HeightProperty, null);
            BeginAnimation(PlexWindow.OpacityProperty, null);
            BeginAnimation(PlexWindow.WindowRectProperty, null);
            Opacity = 1;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            PlexWindow_WindowFocusChanged(this, null);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            PlexWindow_WindowFocusChanged(this, null);
        }

        public void ShiftShadowBehindWindow()
        {
            WinApi.SetWindowPos(new WindowInteropHelper(_shadowWindow).Handle, new WindowInteropHelper(this).Handle, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }


        void PlexWindow_WindowFocusChanged(object sender, EventArgs e)
        {
            ShiftShadowBehindWindow();
        }

        public void SyncShadowToWindow()
        {
            _shadowWindow.Left = Left - _shadowOffsetThickness.Left;
            _shadowWindow.Top = Top - _shadowOffsetThickness.Top;
        }

        public void SyncShadowToWindowSize()
        {
            _shadowWindow.Width = Width + _shadowOffsetThickness.Left + _shadowOffsetThickness.Right;
            _shadowWindow.Height = Height + _shadowOffsetThickness.Top + _shadowOffsetThickness.Bottom;
        }

        public void SyncShadowToWindowScale()
        {
            var scaleTransform = RenderTransform as ScaleTransform;
            _shadowWindow.RenderTransform = new ScaleTransform()
            {
                ScaleX = 1,
                ScaleY = 1,
                CenterX = scaleTransform.CenterX,
                CenterY = scaleTransform.CenterY
            };
            (_shadowWindow.RenderTransform as ScaleTransform).ScaleX = scaleTransform.ScaleX;
            (_shadowWindow.RenderTransform as ScaleTransform).ScaleY = scaleTransform.ScaleY;
        }

        void PlexWindow_StateChanged(object sender, EventArgs e)
        {
            ShiftShadowBehindWindow();
        }

        void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void ThumbBottomRightCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10)
                Width += e.HorizontalChange;
            if (Height + e.VerticalChange > 10)
                Height += e.VerticalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbTopRightCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10)
                Width += e.HorizontalChange;
            if (Top + e.VerticalChange > 10)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbTopLeftCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            if (Top + e.VerticalChange > 10)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbBottomLeftCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            if (Height + e.VerticalChange > 10)
                Height += e.VerticalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10)
                Width += e.HorizontalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbBottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange > 10)
                Height += e.VerticalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbTop_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Top + e.VerticalChange > 10)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }
    }

    public enum PlexResizeMode
    {
        CanResize,
        CanResizeWithGrip,
        CanMinimize,
        NoResize,
        Manual
    };
}