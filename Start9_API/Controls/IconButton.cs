using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Start9.Api.Controls
{
    public partial class IconButton : Button
    {
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(IconButton),
                new PropertyMetadata(null));

        public IconButton()
        {

        }
    }
}
