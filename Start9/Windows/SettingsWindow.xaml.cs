using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Navigation;
using NetworkUI;
using Start9.Api;
using Start9.Api.Plex;
using Start9.Api.Tools;
using Start9.NodeControl;
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
			SettingsFrame.Navigate(new Home());
            MarketFrame.Navigate(new MarketplaceTestPage());
		}

        public NodeControlPageViewModel ViewModel
        {
            get
            {
                return (NodeControlPageViewModel)DataContext;
            }
        }

        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void networkControl_ConnectionDragStarted(Object sender, ConnectionDragStartedEventArgs e)
        {
            var draggedOutConnector = (EntryViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkControl);

            //
            // Delegate the real work to the view model.
            //
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void networkControl_QueryConnectionFeedback(Object sender, QueryConnectionFeedbackEventArgs e)
        {
            var draggedOutConnector = (EntryViewModel)e.ConnectorDraggedOut;
            var draggedOverConnector = (EntryViewModel)e.DraggedOverConnector;
            Object feedbackIndicator = null;
            var connectionOk = true;

            this.ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

            //
            // Return the feedback object to NetworkView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            //
            e.FeedbackIndicator = feedbackIndicator;

            //
            // Let NetworkView know if the connection is ok or not ok.
            //
            e.ConnectionOk = connectionOk;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void networkControl_ConnectionDragging(Object sender, ConnectionDraggingEventArgs e)
        {
            Point curDragPoint = Mouse.GetPosition(networkControl);
            var connection = (MessagePathViewModel)e.Connection;
            this.ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void networkControl_ConnectionDragCompleted(Object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (EntryViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (EntryViewModel)e.ConnectorDraggedOver;
            var newConnection = (MessagePathViewModel)e.Connection;
            this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised when the size of a node has changed.
        /// </summary>
        private void Node_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            var element = (FrameworkElement)sender;
            var node = (ModuleViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
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

        public void AnimateGrid(Grid grid, Boolean goLeft, Boolean hide)
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

                if (!goLeft)
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

                if (!goLeft)
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

        private void ListViewItem_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            ViewModel.CreateNode(Modules.ItemsSource.OfType<Module>().ElementAt(Modules.SelectedIndex), new Point(50, 50), true);
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