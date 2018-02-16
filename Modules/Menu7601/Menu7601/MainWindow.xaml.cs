using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Menu7601
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(250);

        double TopAnimatedOut = 0;
        double TopAnimatedIn = 0;

        CircleEase CircleEase = new CircleEase()
        {
            EasingMode = EasingMode.EaseOut
        };

        public MainWindow()
        {
            InitializeComponent();
            Left = SystemParameters.WorkArea.Left;
            ToggleButton tempStart = new ToggleButton()
            {
                Style = (Style)Resources["StartStyle"],
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch/*,
                Content = "Start"*/
            };

            new Window()
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                AllowsTransparency = true,
                Background = new SolidColorBrush(Colors.Transparent),
                Width = 50,
                Height = 50,
                Left = 0,
                Top = SystemParameters.PrimaryScreenHeight - 50,
                Topmost = true,
                Content = tempStart
            }.Show();

            TopAnimatedIn = SystemParameters.WorkArea.Bottom - Height;
            TopAnimatedOut = TopAnimatedIn + 50;

            tempStart.Click += delegate
            {
                if (Visibility == Visibility.Visible)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            };

            Binding checkedBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("IsVisible"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(tempStart, ToggleButton.IsCheckedProperty, checkedBinding);

            Deactivated += delegate { Hide(); };

            foreach (string s in Directory.EnumerateDirectories(Environment.ExpandEnvironmentVariables(@"%userprofile%")))
            {
                ListViewItem item = new ListViewItem()
                {
                    Content = System.IO.Path.GetFileName(s),
                    Tag = s
                };
                item.MouseLeftButtonUp += delegate { Process.Start(s); };
                if (!(item.Content.ToString().StartsWith(".")))
                {
                    PlacesListView.Items.Add(item);
                }
            }

            string path = @"%AppData%\ClassicShell\Pinned";

            if (Directory.Exists(Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Internet Explorer\Quick Launch\User Pinned\StartMenu")))
            {
                path = @"%appdata%\Microsoft\Internet Explorer\Quick Launch\User Pinned\StartMenu";
            }

            foreach (string f in Directory.EnumerateFiles(Environment.ExpandEnvironmentVariables(path)))
            {
                foreach (string s in Directory.EnumerateFiles(Environment.ExpandEnvironmentVariables(path)))
                {
                    ListViewItem item = new ListViewItem()
                    {
                        Content = System.IO.Path.GetFileName(s),
                        Tag = s
                    };
                    item.MouseLeftButtonUp += delegate
                    {
                        try
                        {
                            Process.Start(s);
                        }
                        catch (System.ComponentModel.Win32Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    };
                    if (!(item.Content.ToString().StartsWith(".")))
                    {
                        PinnedListView.Items.Add(item);
                    }
                }
            }
        }

        new public void Show()
        {
            base.Show();
            //Visibility = Visibility.Visible;

            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                Duration = AnimationDuration,
                EasingFunction = CircleEase,
                To = 1
            };


            DoubleAnimation topAnimation = new DoubleAnimation()
            {
                Duration = AnimationDuration,
                EasingFunction = CircleEase,
                From = TopAnimatedOut,
                To = TopAnimatedIn
            };

            BeginAnimation(MainWindow.OpacityProperty, opacityAnimation);
            BeginAnimation(MainWindow.TopProperty, topAnimation);
        }

        new public void Hide()
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                Duration = AnimationDuration,
                EasingFunction = CircleEase,
                To = 0
            };


            DoubleAnimation topAnimation = new DoubleAnimation()
            {
                Duration = AnimationDuration,
                EasingFunction = CircleEase,
                From = TopAnimatedIn,
                To = TopAnimatedOut
            };
            topAnimation.Completed += delegate { base.Hide(); };

            BeginAnimation(MainWindow.OpacityProperty, opacityAnimation);
            BeginAnimation(MainWindow.TopProperty, topAnimation);
        }

        /*private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (Visibility == Visibility.Visible)
            {
                
            }
        }*/

        private void PinnedListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AllAppsToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllAppsToggleButton.IsChecked == true)
            {
                BeginStoryboard((Storyboard)Resources["ShowAllApps"]);
            }
            else
            {
                BeginStoryboard((Storyboard)Resources["HideAllApps"]);
            }
        }
    }
}
