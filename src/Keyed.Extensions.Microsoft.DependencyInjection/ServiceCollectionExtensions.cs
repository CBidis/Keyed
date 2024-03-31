using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Keyed.Abstractions;
using Keyed.Extensions.Microsoft.DependencyInjection._internals;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientKeyedServiceProvider<TKey, TIKeyedService, TConcreteKeyedService>(
            this IServiceCollection services, IEnumerable<TKey> keys)
            where TIKeyedService : class, IKeyedObject<TKey>
            where TConcreteKeyedService : class, TIKeyedService =>
            AddKeyedServiceProvider<TKey, TIKeyedService, TConcreteKeyedService>(services, keys, ServiceLifetime.Transient);

        public static IServiceCollection AddTransientKeyedServiceProvider<TKey, TConcreteKeyedService>(
            this IServiceCollection services, IEnumerable<TKey> keys)
            where TConcreteKeyedService : class, IKeyedObject<TKey> =>
            AddKeyedServiceProvider<TKey, TConcreteKeyedService>(services, keys, ServiceLifetime.Transient);

        private static IServiceCollection AddKeyedServiceProvider<TKey, TIKeyedService, TConcreteKeyedService>(this IServiceCollection services,
            IEnumerable<TKey> keys, ServiceLifetime serviceLifetime)
            where TIKeyedService : class, IKeyedObject<TKey>
            where TConcreteKeyedService : class, TIKeyedService
        {
            EnsureKeysExistOrThrow(keys);

            var serviceProviderDescriptor = new ServiceDescriptor(typeof(IKeyedObjectProvider<TKey, TIKeyedService>)
                , factory => ResolveKeyedObjectProvider<TKey, TConcreteKeyedService>(factory, keys), serviceLifetime);

            //avoiding double registrations
            services.TryAdd(serviceProviderDescriptor);

            return services;
        }

        private static IServiceCollection AddKeyedServiceProvider<TKey, TConcreteKeyedService>(this IServiceCollection services,
            IEnumerable<TKey> keys, ServiceLifetime serviceLifetime)
            where TConcreteKeyedService : class, IKeyedObject<TKey>
        {
            EnsureKeysExistOrThrow(keys);

            var serviceProviderDescriptor = new ServiceDescriptor(typeof(IKeyedObjectProvider<TKey, TConcreteKeyedService>)
                , factory => ResolveKeyedObjectProvider<TKey, TConcreteKeyedService>(factory, keys), serviceLifetime);

            //avoiding double registrations
            services.TryAdd(serviceProviderDescriptor);

            return services;
        }

        private static IKeyedObjectProvider<TKey, TKeyedService> ResolveKeyedObjectProvider<TKey, TKeyedService>(IServiceProvider serviceProvider
            , IEnumerable<TKey> keys)
            where TKeyedService : class, IKeyedObject<TKey>
        {
            var keyedConstructorInfo = InternalObjectsHelper.GetKeyedConstructorParameters<TKey, TKeyedService>();

            if (keyedConstructorInfo is null)
            {
                throw new ArgumentNullException(nameof(keyedConstructorInfo), $"no public constructor found for key of type {typeof(TKey)}");
            }

            var keyedServices = new List<TKeyedService>();

            foreach (var key in keys)
            {
                var constructorParameters
                    = ResolvedKeyedObjectConstructorParameters<TKey, TKeyedService>(serviceProvider, key, keyedConstructorInfo.GetParameters());

                keyedServices.Add((TKeyedService)keyedConstructorInfo.Invoke(constructorParameters.ToArray()));
            }

            return new InternalKeyedObjectProvider<TKey, TKeyedService>(keyedServices);
        }

        private static List<object> ResolvedKeyedObjectConstructorParameters<TKey, TKeyedService>(IServiceProvider serviceProvider
            , TKey key
            , ParameterInfo[] constructorParameters)
            where TKeyedService : class, IKeyedObject<TKey>
        {
            var constructorParams = new List<object>();

            foreach (var constructorParameter in constructorParameters)
            {
                if (constructorParameter.ParameterType == typeof(TKey))
                {
                    constructorParams.Add(key);
                    continue;
                }

                var dependency = serviceProvider.GetService(constructorParameter.ParameterType);

                // try to see if there is a dependency to another keyed service.
                if (dependency is null
                    && constructorParameter.ParameterType.GetInterfaces()
                        .Any(p => p.IsAssignableFrom(typeof(TKeyedService))))
                {
                    var genericTypeDefinition = typeof(IKeyedObjectProvider<,>);
                    //construct generic parameters
                    var constructedType = genericTypeDefinition.MakeGenericType(typeof(TKey), constructorParameter.ParameterType);
                    // request from service provider and cast to known type
                    var dependencyServiceProvider 
                        = (IKeyedObjectProvider<TKey, IKeyedObject<TKey>>)serviceProvider.GetRequiredService(constructedType);

                    constructorParams.Add(dependencyServiceProvider.Get(key));

                    continue;
                }

                //better be safe than sorry, don't allow null registrations.
                constructorParams.Add(dependency ?? throw new InvalidOperationException(
                $"cannot resolve parameter type {constructorParameter.ParameterType.Name} " +
                $"for Type {typeof(TKeyedService).Name}"));
            }

            return constructorParams;
        }

        private static void EnsureKeysExistOrThrow<TKey>(IEnumerable<TKey> keys)
        {
            if (!keys?.Any() ?? false)
            {
                throw new ArgumentException("No Keys Provided", nameof(keys));
            }
        }

    }
}
