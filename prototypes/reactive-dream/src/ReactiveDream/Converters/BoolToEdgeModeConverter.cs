using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ReactiveDream.Converters
{
    public class BoolToEdgeModeConverter
        : IValueConverter
    {
        public static BoolToEdgeModeConverter Instance { get; } = new();
        private BoolToEdgeModeConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool antialias)
                return antialias
                    ? EdgeMode.Antialias
                    : EdgeMode.Aliased
                ;
            else
                return EdgeMode.Unspecified;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}