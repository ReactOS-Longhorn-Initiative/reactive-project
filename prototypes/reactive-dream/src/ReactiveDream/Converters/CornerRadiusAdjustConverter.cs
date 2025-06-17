using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ReactiveDream.Controls
{
    public class CornerRadiusAdjustConverter
        : IValueConverter
    {
        public static CornerRadiusAdjustConverter Instance { get; } = new();
        private CornerRadiusAdjustConverter()
        {}


        static CornerRadius GetFromParam(object parameter)
        {
            if (parameter is CornerRadius r)
                return r;
            else if (parameter != null)
                return CornerRadius.Parse(parameter.ToString());
            else
                throw new ArgumentNullException(nameof(parameter));
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius radius = GetFromParam(value);
            CornerRadius adjustBy = GetFromParam(parameter);

            return new CornerRadius(
                radius.TopLeft + adjustBy.TopLeft,
                radius.TopRight + adjustBy.TopRight,
                radius.BottomRight + adjustBy.BottomRight,
                radius.BottomLeft + adjustBy.BottomLeft
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius radius = GetFromParam(value);
            CornerRadius adjustBy = GetFromParam(parameter);

            return new CornerRadius(
                radius.TopLeft - adjustBy.TopLeft,
                radius.TopRight - adjustBy.TopRight,
                radius.BottomRight - adjustBy.BottomRight,
                radius.BottomLeft - adjustBy.BottomLeft
            );
        }
    }
}