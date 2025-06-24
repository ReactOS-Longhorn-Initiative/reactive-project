using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ReactiveDream.Converters
{
    public enum ConverterArithmeticOperator
        : ushort
    {
        Add = '+',
        Subtract = '-',
        Multiply = '*',
        Divide = '\\',
    }


    public static class ConverterArithmetic
    {
        static readonly Type _CONVERTERARITHMETICOPERATOR_TYPE = typeof(ConverterArithmeticOperator);
        static ConverterArithmetic()
        {
            Dictionary<char, ConverterArithmeticOperator> charToConverterArithmeticOperatorMap = new();
            ConverterArithmeticOperator[] operatorValues = Enum.GetValues<ConverterArithmeticOperator>();
            foreach (ConverterArithmeticOperator op in operatorValues)
            {
                char key = (char)op;
                charToConverterArithmeticOperatorMap[key] = op;
            }
            _CHAR_TO_CONVERTERARITHMETICOPERATOR_MAP = new(charToConverterArithmeticOperatorMap);
        }
        static readonly ReadOnlyDictionary<char, ConverterArithmeticOperator> _CHAR_TO_CONVERTERARITHMETICOPERATOR_MAP;



        public static double DoArithmetic(double a, double b, ConverterArithmeticOperator op)
        {
            switch (op)
            {
                case ConverterArithmeticOperator.Add:
                    return a + b;
                case ConverterArithmeticOperator.Subtract:
                    return a - b;
                case ConverterArithmeticOperator.Multiply:
                    return a * b;
                case ConverterArithmeticOperator.Divide:
                    return a / b;
                default:
                    return a;
            }
        }


        public static bool TryGetNumberAndOperator(object o, out double number, out ConverterArithmeticOperator op)
        {
            if (o == null)
                goto fail;


            string str = o is string s
                ? s
                : o.ToString()
            ;

            if (string.IsNullOrWhiteSpace(str))
                goto fail;

            str = str.Trim();
            if (str.Length <= 1)
                goto fail;


            if (TryGetOperator(str[0], out op) && double.TryParse(str.Substring(1).TrimStart(), out number))
                return true;


            fail:
            op = default;
            number = default;
            return false;
        }


        public static bool TryGetOperator(object o, out ConverterArithmeticOperator op)
        {
            if ((o is char chr) && _CHAR_TO_CONVERTERARITHMETICOPERATOR_MAP.TryGetValue(chr, out ConverterArithmeticOperator opr))
            {
                op = opr;
                return true;
            }
            else if (GenericHelper.TryGetEnum(o, out op))
                return true;


            op = default;
            return false;
        }


        const string _NOOP_STR = "_";
        const ConverterArithmeticOperator _NOOP_OP = (ConverterArithmeticOperator)'N';
        public static bool TryGetNumberAndOperatorOrNoOp(object o, out double number, out ConverterArithmeticOperator op)
        {
            if ((o != null) && ((o is string s ? s : o.ToString()) == _NOOP_STR))
            {
                number = default;
                op = _NOOP_OP;
                return true;
            }
            else
            {
                return TryGetNumberAndOperator(o, out number, out op);
            }
        }


        public static double DoArithmeticOrNoOp(double a, double b, ConverterArithmeticOperator op)
            => op == _NOOP_OP
                ? a
                : DoArithmetic(a, b, op);
    }
}
