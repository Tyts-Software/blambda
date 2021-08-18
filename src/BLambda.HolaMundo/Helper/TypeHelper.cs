using System;
using System.Collections.Generic;
using System.Linq;

namespace BLambda.HolaMundo.Helper
{
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetAllTypesThatImplementInterface<T>()
        {
            return System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }
    }
}
