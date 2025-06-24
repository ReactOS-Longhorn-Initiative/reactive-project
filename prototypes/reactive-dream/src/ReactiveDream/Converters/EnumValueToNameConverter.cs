using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class EnumValueToNameConverter
        : IValueConverter
    {
        public static EnumValueToNameConverter Instance { get; } = new();
        private EnumValueToNameConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            Type enumType = (Type)parameter;
            return Enum.GetName(enumType, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}