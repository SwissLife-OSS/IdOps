using System;
using System.Collections.Generic;

namespace IdOps.GraphQL.Extensions
{
    public static class EnumToValueList
    {
        public static IEnumerable<EnumValueItem<TEnum>> GetValueList<TEnum>()
            where TEnum : struct, Enum
        {
            foreach (TEnum value in Enum.GetValues<TEnum>())
            {
                yield return new EnumValueItem<TEnum>(value.ToString(), value);
            }
        }
    }

    public record EnumValueItem<TEnum>(string Text, TEnum Value)
        where TEnum : struct, Enum;
}
