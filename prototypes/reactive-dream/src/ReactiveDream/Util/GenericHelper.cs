using System;

namespace ReactiveDream
{
    public static class GenericHelper
    {
        public static bool TryGet<T>(this T? maybe, out T result)
            where T : struct
        {
            if (maybe == null)
                goto fail;
            else if (!maybe.HasValue)
                goto fail;
            else
            {
                result = maybe.Value;
                return true;
            }


            fail:
            result = default;
            return false;
        }


        public static bool TryGetEnum<TEnum>(object o, out TEnum value)
            where TEnum : struct, Enum, IConvertible
        {
            if (o == null)
                goto fail;
            else if (o is TEnum val0)
            {
                value = val0;
                return true;
            }

            
            Type tEnumType = typeof(TEnum);
            if ((o is string str) && (!string.IsNullOrWhiteSpace(str)) && Enum.TryParse(str, out TEnum val1))
            {
                value = val1;
                return true;
            }
            else if (Enum.ToObject(tEnumType, o) is TEnum val2)
            {
                value = val2;
                return true;
            }



            fail:
            value = default;
            return false;
        }
    }
}