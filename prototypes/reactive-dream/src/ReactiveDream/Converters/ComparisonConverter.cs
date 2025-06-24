using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class ComparisonConverter
        : IValueConverter
    {
        public static ComparisonConverter Instance { get; } = new();
        private ComparisonConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value == parameter;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}