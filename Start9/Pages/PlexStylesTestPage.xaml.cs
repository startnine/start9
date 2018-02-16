using Start9.Api.Plex;
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

namespace Start9.Pages
{
    /// <summary>
    /// Interaction logic for PlexStylesTestPage.xaml
    /// </summary>
    public partial class PlexStylesTestPage : Page
    {
        PlexWindow ParentWindow;

        public PlexStylesTestPage()
        {
            InitializeComponent();
            ParentWindow = (Window.GetWindow(this) as PlexWindow);
        }

        void TitleBarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentWindow.ShowTitleBar = true;
            }
            catch { }
        }

        void TitleBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentWindow.ShowTitleBar = false;
            }
            catch { }
        }

        void ToolBarCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentWindow.ShowToolBar = true;
            }
            catch { }
        }

        void ToolBarCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentWindow.ShowToolBar = false;
            }
            catch { }
        }

        void FooterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentWindow.ShowFooter = true;
            }
            catch { }
        }

        void FooterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentWindow.ShowFooter = false;
            }
            catch { }
        }

        private void ColouresButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary coloures = new ResourceDictionary();

            if ((sender as System.Windows.Controls.Primitives.ToggleButton).IsChecked == true)
            {
                coloures.Source = new Uri("/Start9.Api;component/Themes/Colors/PlexGreen.xaml", UriKind.Relative);
            }
            else
            {
                coloures.Source = new Uri("/Start9.Api;component/Themes/Colors/PlexBlue.xaml", UriKind.Relative);
            }

            Resources.MergedDictionaries.Add(coloures);
            //BodyBrush = (System.Windows.Media.Brush)(BodyRoot.Resources["DefaultWindowBodyBrush"]);
        }
    }
}
