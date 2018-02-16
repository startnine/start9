using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
//using System.Windows.Shapes;
using System.Diagnostics;
using Start9.Api.Tools;
using System.ComponentModel;

namespace MenuJumpstart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Show();
            /*Left = SystemParameters.WorkArea.Left;
            Top = SystemParameters.WorkArea.Top;
            Height = SystemParameters.WorkArea.Height;*/



            ToggleButton tempStart = new ToggleButton()
            {
                Style = (Style)Resources["StartStyle"]/*,
                Content = "Start"*/
            };

            new Window()
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                AllowsTransparency = true,
                Background = new SolidColorBrush(Colors.Transparent),
                Width = 70,
                Height = 50,
                Left = 0,
                Top = SystemParameters.PrimaryScreenHeight - 50,
                Topmost = true,
                Content = tempStart
            }.Show();

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
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*var showMenuThickness = (((Storyboard)Resources["ShowMenu"]).Children[0] as ThicknessAnimation);
            var hideMenuThickness = (((Storyboard)Resources["HideMenu"]).Children[0] as ThicknessAnimation);
            //Storyboard.SetTargetName(showMenuThickness, RootGrid.Name);
            //Storyboard.SetTargetName(hideMenuThickness, RootGrid.Name);
            try
            {
                showMenuThickness.Completed += delegate
                {
                    RootGrid.BeginAnimation(Grid.MarginProperty, null);
                    RootGrid.Margin = new Thickness(0, 0, 0, 0);
                };

                hideMenuThickness.Completed += delegate
                {
                    RootGrid.BeginAnimation(Grid.MarginProperty, null);
                    RootGrid.Margin = new Thickness(-256, 0, 256, 0);
                };
            }
            catch (NullReferenceException ex) { }*/

            AllAppsTree.Items.Clear();
            AllAppsTree.ItemsSource = PopulateAllAppsList();
        }

        private List<TreeViewItem> PopulateAllAppsList()
        {
            List<TreeViewItem> AllAppsAppDataItems = GetAllAppsFoldersAsTree(Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Windows\Start Menu\Programs"));
            List<TreeViewItem> AllAppsProgramDataItems = GetAllAppsFoldersAsTree(Environment.ExpandEnvironmentVariables(@"%programdata%\Microsoft\Windows\Start Menu\Programs"));
            List<TreeViewItem> AllAppsItems = new List<TreeViewItem>();
            List<TreeViewItem> AllAppsReorgItems = new List<TreeViewItem>();

            Dispatcher.Invoke(new Action(() =>
            {
                foreach (TreeViewItem t in AllAppsAppDataItems)
                {
                    bool FolderIsDuplicate = false;

                    foreach (TreeViewItem v in AllAppsProgramDataItems)
                    {
                        List<TreeViewItem> SubItemsList = new List<TreeViewItem>();

                        if (Directory.Exists(t.Tag.ToString()))
                        {
                            if ((t.Tag.ToString().Substring(t.Tag.ToString().LastIndexOf(@"\"))) == (v.Tag.ToString().Substring(v.Tag.ToString().LastIndexOf(@"\"))))
                            {
                                FolderIsDuplicate = true;
                                foreach (TreeViewItem i in t.Items)
                                {
                                    SubItemsList.Add(i);
                                }

                                foreach (TreeViewItem j in v.Items)
                                {
                                    SubItemsList.Add(j);
                                }
                            }

                            /*if (SubItemsList.Count != 0)
                            {
                                v.ItemsSource = SubItemsList;
                            }*/
                        }

                        if (!AllAppsItems.Contains(v))
                        {
                            AllAppsItems.Add(v);
                        }
                    }

                    if ((!AllAppsItems.Contains(t)) && (!FolderIsDuplicate))
                    {
                        AllAppsItems.Add(t);
                    }
                }

                foreach (TreeViewItem x in AllAppsItems)
                {
                    if (File.Exists(x.Tag.ToString()))
                    {
                        AllAppsReorgItems.Add(x);
                    }
                }

                foreach (TreeViewItem x in AllAppsItems)
                {
                    if (Directory.Exists(x.Tag.ToString()))
                    {
                        AllAppsReorgItems.Add(x);
                    }
                }
            }));

            return AllAppsReorgItems;
        }

        public TreeViewItem AllAppsListGetItem(string path)
        {
            string target = path;

            if (System.IO.Path.GetExtension(path).Contains("lnk"))
            {
                target = GetTargetPath(path);
            }

            TreeViewItem item = new TreeViewItem();

            item.Tag = target;

            if (Directory.Exists(target))
            {
                foreach (string s in Directory.EnumerateFiles(target))
                {
                    var subItem = AllAppsListGetItem(s);
                    subItem.MinWidth = item.MinWidth + 16;
                    item.Items.Add(subItem);
                }
            }

            item.Header = System.IO.Path.GetFileNameWithoutExtension(path);

            if (Directory.Exists(item.Tag.ToString()))
            {
                item.MouseDoubleClick += Item_Opened;
                item.Header = "(FLDR) " + item.Header.ToString();
            }
            else
            {
                item.Expanded += Item_Opened;
                item.Header = "(FILE) " + item.Header.ToString();
            }

            return item;
        }

        private void Item_Opened(object sender, RoutedEventArgs e)
        {
            var item = (sender as TreeViewItem);
            try
            {
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    FileName = item.Tag.ToString(),
                    WorkingDirectory = Path.GetDirectoryName(item.Tag.ToString()),
                    UseShellExecute = true
                };
                Process.Start(info);
            }
            catch (Exception ex)
            {
                if (ex is Win32Exception)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("It's a Win32Exception, so probably just the user selecting a program that requires UAC privileges, and then cancelling out.");
                }
                else
                {
                    try
                    {
                        ProcessStartInfo info = new ProcessStartInfo()
                        {
                            FileName = item.Tag.ToString(),
                            WorkingDirectory = Path.GetDirectoryName(item.Tag.ToString())
                        };
                        Process.Start(info);
                    }
                    catch { }
                }
            }
            Hide();
        }

        private List<TreeViewItem> GetAllAppsFoldersAsTree(string Path)
        {
            List<TreeViewItem> AllAppsItems = new List<TreeViewItem>();

            foreach (string s in Directory.EnumerateFiles(Path))
            {
                TreeViewItem t = AllAppsListGetItem(s);
                /*((string)(((t.Header as DockPanel).Children[1] as System.Windows.Controls.Label).Content as string)*/
                /*(string)(((t.Header as DockPanel).Children[1] as System.Windows.Controls.Label).Content as string)*/
                if (!(t.Header.ToString().ToLower().Contains("desktop")))
                {
                    /*if (System.IO.Path.GetExtension(t.Tag.ToString()).Contains("lnk"))
                    {
                        t.Tag = GetTargetPath(t.Tag.ToString());
                    }*/
                    AllAppsItems.Add(t);
                }
            }

            foreach (string s in Directory.EnumerateDirectories(Path))
            {
                TreeViewItem t = AllAppsListGetItem(s);
                /*if (((string)(((t.Header as DockPanel).Children[1] as System.Windows.Controls.Label).Content as string) != "desktop") & ((string)(((t.Header as DockPanel).Children[1] as System.Windows.Controls.Label).Content as string) != "desktop.ini"))*/
                /*if ((t.Header.ToString() != "desktop") & (t.Header.ToString() != "desktop.ini"))*/
                if (!(t.Header.ToString().ToLower().Contains("desktop")))
                {
                    if (t.Items.Count != 0)
                    {
                        AllAppsItems.Add(t);
                    }
                }
            }
            return AllAppsItems;
        }

        public string GetTargetPath(string filePath)
        {
            string targetPath = ResolveMsiShortcut(filePath);

            if (targetPath == null)
            {
                targetPath = ResolveShortcut(filePath);
            }

            if (targetPath == null)
            {
                targetPath = GetInternetShortcut(filePath);
            }

            if (targetPath == null | targetPath == "" | targetPath.Replace(" ", "") == "")
            {
                return filePath;
            }
            else
            {
                return targetPath;
            }
        }

        public string GetInternetShortcut(string filePath)
        {
            try
            {
                string url = "";

                using (TextReader reader = new StreamReader(filePath))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("URL="))
                        {
                            string[] splitLine = line.Split('=');
                            if (splitLine.Length > 0)
                            {
                                url = splitLine[1];
                                break;
                            }
                        }
                    }
                }
                return url;
            }
            catch
            {
                return null;
            }
        }

        string ResolveShortcut(string filePath)
        {
            // IWshRuntimeLibrary is in the COM library "Windows Script Host Object Model"
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

            try
            {
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                return shortcut.TargetPath;
            }
            catch
            {
                // A COMException is thrown if the file is not a valid shortcut (.lnk) file 
                return null;
            }
        }

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        public static extern uint MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        public static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

        public const int MaxFeatureLength = 38;
        public const int MaxGuidLength = 38;
        public const int MaxPathLength = 1024;

        public enum InstallState
        {
            NotUsed = -7,
            BadConfig = -6,
            Incomplete = -5,
            SourceAbsent = -4,
            MoreData = -3,
            InvalidArg = -2,
            Unknown = -1,
            Broken = 0,
            Advertised = 1,
            Removed = 1,
            Absent = 2,
            Local = 3,
            Source = 4,
            Default = 5
        }

        string ResolveMsiShortcut(string file)
        {
            StringBuilder product = new StringBuilder(MaxGuidLength + 1);
            StringBuilder feature = new StringBuilder(MaxFeatureLength + 1);
            StringBuilder component = new StringBuilder(MaxGuidLength + 1);

            MsiGetShortcutTarget(file, product, feature, component);

            int pathLength = MaxPathLength;
            StringBuilder path = new StringBuilder(pathLength);

            InstallState installState = MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            if (installState == InstallState.Local)
            {
                return path.ToString();
            }
            else
            {
                return null;
            }
        }

        new public void Show()
        {
            //Ststem.Drawing.Point
            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Left = DpiManager.ConvertPixelsToWpfUnits(screen.WorkingArea.Left);
            Top = DpiManager.ConvertPixelsToWpfUnits(screen.WorkingArea.Top);
            Height = DpiManager.ConvertPixelsToWpfUnits(screen.WorkingArea.Height);
            base.Show();
            BeginStoryboard((Storyboard)Resources["ShowMenu"]);
        }

        new public void Hide()
        {
            ((Storyboard)Resources["HideMenu"]).Completed += delegate { base.Hide(); };
            BeginStoryboard((Storyboard)Resources["HideMenu"]);
        }

        private void ShutDownButton_Click(object sender, RoutedEventArgs e)
        {
            ShutDownSystem();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ShutDownSystem();
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            SignOut();
        }

        private void SwitchUserButton_Click(object sender, RoutedEventArgs e)
        {
            LockUserAccount();
        }


        /// <Oops>
        /// These are from Start9.Api.Tools.SystemPowerTools class, and should be used directly from there, but I accidentally made the class not public, so for now they're here.
        /// </Oops>

        public void LockUserAccount()
        {
            WinApi.LockWorkStation();
        }

        public void SignOut()
        {
            WinApi.ExitWindowsEx(WinApi.ExitWindowsAction.Force, 0);
        }

        public void SleepSystem()
        {
            WinApi.SetSuspendState(false, true, true);
        }

        public void ShutDownSystem()
        {
            WinApi.ExitWindowsEx(WinApi.ExitWindowsAction.Shutdown, 0);
        }

        public void RestartSystem()
        {
            WinApi.ExitWindowsEx(WinApi.ExitWindowsAction.Reboot, 0);
        }
    }
}