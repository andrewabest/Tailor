using System;

namespace Tailor.Test
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type type)
        {
            if (type.IsArray)
            {
                return Array.CreateInstance(type.GetElementType(), 0);
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}