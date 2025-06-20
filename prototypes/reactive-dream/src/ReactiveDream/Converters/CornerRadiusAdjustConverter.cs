using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

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