using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ReactiveDream.Controls
{
    public class MWindowLeftTopToMarginConverter : IMultiValueConverter
    {
        static readonly Thickness _EMPTY = new Thickness(0);
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