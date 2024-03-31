using Keyed.Abstractions;

namespace Keyed.Tests.Services
{
    public interface IOrdersService : IKeyedObject<OrderType>
    {
        public IOrdersReposity Reposity { get; }
    }

    public class OrdersService : IOrdersService
    {
        public OrderType Key { get; }
        public IOrdersReposity Reposity { get; }

        public OrdersService(OrderType key, IOrdersReposity reposity)
        {
            Key = key;
            Reposity = reposity;
        }
    }
}
