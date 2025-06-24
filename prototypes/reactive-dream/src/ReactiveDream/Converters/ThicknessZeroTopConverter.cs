using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class ThicknessZeroTopConverter
        : IValueConverter
    {
        public static ThicknessZeroTopConverter Instance { get; } = new();
        private ThicknessZeroTopConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Thickness thickness
                ? new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom)
                : default
            ;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}