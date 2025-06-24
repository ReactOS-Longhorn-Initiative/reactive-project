using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public abstract class ThicknessToDoubleConverter
        : IValueConverter
    {
        class LeftConverter
            : ThicknessToDoubleConverter
        {
            protected override double GetForSide(Thickness thickness)
                => thickness.Left;
        }
        public static ThicknessToDoubleConverter Left { get; } = new LeftConverter();




        class TopConverter
            : ThicknessToDoubleConverter
        {
            protected override double GetForSide(Thickness thickness)
                => thickness.Top;
        }
        public static ThicknessToDoubleConverter Top { get; } = new TopConverter();




        class RightConverter
            : ThicknessToDoubleConverter
        {
            protected override double GetForSide(Thickness thickness)
                => thickness.Right;
        }
        public static ThicknessToDoubleConverter Right { get; } = new RightConverter();




        class BottomConverter
            : ThicknessToDoubleConverter
        {
            protected override double GetForSide(Thickness thickness)
                => thickness.Bottom;
        }
        public static ThicknessToDoubleConverter Bottom { get; } = new BottomConverter();




        protected abstract double GetForSide(Thickness thickness);


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is Thickness thickness)
                ? GetForSide(thickness)
                : default
            ;


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}