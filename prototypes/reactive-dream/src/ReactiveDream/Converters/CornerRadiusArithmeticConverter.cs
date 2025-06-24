using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ReactiveDream.Converters
{
    public class CornerRadiusArithmeticConverter
        : IValueConverter
    {
        public static CornerRadiusArithmeticConverter Instance { get; } = new();
        private CornerRadiusArithmeticConverter()
        {}


        const char _SEPARATOR = ';';
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CornerRadius a))
                goto fail;

            if (!(parameter is string paramStr))
                goto fail;


            if (paramStr.Contains(_SEPARATOR))
            {
                double bTL, bTR, bBR, bBL;
                ConverterArithmeticOperator opTL, opTR, opBR, opBL;


                string[] operations = paramStr.Split(_SEPARATOR);
                var operationCount = operations.Length;
                if (operationCount == 2)
                {
                    if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[0], out double bT, out ConverterArithmeticOperator opT))
                        goto fail;
                    if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[1], out double bB, out ConverterArithmeticOperator opB))
                        goto fail;

                    bTL = bTR = bT;
                    bBR = bBL = bB;

                    opTL = opTR = opT;
                    opBR = opBL = opB;
                }
                else if (operationCount == 4)
                {
                    if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[0], out bTL, out opTL))
                        goto fail;
                    else if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[1], out bTR, out opTR))
                        goto fail;
                    else if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[2], out bBR, out opBR))
                        goto fail;
                    else if (!ConverterArithmetic.TryGetNumberAndOperatorOrNoOp(operations[3], out bBL, out opBL))
                        goto fail;
                }
                else
                {
                    goto fail;
                }

                return new CornerRadius(
                    ConverterArithmetic.DoArithmeticOrNoOp(a.TopLeft,     bTL, opTL),
                    ConverterArithmetic.DoArithmeticOrNoOp(a.TopRight,    bTR, opTR),
                    ConverterArithmetic.DoArithmeticOrNoOp(a.BottomRight, bBR, opBR),
                    ConverterArithmetic.DoArithmeticOrNoOp(a.BottomLeft,  bBL, opBL)
                );
            }
            else if (ConverterArithmetic.TryGetNumberAndOperator(parameter, out double b, out ConverterArithmeticOperator op))
            {
                return new CornerRadius(
                    ConverterArithmetic.DoArithmetic(a.TopLeft,     b, op),
                    ConverterArithmetic.DoArithmetic(a.TopRight,    b, op),
                    ConverterArithmetic.DoArithmetic(a.BottomRight, b, op),
                    ConverterArithmetic.DoArithmetic(a.BottomLeft,  b, op)
                );
            }


            fail:
            return default;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}