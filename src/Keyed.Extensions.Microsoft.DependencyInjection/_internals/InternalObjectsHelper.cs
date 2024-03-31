using System.Linq;
using System.Reflection;

namespace Keyed.Extensions.Microsoft.DependencyInjection._internals
{
    internal static class InternalObjectsHelper
    {
        internal static ConstructorInfo GetKeyedConstructorParameters<TKey, T>() where T : class 
            => typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.GetParameters()
                .Any(c => c.ParameterType == typeof(TKey)));
    }
}
