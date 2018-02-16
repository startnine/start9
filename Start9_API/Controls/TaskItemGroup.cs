using Start9.Api.Programs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Start9.Api.Controls
{
    [TemplatePart(Name = PartButtonsStack, Type = typeof(StackPanel))]
    [TemplatePart(Name = PartGroupToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PartSingleGroupTab, Type = typeof(Border))]
    [TemplatePart(Name = PartDoubleGroupTab, Type = typeof(Border))]

    public partial class TaskItemGroup : Control
    {
        const string PartButtonsStack = "PART_ButtonsStack";
        const string PartGroupToggleButton = "PART_GroupToggleButton";
        const string PartSingleGroupTab = "PART_SingleGroupTab";
        const string PartDoubleGroupTab = "PART_DoubleGroupTab";

        public string ProcessName
        {
            get => (string)GetValue(ProcessNameProperty);
            set => SetValue(ProcessNameProperty, (value));
        }

        public static readonly DependencyProperty ProcessNameProperty = DependencyProperty.Register("ProcessName", typeof(string), typeof(TaskItemGroup), new PropertyMetadata("", OnProcessNamePropertyChangedCallback));

        static void OnProcessNamePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public List<ProgramWindow> ProcessWindows
        {
            get => (List<ProgramWindow>)GetValue(ProcessWindowsProperty);
            set => SetValue(ProcessWindowsProperty, (value));
        }

        public static readonly DependencyProperty ProcessWindowsProperty = DependencyProperty.Register("ProcessWindows", typeof(List<ProgramWindow>), typeof(TaskItemGroup), new PropertyMetadata(new List<ProgramWindow>()));

        public bool CombineButtons
        {
            get => (bool)GetValue(CombineButtonsProperty);
            set => SetValue(CombineButtonsProperty, (value));
        }

        public static readonly DependencyProperty CombineButtonsProperty = DependencyProperty.Register("CombineButtons", typeof(bool), typeof(TaskItemGroup), new PropertyMetadata(true, OnCombineButtonsPropertyChangedCallback));

        static void OnCombineButtonsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public bool UseSmallButtons
        {
            get => (bool)GetValue(UseSmallButtonsProperty);
            set => SetValue(UseSmallButtonsProperty, (value));
        }

        public static readonly DependencyProperty UseSmallButtonsProperty = DependencyProperty.Register("UseSmallButtons", typeof(bool), typeof(TaskItemGroup), new PropertyMetadata(false));

        public bool IsVertical
        {
            get => (bool)GetValue(IsVerticalProperty);
            set => SetValue(IsVerticalProperty, (value));
        }

        public static readonly DependencyProperty IsVerticalProperty = DependencyProperty.Register("IsVertical", typeof(bool), typeof(TaskItemGroup), new PropertyMetadata(false));


        public TaskItemGroup(string programName)
        {
            ProcessName = programName;

            foreach (ProgramWindow p in ProgramWindow.ProgramWindows)
            {
                try
                {
                    if (p.Process.MainModule.FileName == ProcessName)
                    {
                        ProcessWindows.Add(p);
                    }
                }
                catch (Win32Exception ex) { Debug.WriteLine(ex); }
            }
        }

        StackPanel _buttonsStack;
        ToggleButton _groupToggleButton;
        Border _singleGroupTab;
        Border _doubleGroupTab;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _buttonsStack = GetTemplateChild(PartButtonsStack) as StackPanel;

            _groupToggleButton = GetTemplateChild(PartGroupToggleButton) as ToggleButton;

            _singleGroupTab = GetTemplateChild(PartSingleGroupTab) as Border;

            _doubleGroupTab = GetTemplateChild(PartDoubleGroupTab) as Border;
        }
    }

    public class ListIntPtrToListTaskItemButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hwnds = (List<ProgramWindow>)value;
            var Buttons = new List<ToggleButton>();

            foreach (ProgramWindow hwnd in hwnds)
            {
                ToggleButton button = new ToggleButton()
                {
                    Tag = hwnd,
                };

                Buttons.Add(button);
            }
            
            return Buttons;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}