using Start9.Api.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Start9.Api.Controls
{
    [TemplatePart(Name = PartGrip, Type = typeof(Button))]
    [TemplatePart(Name = PartOffsetter, Type = typeof(Canvas))]
    [TemplatePart(Name = PartStateText, Type = typeof(TextBlock))]

    public partial class ToggleSwitch : CheckBox
    {
        const string PartGrip = "PART_Grip";
        const string PartOffsetter = "PART_Offsetter";
        const string PartStateText = "PART_StateText";

        public string TrueText
        {
            get => (string)GetValue(TrueTextProperty);
            set => SetValue(TrueTextProperty, value);
        }

        public static readonly DependencyProperty TrueTextProperty =
            DependencyProperty.RegisterAttached("TrueText", typeof(string), typeof(ToggleSwitch),
                new PropertyMetadata("True"));

        public string FalseText
        {
            get => (string)GetValue(FalseTextProperty);
            set => SetValue(FalseTextProperty, value);
        }

        public static readonly DependencyProperty FalseTextProperty =
            DependencyProperty.RegisterAttached("FalseText", typeof(string), typeof(ToggleSwitch),
                new PropertyMetadata("False"));

        public string NullText
        {
            get => (string)GetValue(NullTextProperty);
            set => SetValue(NullTextProperty, value);
        }

        public static readonly DependencyProperty NullTextProperty =
            DependencyProperty.RegisterAttached("NullText", typeof(string), typeof(ToggleSwitch),
                new PropertyMetadata("Null"));

        public double OffsetterWidth
        {
            get => (double)GetValue(OffsetterWidthProperty);
            set => SetValue(OffsetterWidthProperty, value);
        }

        public static readonly DependencyProperty OffsetterWidthProperty =
            DependencyProperty.RegisterAttached("OffsetterWidth", typeof(double), typeof(ToggleSwitch),
                new PropertyMetadata((double)0));

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
            IsCheckedProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(false, OnIsCheckedChanged));
        }

        public ToggleSwitch()
        {
            //Click += delegate { OnClick(); };
        }

        /*protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            HalfWidth = Width / 2;
        }*/

        static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (d as ToggleSwitch);

            toggle.AnimateGripPosition();

            try
            {
                if (toggle.IsChecked == true)
                {
                    toggle._stateText.Text = toggle.TrueText;
                }
                else if (toggle.IsChecked == false)
                {
                    toggle._stateText.Text = toggle.FalseText;
                }
                else
                {
                    toggle._stateText.Text = toggle.NullText;
                }
            } catch { }
        }

        public void AnimateGripPosition()
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(125)
            };

            double targetWidth = 0;

            if ((IsChecked == null) & (IsThreeState))
            {
                //toggle.OffsetterWidth
                targetWidth = 16;
                //animation.To = (toggle.ActualWidth / 2) - 18;

            }
            else if (IsChecked == false)
            {
                targetWidth = 0;
            }
            else
            {
                targetWidth = 32;
                //animation.To = toggle.ActualWidth - 18;
            }

            animation.To = targetWidth;

            animation.Completed += delegate
            {
                OffsetterWidth = targetWidth;
                BeginAnimation(ToggleSwitch.OffsetterWidthProperty, null);
                try
                {
                    Debug.WriteLine(IsChecked.Value.ToString());
                } catch { }
            };

            BeginAnimation(ToggleSwitch.OffsetterWidthProperty, animation);
        }

        Button _grip = new Button();
        Canvas _offsetter = new Canvas();
        TextBlock _stateText = new TextBlock();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _grip = GetTemplateChild(PartGrip) as Button;
            _grip.PreviewMouseLeftButtonDown += (sendurr, args) => ToggleSwitch_PreviewMouseLeftButtonDown(this, args);
            _offsetter = GetTemplateChild(PartOffsetter) as Canvas;
            _stateText = GetTemplateChild(PartStateText) as TextBlock;
            OnIsCheckedChanged(this, new DependencyPropertyChangedEventArgs());
        }

        private void ToggleSwitch_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool? originalValue = (sender as ToggleSwitch).IsChecked;
            //var toggleSwitch = (sender as ToggleSwitch);

            bool isDragging = false;
            double offsetter = OffsetterWidth;
            //var grip = toggleSwitch._grip;

            double toggleX = DpiManager.ConvertPixelsToWpfUnits((sender as ToggleSwitch).PointToScreen(new System.Windows.Point(0, 0)).X);
            double gripInitialX = DpiManager.ConvertPixelsToWpfUnits((sender as ToggleSwitch)._grip.PointToScreen(new System.Windows.Point(0, 0)).X);
            double gripX = DpiManager.ConvertPixelsToWpfUnits((sender as ToggleSwitch)._grip.PointToScreen(new System.Windows.Point(0, 0)).X);

            double cursorStartX = DpiManager.ConvertPixelsToWpfUnits(System.Windows.Forms.Cursor.Position.X);
            double cursorCurrentX = DpiManager.ConvertPixelsToWpfUnits(System.Windows.Forms.Cursor.Position.X);
            double cursorChange = (cursorCurrentX - cursorStartX);
            double offset = (gripX - toggleX) + (cursorCurrentX - cursorStartX);
            //System.Windows.Point cursorStartOffsetPoint = new System.Windows.Point(toggleSwitch.Margin.Left, grip.Margin.Top);

            var timer = new System.Timers.Timer(1);

            timer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        //toggleX = DpiManager.ConvertPixelsToWpfUnits((sender as ToggleSwitch).PointToScreen(new System.Windows.Point(0, 0)).X);
                        cursorCurrentX = DpiManager.ConvertPixelsToWpfUnits(System.Windows.Forms.Cursor.Position.X);

                        cursorChange = (cursorCurrentX - cursorStartX);

                        offset = cursorChange + (gripX - toggleX);
                        Debug.WriteLine(cursorChange.ToString() + "," + offset.ToString());

                        if ((cursorChange > 2) | (cursorChange < -2))
                        {
                            isDragging = true;
                        }

                        OffsetterWidth = offsetter + cursorChange;
                    }
                    else
                    {
                        timer.Stop();
                        //offset = (cursorCurrentX - cursorStartX);
                        if (isDragging)
                        {
                            double isCheckedOffset = 0;
                            if (IsChecked == true)
                            {
                                isCheckedOffset = 32;
                            }
                            else if (IsChecked == null)
                            {
                                isCheckedOffset = 16;
                            }

                            double toggleChange = cursorChange + isCheckedOffset;
                            if (IsThreeState)
                            {
                                if (toggleChange < 10.666666666666666666666666666667)
                                {
                                    IsChecked = false;
                                    Debug.WriteLine("VERTICT: false");
                                }
                                else if (toggleChange > 21.333333333333333333333333333333)
                                {
                                    IsChecked = true;
                                    Debug.WriteLine("VERTICT: true");
                                }
                                else
                                {
                                    IsChecked = null;
                                    Debug.WriteLine("VERTICT: null");
                                }
                            }
                            else
                            {
                                if (toggleChange < 16)
                                {
                                    IsChecked = false;
                                    Debug.WriteLine("VERTICT: false");
                                }
                                else
                                {
                                    IsChecked = true;
                                    Debug.WriteLine("VERTICT: true");
                                }
                            }
                        }
                        else
                        {
                            base.OnClick();
                        }
                        if (originalValue == IsChecked)
                        {
                            AnimateGripPosition();
                        }
                    }
                }));
            };
            timer.Start();
        }

        protected override void OnClick()
        {
            Debug.WriteLine("C L I C C");
            base.OnClick();
        }
    }
}