using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ReactiveDream.Controls
{
    public class EnumValueToNameConverter
        : IValueConverter
    {
        public static EnumValueToNameConverter Instance { get; } = new();
        private EnumValueToNameConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type enumType = (Type)parameter;
            return Enum.GetName(enumType, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}