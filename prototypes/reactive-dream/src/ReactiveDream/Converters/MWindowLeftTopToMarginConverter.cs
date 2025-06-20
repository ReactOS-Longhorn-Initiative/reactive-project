using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ReactiveDream.Controls
{
    public class MWindowLeftTopToMarginConverter
        : IMultiValueConverter
    {
        static readonly Thickness _EMPTY = new(0);
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                return _EMPTY;
            
            if ((values[0] is double left) && (values[1] is double top))
                return new Thickness(left, top, 0, 0);
            
            return _EMPTY;
        }


        static readonly MWindowLeftTopToMarginConverter _instance = new();
        public static MWindowLeftTopToMarginConverter Instance
        {
            get => _instance;
        }
    }

    /*
    public class MWindowAnimationStateToIsOpenedBooleanConverter
        : IValueConverter
    {
        public static readonly MWindowAnimationStateToIsOpenedBooleanConverter Instance = new();
        private MWindowAnimationStateToIsOpenedBooleanConverter()
            : base()
        {}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is MWindowAnimationState animState)
                ? animState == MWindowAnimationState.Opened
                : false
            ;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => ((value is bool val) && val)
                ? MWindowAnimationState.Opened
                : BindingOperations.DoNothing
            ;
    }
    */
}