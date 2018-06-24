using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Start9.Api;
using Start9.Api.Plex;
using Start9.Api.Tools;
using Start9.Pages;

namespace Start9.Windows
{
	/// <summary>
	///     Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : PlexWindow
	{
		public SettingsWindow()
		{
			InitializeComponent();
			MainTools.SettingsWindow = this;
			SettingsFrame.Navigate(new Home());
            MarketFrame.Navigate(new MarketplaceTestPage());
		}

		private void ShowThumbnailButton_Click(Object sender, RoutedEventArgs e)
		{
			//DwmTools.GetThumbnail(WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Progman"), ShowThumbnailButton);
		}

		private void BackButton_Click(Object sender, RoutedEventArgs e)
		{
            if (SettingsGrid.IsVisible)
            {
                SettingsFrame.NavigationService.GoBack();
            }
            else if (MarketGrid.IsVisible)
            {
                MarketFrame.NavigationService.GoBack();
            }
		}

		private void SettingsFrame_Navigated(Object sender, NavigationEventArgs e)
		{
			/*if (SettingsFrame.CanGoBack)
			{
			    BackButton.IsEnabled = true;
			}
			else
			{
			    BackButton.IsEnabled = false;
			}*/
		}

        private void MarketFrame_Navigated(Object sender, NavigationEventArgs e)
        {

        }

        QuinticEase gridEase = new QuinticEase()
        {
            EasingMode = EasingMode.EaseOut
        };

        Duration gridAnimTime = TimeSpan.FromMilliseconds(400);

        public void AnimateGrid(Grid grid, Boolean goRight, Boolean hide)
        {
            DoubleAnimation opacityAnim = new DoubleAnimation()
            {
                Duration = gridAnimTime
            };

            DoubleAnimation animation = new DoubleAnimation()
            {
                EasingFunction = gridEase,
                Duration = gridAnimTime
            };

            if (hide)
            {
                animation.Completed += delegate
                {
                    grid.Visibility = Visibility.Hidden;
                };
                opacityAnim.To = 0;

                if (goRight)
                {
                    animation.To = Width;
                }
                else
                {
                    animation.To = Width * -1;
                }
            }
            else
            {
                grid.Visibility = Visibility.Visible;
                opacityAnim.To = 1;

                if (goRight)
                {
                    animation.From = Width * -1;
                }
                else
                {
                    animation.From = Width;
                }
                animation.To = 0;
            }
            grid.BeginAnimation(Grid.OpacityProperty, opacityAnim);
            (grid.RenderTransform as TranslateTransform).BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void ToolbarHeader_Click(Object sender, RoutedEventArgs e)
        {
            Boolean goingRight = false;

            Int32 prevIndex = 0;

            foreach (Grid g in RootGrid.Children)
            {
                if (g.IsVisible)
                {
                    prevIndex = RootGrid.Children.IndexOf(g);
                }
            }

            var source = (sender as ToggleButton);

            Int32 index = ToolbarGrid.Children.IndexOf(source);

            if (index > prevIndex)
            {
                goingRight = true;
            }

            if (index != prevIndex)
            {

                var targetGrid = RootGrid.Children[index];

                foreach (Grid g in RootGrid.Children)
                {
                    if (g == targetGrid)
                    {
                        AnimateGrid(g, goingRight, false);
                    }
                    else if (g.IsVisible)
                    {
                        AnimateGrid(g, goingRight, true);
                    }
                }

                foreach (ToggleButton b in ToolbarGrid.Children)
                {
                    if (b == source)
                    {
                        b.IsChecked = true;
                    }
                    else
                    {
                        b.IsChecked = false;
                    }
                }
            }
            else
            {
                source.IsChecked = true;
            }
        }

        /*private void PlexTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
	    {
	        GoToPage(0, 0);
	    }

	    private void ModuleFrontEndTestCommandLinkButton_Click(object sender, RoutedEventArgs e)
	    {
	        GoToPage(0, 1);
	    }

	    public void GoToPage(int layerIndex, int pageIndex)
	    {
	        PageLayers.Visibility = Visibility.Visible;
	        HomePage.Visibility = Visibility.Hidden;
	        foreach (Grid g in PageLayers.Children)
	        {
	            if (PageLayers.Children.IndexOf(g) == layerIndex)
	            {
	                g.Visibility = Visibility.Visible;
	                foreach (Grid h in g.Children)
	                {
	                    if (g.Children.IndexOf(h) == pageIndex)
	                    {
	                        h.Visibility = Visibility.Visible;
	                    }
	                    else
	                    {
	                        h.Visibility = Visibility.Hidden;
	                    }
	                }
	            }
	            else
	            {
	                g.Visibility = Visibility.Hidden;
	            }
	        }
	    }

	    public void GoHome()
	    {
	        PageLayers.Visibility = Visibility.Hidden;
	        HomePage.Visibility = Visibility.Visible;
	    }

	    private void TaskbarModule1Button_Click(object sender, RoutedEventArgs e)
	    {
	        new TaskbarStylesTest().Show();
	    }

	    private void BackButton_Click(object sender, RoutedEventArgs e)
	    {
	        GoHome();
	    }*/
    }
}