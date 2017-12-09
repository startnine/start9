using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Start9.Api.Objects;
using Start9.Api.Tools;
using Color = System.Windows.Media.Color;

namespace TestModule
{
	/// <summary>
	///     Interaction logic for TaskbarGroupStackPanel.xaml
	/// </summary>
	public partial class TaskbarGroupStackPanel : UserControl
	{
		public TaskbarGroupStackPanel()
		{
			InitializeComponent();
			ThumbPreviewWindow = new Window
			{
				Background = new SolidColorBrush(Color.FromArgb(0xC0, 0x0, 0x0, 0x0)),
				WindowStyle = WindowStyle.None,
				AllowsTransparency = true,
				Width = 400,
				Height = 35,
				Visibility = Visibility.Hidden,
				Content = new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Top,
					Orientation = Orientation.Vertical,
					Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF))
				},
				Topmost = true,
				ShowActivated = false,
				ShowInTaskbar = false
			};


			ThumbPreviewWindow.MouseLeave += delegate
			{
				if (!IsMouseOver)
				{
					HideThumbnailPreviewWindow();
					(ThumbPreviewWindow.Content as StackPanel).Children.Clear();
				}
			};

			MouseLeave += delegate
			{
				if (ThumbPreviewWindow.Visibility == Visibility.Visible && !ThumbPreviewWindow.IsMouseOver)
					ThumbPreviewWindow.Hide();
			};


			ThumbPreviewWindow.IsVisibleChanged += delegate
			{
				if (ThumbPreviewWindow.Visibility == Visibility.Visible)
					foreach (var b in ProgramWindowsList)
					{
						var windowEntry = new Button
						{
							Style = (Style) Resources["ThumbPreviewButtonStyle"],
							Content = new Grid
							{
								HorizontalAlignment = HorizontalAlignment.Stretch,
								VerticalAlignment = VerticalAlignment.Stretch,
								Margin = new Thickness(6, 0, 0, 0)
							},
							Tag = b
						};

						var entryContent = windowEntry.Content as Grid;

						try
						{
							entryContent.Children.Add(new Canvas
							{
								HorizontalAlignment = HorizontalAlignment.Left,
								Width = 16,
								Height = 16,
								Background = new ImageBrush(b.Icon.ToBitmap().ToBitmapSource())
							});
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
						}
						;

						entryContent.Children.Add(new Label
						{
							HorizontalAlignment = HorizontalAlignment.Left,
							Content = b.Name,
							Margin = new Thickness(16, 0, 0, 0),
							Foreground = new SolidColorBrush(Colors.White)
						});

						var closeButton = new Button
						{
							Style = (Style) Resources["ThumbPreviewCloseButtonStyle"]
						};
						closeButton.Click += delegate
						{
							(windowEntry.Tag as ProgramWindow).Close();
							(ThumbPreviewWindow.Content as StackPanel).Children.Remove(windowEntry);
							foreach (Button g in Buttons.Children)
								if ((g.Tag as ProgramWindow).Hwnd == (windowEntry.Tag as ProgramWindow).Hwnd)
								{
									Buttons.Children.Remove(g);
									return;
								}
							foreach (var p in ProgramWindowsList)
								if (p.Hwnd == (windowEntry.Tag as ProgramWindow).Hwnd)
								{
									ProgramWindowsList.Remove(p);
									return;
								}
						};


						entryContent.Children.Add(closeButton);

						windowEntry.Click += delegate
						{
							(windowEntry.Tag as ProgramWindow).Show();
							/*MiscTools.ShowWindow((WindowEntry.Tag as ProgramWindow).Hwnd, 10);
				            MiscTools.SetForegroundWindow((WindowEntry.Tag as ProgramWindow).Hwnd);*/
							ThumbPreviewWindow.Hide();
						};

						(ThumbPreviewWindow.Content as StackPanel).Children.Add(windowEntry);
					}
			};
			//RunningBackgroundButton.Click += delegate { ShowThumbnailPreviewWindow(); };

			var showThumbnailPreviewCounter = 0;

			var mouseGroupContactTimer = new Timer
			{
				Interval = 1
			};
			mouseGroupContactTimer.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					{
						if (
							(MainTools.GetDpiScaledCursorPosition().X > MainTools.GetDpiScaledGlobalControlPosition(this).X) &
							(MainTools.GetDpiScaledCursorPosition().X < MainTools.GetDpiScaledGlobalControlPosition(this).X + ActualWidth) &
							(MainTools.GetDpiScaledCursorPosition().Y > MainTools.GetDpiScaledGlobalControlPosition(this).Y) &
							(MainTools.GetDpiScaledCursorPosition().Y < MainTools.GetDpiScaledGlobalControlPosition(this).Y + ActualHeight)
						)
							if (showThumbnailPreviewCounter < 50)
							{
								showThumbnailPreviewCounter = showThumbnailPreviewCounter + 1;
							}
							else
							{
								showThumbnailPreviewCounter = 0;
								if ((ThumbPreviewWindow.Visibility == Visibility.Visible) & !ThumbPreviewWindow.IsMouseOver & !IsMouseOver)
									HideThumbnailPreviewWindow();
								else
									ShowThumbnailPreviewWindow();
							}
					}
				}));
			};

			Loaded += delegate { mouseGroupContactTimer.Start(); };
		}

		public TaskbarGroupStatus GroupStatus
		{
			get => GroupStatusStorage;
			set => GroupStatusStorage = value;
		}

		public TaskbarGroupSideTabMode GroupSideTabMode
		{
			get => GroupSideTabModeStorage;
			set
			{
				GroupSideTabModeStorage = value;
				if ((GroupSideTabModeStorage == TaskbarGroupSideTabMode.GroupTabSingle) |
				    (GroupSideTabModeStorage == TaskbarGroupSideTabMode.GroupTabDouble))
				{
					JumpGroupGrid.Visibility = Visibility.Visible;
					(JumpGroupGrid.Children[0] as Grid).Visibility = Visibility.Visible;
					if (GroupSideTabModeStorage == TaskbarGroupSideTabMode.GroupTabSingle)
					{
						((JumpGroupGrid.Children[0] as Grid).Children[0] as Border).Visibility = Visibility.Visible;
						((JumpGroupGrid.Children[0] as Grid).Children[1] as Border).Visibility = Visibility.Hidden;
					}
					else if (GroupSideTabModeStorage == TaskbarGroupSideTabMode.GroupTabDouble)
					{
						((JumpGroupGrid.Children[0] as Grid).Children[0] as Border).Visibility = Visibility.Hidden;
						((JumpGroupGrid.Children[0] as Grid).Children[1] as Border).Visibility = Visibility.Visible;
					}
					(JumpGroupGrid.Children[1] as Button).Visibility = Visibility.Hidden;
					JumpGroupGrid.Width = 8;
					Debug.WriteLine("GroupTab shown!");
				}
				else if (GroupSideTabModeStorage == TaskbarGroupSideTabMode.JumpListButton)
				{
					JumpGroupGrid.Visibility = Visibility.Visible;
					(JumpGroupGrid.Children[0] as Grid).Visibility = Visibility.Hidden;
					(JumpGroupGrid.Children[1] as Button).Visibility = Visibility.Visible;
					JumpGroupGrid.Width = 8;
					Debug.WriteLine("Jump List Button shown!");
				}
				else
				{
					JumpGroupGrid.Visibility = Visibility.Hidden;
					JumpGroupGrid.Width = 0;
					Debug.WriteLine("Jump Group Grid hidden!");
				}
			}
		}

		readonly SuperbarModule _thisModule = (SuperbarModule) MainTools.GetFixedModule("Test Module");

		public bool ForceCombine = false;

		public TaskbarGroupSideTabMode GroupSideTabModeStorage = TaskbarGroupSideTabMode.None;

		public TaskbarGroupStatus GroupStatusStorage = TaskbarGroupStatus.Idle;

		public List<ProgramWindow> ProgramWindowsList = new List<ProgramWindow>();

		public Window ThumbPreviewWindow;

		public void AddButton(TaskItemButton itemButton)
		{
			Buttons.Children.Add(itemButton);
			ManageButtons();
		}

		public void RemoveButtonByHwnd(IntPtr hWnd)
		{
			for (var i = 0; i < ProgramWindowsList.Count; i++)
			{
				var p = ProgramWindowsList[i];
				if (p.Hwnd == hWnd)
					ProgramWindowsList.Remove(p);
			}

			if ((Config.GroupingMode != TaskbarGroupingMode.Combine) | ForceCombine)
				for (var i = 0; i < Buttons.Children.Count; i++)
				{
					var b = (Button) Buttons.Children[i];
					if ((b.Tag as ProgramWindow).Hwnd == hWnd)
						Buttons.Children.Remove(b);
				}
		}

		void ManageButtons()
		{
			if (Buttons.Children.Count == 0)
			{
				GroupStatus = TaskbarGroupStatus.Idle;
				GroupSideTabMode = TaskbarGroupSideTabMode.None;
			}
			else if ((Config.GroupingMode == TaskbarGroupingMode.Combine) | ForceCombine
			) //This condition will ultimately be true if Taskbar Grouping is set to Always Combine
			{
				//Set the only Button's style to a single Taskbar Button
				GroupStatus = TaskbarGroupStatus.Running;
				if (Buttons.Children.Count >= 3)
					GroupSideTabMode = TaskbarGroupSideTabMode.GroupTabDouble;
				else if (Buttons.Children.Count == 2)
					GroupSideTabMode = TaskbarGroupSideTabMode.GroupTabSingle;
				else
					GroupSideTabMode = TaskbarGroupSideTabMode.None;
			}
			else if (Config.GroupingMode == TaskbarGroupingMode.NeverCombine
			) //This condition will ultimately be true if Taskbar Grouping is set to Never Combine, I guess
			{
				GroupStatus = TaskbarGroupStatus.Running;
				GroupSideTabMode = TaskbarGroupSideTabMode.None;
				if (Buttons.Children.Count == 1)
				{
					//Set the only Button's style to a single Taskbar Button
				}
				else if (Buttons.Children.Count >= 2)
				{
					//Set the first Button's style to a left-end Taskbar Button
					//Set the last Button's style to a right-end Taskbar Button
					//Set the other Buttons' styles to middle Taskbar Buttons if there are any
				}
			}
		}

		public void CreateButtons(WrapPanel taskband)
		{
			Buttons.Children.Clear();

			if ((Config.GroupingMode == TaskbarGroupingMode.Combine) | ForceCombine)
			{
				Width = 60;
				if (ProgramWindowsList.Count > 0)
				{
					RunningBackgroundButton.Visibility = Visibility.Visible;
					if (ProgramWindowsList.Count > 2)
						GroupSideTabMode = TaskbarGroupSideTabMode.GroupTabDouble;
					else if (ProgramWindowsList.Count > 1)
						GroupSideTabMode = TaskbarGroupSideTabMode.GroupTabSingle;
					else
						GroupSideTabMode = TaskbarGroupSideTabMode.None;
				}
				else
				{
					RunningBackgroundButton.Visibility = Visibility.Hidden;
				}
				PinnedIcon.Visibility = Visibility.Visible;
				var pinnedIconIcon = Icon.ExtractAssociatedIcon(Convert.ToString(Tag.ToString()));
				PinnedIcon.Background = new ImageBrush(pinnedIconIcon.ToBitmap().ToBitmapSource());

				//RunningBackgroundButton.RootButton.MouseEnter += TaskItemButton_MouseEnter;
				//RunningBackgroundButton.RootButton.Click += RunningBackgroundButton_Click;
				RunningBackgroundButton.RootButton.PreviewMouseLeftButtonDown += TaskItemButton_MouseLeftButtonDown;
			}
			else
			{
				GroupSideTabMode = TaskbarGroupSideTabMode.None;
				RunningBackgroundButton.Visibility = Visibility.Hidden;
				PinnedIcon.Visibility = Visibility.Visible;
				foreach (var b in ProgramWindowsList)
				{
					var taskItemButton = new TaskItemButton(b);

					taskItemButton.RootButton.PreviewMouseLeftButtonDown += TaskItemButton_MouseLeftButtonDown;


					taskItemButton.RootButton.Click += delegate
					{
						if (MiscTools.GetForegroundWindow() == (taskItemButton.Tag as ProgramWindow).Hwnd)
							(taskItemButton.Tag as ProgramWindow).Minimize();
						else
							(taskItemButton.Tag as ProgramWindow).Show();
					};

					try
					{
						var exStyle = MiscTools.GetWindowLong(b.Hwnd, MiscTools.GwlExstyle);
						if (MiscTools.Taskstyle == (MiscTools.Taskstyle & MiscTools.GetWindowLong(b.Hwnd, MiscTools.GwlExstyle)) &&
						    (exStyle & MiscTools.WsExToolwindow) != MiscTools.WsExToolwindow)
							AddButton(taskItemButton);
					}
					catch
					{
						Debug.WriteLine("TaskItemButton not added");
					}
				}
			}
		}

		void TaskItemButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var dragTimer = new Timer
			{
				Interval = 1
			};

			dragTimer.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					{
						if (Mouse.LeftButton == MouseButtonState.Released)
						{
							dragTimer.Stop();
							var g = _thisModule.Taskbars[0].Taskband.Children;
							for (var i = 0; i < g.Count; i++)
							{
								var taskGroup = (TaskbarGroupStackPanel) g[i];

								if ((taskGroup != this) &
								    (MainTools.GetDpiScaledCursorPosition().X > MainTools.GetDpiScaledGlobalControlPosition(taskGroup).X) &
								    (MainTools.GetDpiScaledCursorPosition().X <
								     MainTools.GetDpiScaledGlobalControlPosition(taskGroup).X + taskGroup.ActualWidth)
								)
								{
									var thisIsGreater = false;
									if (g.IndexOf(taskGroup) < g.IndexOf(this))
										thisIsGreater = true;
									g.Remove(this);
									var h = _thisModule.Taskbars[0].Taskband.Children;
									if (thisIsGreater)
									{
										if (g.IndexOf(taskGroup) - 1 >= 0)
											g.Insert(g.IndexOf(taskGroup) - 1, this);
										else
											g.Insert(0, this);
										Console.WriteLine("Left");
										//True regardless of direction moved (!?)
									}
									else
									{
										g.Insert(g.IndexOf(taskGroup), this);
										Console.WriteLine("Right");
										//True only when moving right, as intended
									}
								}
							}
						}
					}
				}));
			};

			dragTimer.Start();
		}

		void TaskItemButton_MouseEnter(object sender, MouseEventArgs e)
		{
			foreach (TaskbarGroupStackPanel t in _thisModule.Taskbars[0].Taskband.Children)
				try
				{
					if (sender is TaskItemButton)
					{
						if (t.Tag.ToString() == ((sender as TaskItemButton).Tag as ProgramWindow).Process.MainModule.FileName)
							t.ShowThumbnailPreviewWindow();
					}
					else if (sender is Button)
					{
						if (t.Tag.ToString() == ((sender as Button).Tag as ProgramWindow).Process.MainModule.FileName)
							t.ShowThumbnailPreviewWindow();
					}
				}
				catch
				{
				}
		}

		public void ShowThumbnailPreviewWindow()
		{
			var stackPoint = MainTools.GetDpiScaledGlobalControlPosition(this);
			ThumbPreviewWindow.Height = 35 * ProgramWindowsList.Count;
			ThumbPreviewWindow.Left = (stackPoint.X + (ActualWidth / 2)) - (ThumbPreviewWindow.ActualWidth / 2);
			ThumbPreviewWindow.Top = stackPoint.Y - ThumbPreviewWindow.Height;
			ThumbPreviewWindow.Visibility = Visibility.Visible;
		}

		public void HideThumbnailPreviewWindow()
		{
			ThumbPreviewWindow.Visibility = Visibility.Hidden;
		}

		public void RunningBackgroundButton_Click(object sender, RoutedEventArgs e)
		{
			ProgramWindowsList[0].Show();
		}

		public void RunningBackgroundButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ProgramWindowsList[0].Show();
		}
	}
}