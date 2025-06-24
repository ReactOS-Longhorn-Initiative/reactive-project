using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class DoubleArithmeticConverter
        : IValueConverter
    {
        public static DoubleArithmeticConverter Instance { get; } = new();
        private DoubleArithmeticConverter()
        {}


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is double a) && ConverterArithmetic.TryGetNumberAndOperator(parameter, out double b, out ConverterArithmeticOperator op)
                ? ConverterArithmetic.DoArithmetic(a, b, op)
                : default
            ;


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}