using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class ThicknessArithmeticConverter
        : IValueConverter
    {
        public static ThicknessArithmeticConverter Instance { get; } = new();
        private ThicknessArithmeticConverter()
        {}


        const char _SEPARATOR = ';';
        static readonly string _SEPARATOR_STR = _SEPARATOR.ToString();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine($"{nameof(ThicknessArithmeticConverter)}.{nameof(Convert)}('{value}', {nameof(targetType)}, '{parameter}', {nameof(culture)})");
            var ret = ConvertInternal(value, parameter);
            Console.WriteLine($"    => '{ret}';");
            return ret;
        }
        Thickness ConvertInternal(object value, object parameter)
        {
            if (!(value is Thickness a))
                goto fail;

            if (!(parameter is string paramStr))
                goto fail;


            if (paramStr.Contains(_SEPARATOR_STR))
            {
                double bL, bT, bR, bB;
                ConverterArithmeticOperator opL, opT, opR, opB;


                string[] operations = paramStr.Split(_SEPARATOR);
                var operationCount = operations.Length;
                if (operationCount == 2)
                {
                    if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[0], out double bLR, out ConverterArithmeticOperator opLR))
                        goto fail;
                    if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[1], out double bTB, out ConverterArithmeticOperator opTB))
                        goto fail;

                    bL = bR = bLR;
                    bT = bB = bTB;

                    opL = opR = opLR;
                    opT = opB = opTB;
                }
                else if (operationCount == 4)
                {
                    if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[0], out bL, out opL))
                        goto fail;
                    else if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[1], out bT, out opT))
                        goto fail;
                    else if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[2], out bR, out opR))
                        goto fail;
                    else if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[3], out bB, out opB))
                        goto fail;
                }
                else
                {
                    goto fail;
                }

                return new Thickness(
                    ConverterArithmetic.DoArithmeticOrNoOp(a.Left,   bL, opL),
                    ConverterArithmetic.DoArithmeticOrNoOp(a.Top,    bT, opT),
                    ConverterArithmetic.DoArithmeticOrNoOp(a.Right,  bR, opR),
                    ConverterArithmetic.DoArithmeticOrNoOp(a.Bottom, bB, opB)
                );
            }
            else if (ConverterArithmetic.TryGetNumberAndOperator(parameter, out double b, out ConverterArithmeticOperator op))
            {
                return new Thickness(
                    ConverterArithmetic.DoArithmetic(a.Left,   b, op),
                    ConverterArithmetic.DoArithmetic(a.Top,    b, op),
                    ConverterArithmetic.DoArithmetic(a.Right,  b, op),
                    ConverterArithmetic.DoArithmetic(a.Bottom, b, op)
                );
            }


            fail:
            return default;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}