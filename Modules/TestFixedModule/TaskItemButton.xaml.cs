using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Start9.Api.Objects;
using Start9.Api.Tools;
using System.Windows.Media.Imaging;
using System.IO;

namespace TestModule
{
	/// <summary>
	///     Interaction logic for TaskItemButton.xaml
	/// </summary>
	public partial class TaskItemButton : UserControl
	{
		public static readonly DependencyProperty WindowTitleProperty =
			DependencyProperty.Register("WindowTitle", typeof(string), typeof(TaskItemButton), new PropertyMetadata(""));

		public static readonly DependencyProperty IsActiveWindowProperty =
			DependencyProperty.Register("IsActiveWindow", typeof(bool), typeof(TaskItemButton), new PropertyMetadata(false));

		public static readonly DependencyProperty WindowIconProperty = DependencyProperty.Register("WindowIcon",
			typeof(ImageBrush), typeof(TaskItemButton), new PropertyMetadata(new ImageBrush()));

        public static readonly DependencyProperty WindowIconColorProperty = DependencyProperty.Register("WindowIconColor",
            typeof(Brush), typeof(TaskItemButton), new PropertyMetadata((new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00))) as Brush));


        public TaskItemButton()
        {
            InitializeComponent();
            if (File.Exists(Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\TestImage.png")))
            {
                Resources["TestBitmapImage"] = new BitmapImage(new Uri(Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\TestImage.png"), UriKind.Relative));
            }
            else
            {
                //Resources["TestBitmapImage"] = new BitmapImage(new Uri(Environment.ExpandEnvironmentVariables(@"%windir%\web\Wallpaper\Windows\img0.jpg"), UriKind.Relative));
                Resources["TestBitmapImage"] = null;
            }
		}

		public TaskItemButton(ProgramWindow programWindow)
		{
			InitializeComponent();
			try
			{
                Console.WriteLine("TEST 0");
                WindowIcon = new ImageBrush(programWindow.Icon.ToBitmap().ToBitmapSource());
                Console.WriteLine("TEST 1 " + Environment.ExpandEnvironmentVariables(programWindow.Process.MainModule.FileName));
            }
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

            var drawingColor = MiscTools.GetColorFromImage(Environment.ExpandEnvironmentVariables(programWindow.Process.MainModule.FileName));
            Console.WriteLine("TEST 2 " + drawingColor.ToString());
            WindowIconColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B));
            Console.WriteLine("TEST 3 " + WindowIconColor.ToString());

            WindowTitle = programWindow.Name;
			Tag = programWindow;
			var doesThisWindowExist = new Timer
			{
				Interval = 100
			};
			var thisModule = (SuperbarModule) MainTools.GetFixedModule("Test Module");
			doesThisWindowExist.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					{
						if (!IsWindow(programWindow.Hwnd))
							foreach (TaskbarGroupStackPanel t in thisModule.Taskbars[0].Taskband.Children)
							foreach (var p in t.ProgramWindowsList)
								if (p.Hwnd == programWindow.Hwnd)
									t.RemoveButtonByHwnd(programWindow.Hwnd);
					}
				}));
			};
		}

		public string WindowTitle
		{
			get => (string) GetValue(WindowTitleProperty);
			set => SetValue(WindowTitleProperty, value);
		}

		public bool IsActiveWindow
		{
			get => (bool) GetValue(IsActiveWindowProperty);
			set => SetValue(IsActiveWindowProperty, value);
		}

		public ImageBrush WindowIcon
		{
			get => (ImageBrush) GetValue(WindowIconProperty);
			set => SetValue(WindowIconProperty, value);
		}

        public Brush WindowIconColor
        {
            get => (Brush)GetValue(WindowIconColorProperty);
            set => SetValue(WindowIconColorProperty, value);
        }

        /*private static void OnWindowIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as TaskItemButton).Tag is ProgramWindow)
            {
                //e.NewValue
            }
        }*/


        [DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindow(IntPtr hWnd);

		public void IsActiveWindowChangedCallBack(object defaultValue, PropertyChangedCallback propertyChangedCallback)
		{
		}
	}
}