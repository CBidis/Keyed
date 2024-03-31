using System;
using Keyed.Abstractions;
using Keyed.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Keyed.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection _services;

        public ServiceCollectionExtensionsTests() 
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void Resolve()
        {
            _services.AddTransientKeyedServiceProvider<OrderType, IOrdersReposity, OrdersRepository>(IOrdersReposity.Keys);
            _services.AddTransientKeyedServiceProvider<OrderType, IOrdersService, OrdersService>(IOrdersReposity.Keys);
            var serviceProvider = _services.BuildServiceProvider();

            var ordersRepoProvider = serviceProvider.GetRequiredService<IKeyedObjectProvider<OrderType, IOrdersReposity>>();
            var ordersServiceProvider = serviceProvider.GetRequiredService<IKeyedObjectProvider<OrderType, IOrdersService>>();
        }

        [Fact]
        public void Resolve_2()
        {
            _services.AddTransientKeyedServiceProvider<AssignmentType, AssignmentHandler>(Enum.GetValues<AssignmentType>());
            var serviceProvider = _services.BuildServiceProvider();

            var assginmentHandlersProvider = serviceProvider.GetRequiredService<IKeyedObjectProvider<AssignmentType, AssignmentHandler>>();
        }
    }
}
