using System;
using System.Collections.Generic;
using Keyed.Abstractions;

namespace Keyed.Tests.Services
{
    public interface IOrdersReposity : IKeyedObject<OrderType>
    {
        public static readonly IEnumerable<OrderType> Keys = Enum.GetValues<OrderType>();
    }

    public class OrdersRepository : IOrdersReposity
    {
        public OrderType Key { get; }

        public OrdersRepository(OrderType key)
        {
            Key = key;
        }
    }
}
