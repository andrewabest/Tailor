using System;
using System.Collections.Generic;
using System.Linq;

namespace Tailor.Test
{
    public static class DapperQueryTypeExtensions
    {
        public static IEnumerable<Type> ThatHaveParameters(this IEnumerable<Type> types)
        {
            return types.Where(x => x.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IQuery<,>) || i.GetGenericTypeDefinition() == typeof(IAsyncQuery<,>))));
        }

        public static IEnumerable<Type> ThatHaveNoParameters(this IEnumerable<Type> types)
        {
            return types.Where(x => x.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IQuery<>) || i.GetGenericTypeDefinition() == typeof(IAsyncQuery<>))));
        }
    }
}