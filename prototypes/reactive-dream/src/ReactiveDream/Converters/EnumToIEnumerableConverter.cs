using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class EnumToIEnumerableConverter
        : IValueConverter
    {
        public static EnumToIEnumerableConverter Instance { get; } = new();
        private EnumToIEnumerableConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type enumType = (Type)(parameter ?? value);
            return Enum.GetValues(enumType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}