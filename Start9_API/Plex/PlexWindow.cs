using System;
using System.Collections.Generic;
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
		const string PartThumbTopRightCorner = "PART_ThumbTopRightCorner";
		const string PartThumbTopLeftCorner = "PART_ThumbTopLeftCorner";
		const string PartThumbBottomLeftCorner = "PART_ThumbBottomLeftCorner";
		const string PartThumbRight = "PART_ThumbRight";
		const string PartThumbLeft = "PART_ThumbLeft";

		public static readonly DependencyProperty MaximizedProperty =
			DependencyProperty.Register("Maximized", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

		public static readonly DependencyProperty MinimizedProperty =
			DependencyProperty.Register("Minimized", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

		public static readonly DependencyProperty WindowRectProperty = DependencyProperty.Register("WindowRect", typeof(Rect),
			typeof(PlexWindow), new PropertyMetadata(new Rect(), OnWindowRectPropertyChangedCallback));

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
			new FrameworkPropertyMetadata((double) 48, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ToolBarHeightProperty = DependencyProperty.Register("ToolBarHeight",
			typeof(double), typeof(PlexWindow),
			new FrameworkPropertyMetadata((double) 42, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty FooterHeightProperty = DependencyProperty.Register("FooterHeight",
			typeof(double), typeof(PlexWindow),
			new FrameworkPropertyMetadata((double) 36, FrameworkPropertyMetadataOptions.AffectsRender));

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
		//#FF5E98D9


		/// <summary>
		///     Interaction logic for PlexWindow.xaml
		/// </summary>
		public PlexWindow()
		{
			_shadowWindow = new ShadowWindow(this);
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
			//SyncShadowToWindow();
			//SyncShadowToWindowSize();
			_shadowTimer.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					{
						SyncShadowToWindow();
					}
				}));
			};
			Loaded += PlexWindow_Loaded;
			Activated += PlexWindow_Activated;
			Deactivated += PlexWindow_Deactivated;
			/*StoreMaxWidth = MaxWidth;
			StoreMaxHeight = MaxHeight;*/

			var restoreMinSettings = new RoutedCommand();
			restoreMinSettings.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Windows));
			CommandBindings.Add(new CommandBinding(restoreMinSettings, RestoreMinimizeWindow));
		}

		public bool Maximized
		{
			get => (bool) GetValue(MaximizedProperty);
			set => SetValue(MaximizedProperty, value);
		}

		public bool Minimized
		{
			get => (bool) GetValue(MinimizedProperty);
			set => SetValue(MinimizedProperty, value);
		}

		public Rect WindowRect
		{
			get => (Rect) GetValue(WindowRectProperty);
			set => SetValue(WindowRectProperty, value);
		}

		//private const string PART_Footer = "PART_Footer";

		/*public static void SetTitleBarContent(PlexWindow element, object value)
	    {

	        element.SetValue(TitleBarContentProperty, value);
	    }

	    public static object GetTitleBarContent(PlexWindow element)
	    {
	        return element.GetValue(TitleBarContentProperty);
	    }*/

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
			get => (double) GetValue(TitleBarHeightProperty);
			set => SetValue(TitleBarHeightProperty, value);
		}

		public double ToolBarHeight
		{
			get => (double) GetValue(ToolBarHeightProperty);
			set => SetValue(ToolBarHeightProperty, value);
		}

		public double FooterHeight
		{
			get => (double) GetValue(FooterHeightProperty);
			set => SetValue(FooterHeightProperty, value);
		}

		public bool ShowTitleBar
		{
			get => (bool) GetValue(ShowTitleBarProperty);
			set => SetValue(ShowTitleBarProperty, value);
		}

		public bool ShowToolBar
		{
			get => (bool) GetValue(ShowToolBarProperty);
			set => SetValue(ShowToolBarProperty, value);
		}

		public bool ShowFooter
		{
			get => (bool) GetValue(ShowFooterProperty);
			set => SetValue(ShowFooterProperty, value);
		}

		public Brush BodyBrush
		{
			get => (Brush) GetValue(BodyBrushProperty);
			set => SetValue(BodyBrushProperty, value);
		}
		/*<Border.Background>
	    <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
	    <LinearGradientBrush.GradientStops>
	    <GradientStop Color="White" Offset="0"/>
	    <GradientStop Color="#FFC8D4E7" Offset="1"/>
	    </LinearGradientBrush.GradientStops>
	    </LinearGradientBrush>
	    </Border.Background>*/

		public Brush FooterBrush
		{
			get => (Brush) GetValue(FooterBrushProperty);
			set => SetValue(FooterBrushProperty, value);
		}

		readonly Timer _shadowTimer = new Timer
		{
			Interval = 1
		};
		/*double StoreMaxWidth = 0;
	    double StoreMaxHeight = 0;*/

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
		double _restoreHeight;

		double _restoreLeft;
		double _restoreTop;
		double _restoreWidth;

		Thickness _shadowOffsetThickness = new Thickness(49, 14, 14, 60);
		Thumb _thumbBottom;
		Thumb _thumbBottomLeftCorner;
		Thumb _thumbBottomRightCorner;
		Thumb _thumbLeft;
		Thumb _thumbRight;
		Thumb _thumbTop;
		Thumb _thumbTopLeftCorner;
		Thumb _thumbTopRightCorner;

		Grid _titlebar;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

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
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			{
				_titlebar = GetTemplateChild(PartTitlebar) as Grid;
				_titlebar.MouseLeftButtonDown += Titlebar_MouseLeftButtonDown;
				_titlebar.MouseLeftButtonUp += Titlebar_MouseLeftButtonUp;
				_titlebar.MouseMove += Titlebar_MouseMove;

				_minButton = GetTemplateChild(PartMinimizeButton) as Button;
				_minButton.Click += delegate { ManageMaximizeRestore(2); };

				_maxButton = GetTemplateChild(PartMaximizeButton) as Button;
				_maxButton.Click += delegate { ManageMaximizeRestore(0); };

				_restButton = GetTemplateChild(PartRestoreButton) as Button;
				_restButton.Click += delegate { ManageMaximizeRestore(1); };

				_closeButton = GetTemplateChild(PartCloseButton) as Button;
				_closeButton.Click += delegate
				{
					Close();
					_shadowWindow.Close();
				};


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
		}

		public void ManageMaximizeRestore(int maxRestMin)
		{
			if (maxRestMin < 2)
			{
				var windowAnimation = new RectAnimation
				{
					EasingFunction = new ExponentialEase
					{
						EasingMode = EasingMode.EaseOut
					},
					Duration = TimeSpan.FromMilliseconds(250),
					From = new Rect(Left, Top, ActualWidth, ActualHeight)
				};

				switch (maxRestMin)
				{
					case 0:
						ManageWindowSize(true);
						var s = Screen.FromHandle(new WindowInteropHelper(this).Handle);
						windowAnimation.To = new Rect(s.WorkingArea.Left, s.WorkingArea.Top, s.WorkingArea.Width, s.WorkingArea.Height);
						Maximized = true;
						_shadowWindow.Hide();
						break;
					case 1:
						windowAnimation.To = new Rect(_restoreLeft, _restoreTop, _restoreWidth, _restoreHeight);

						Maximized = false;
						windowAnimation.Completed += delegate
						{
							_shadowWindow.Visibility = Visibility.Visible;
							Visibility = Visibility.Hidden;
							Visibility = Visibility.Visible;
						};
						break;
				}

				BeginAnimation(WindowRectProperty, windowAnimation);
			}
			else if (maxRestMin == 2)
			{
				ManageWindowSize(true);
				var windowTopAnimation = new DoubleAnimation
				{
					From = Top,
					To = SystemParameters.WorkArea.Bottom,
					Duration = TimeSpan.FromMilliseconds(250)
				};

				var windowLeftAnimation = new DoubleAnimation
				{
					To = _restoreLeft + _restoreWidth,
					Duration = TimeSpan.FromMilliseconds(500)
				};

				var windowScaleAnimation = new DoubleAnimation
				{
					To = 0,
					Duration = TimeSpan.FromMilliseconds(250)
				};


				_shadowWindow.Hide();
				windowLeftAnimation.Completed += delegate
				{
					BeginAnimation(LeftProperty, null);
					Left = _restoreLeft;
				};
				//BeginAnimation(PlexWindow.LeftProperty, WindowLeftAnimation);

				BeginAnimation(WidthProperty, windowScaleAnimation);
				BeginAnimation(HeightProperty, windowScaleAnimation);
				windowScaleAnimation.Completed += delegate
				{
					BeginAnimation(WidthProperty, null);
					BeginAnimation(HeightProperty, null);
					BeginAnimation(OpacityProperty, null);
					Opacity = 1;
					Width = _restoreWidth;
					Height = _restoreHeight;
				};
				BeginAnimation(OpacityProperty, windowScaleAnimation);

				windowTopAnimation.Completed += delegate
				{
					BeginAnimation(TopProperty, null);
					Top = _restoreTop;
					Minimized = true;
					WindowState = WindowState.Minimized;
				};
				BeginAnimation(TopProperty, windowTopAnimation);
			}
		}

		public void ManageWindowSize(bool store)
		{
			if (store) //Store real values to Restore variables
			{
				_restoreLeft = Left;
				_restoreTop = Top;
				_restoreWidth = Width;
				_restoreHeight = Height;
			}
			else //Retrieve real values from Restore variables
			{
				Left = _restoreLeft;
				Top = _restoreTop;
				Width = _restoreWidth;
				Height = _restoreHeight;
			}
		}

		protected override void OnStateChanged(EventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				Minimized = false;
				WindowState = WindowState.Normal;
				ManageMaximizeRestore(0);
			}
			else if (WindowState == WindowState.Normal)
			{
				Minimized = false;
				Maximized = false;
			}
			else if ((WindowState == WindowState.Minimized) & !Minimized)
			{
				Maximized = false;
				ManageMaximizeRestore(2);
			}
		}

		void PlexWindow_Activated(object sender, EventArgs e)
		{
			_shadowWindow.Visibility = Visibility.Visible;
			SyncShadowToWindow();
			_shadowWindow.Topmost = true;
			_shadowWindow.Topmost = false;
			_shadowWindow.Opacity = 1;
			Focus();
			Topmost = true;
			Topmost = false;
		}


		void PlexWindow_Deactivated(object sender, EventArgs e)
		{
			_shadowWindow.Opacity = 0.5;
		}

		static void OnWindowRectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = (PlexWindow) d;
			var hwnd = new WindowInteropHelper(window).Handle;
			var rect = window.WindowRect;
			MoveWindow(hwnd, (int) rect.Left, (int) rect.Top, (int) rect.Width, (int) rect.Height, true);
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

		void PlexWindow_StateChanged(object sender, EventArgs e)
		{
			if ((WindowState == WindowState.Maximized) | (WindowState == WindowState.Minimized))
				_shadowWindow.Visibility = Visibility.Hidden;
			else
				_shadowWindow.Visibility = Visibility.Visible;
		}

		void PlexWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((Visibility == Visibility.Visible) & (WindowState == WindowState.Normal))
				_shadowWindow.Visibility = Visibility.Visible;
			else
				_shadowWindow.Visibility = Visibility.Hidden;
		}

		void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
			SyncShadowToWindow();
			_shadowTimer.Start();
		}

		void Titlebar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_shadowTimer.Stop();
		}

		void Titlebar_MouseMove(object sender, MouseEventArgs e)
		{
			/*if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
			{
			    SyncShadowToWindow();
			}*/
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
}