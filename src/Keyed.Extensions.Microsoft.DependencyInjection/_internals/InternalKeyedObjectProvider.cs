using System.Collections.Generic;
using System.Linq;
using Keyed.Abstractions;

namespace Keyed.Extensions.Microsoft.DependencyInjection._internals
{
    /// <summary>
    /// this is an internal default implementation for <see cref="IKeyedObjectProvider{TKey, TKeyedObject}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TKeyedObject"></typeparam>
    internal class InternalKeyedObjectProvider<TKey, TKeyedObject>
        : IKeyedObjectProvider<TKey, TKeyedObject>
        where TKeyedObject : IKeyedObject<TKey>
    {
        private readonly Dictionary<TKey, TKeyedObject> _keyedObjects;

        public InternalKeyedObjectProvider(IEnumerable<TKeyedObject> keyedObjects) 
            => _keyedObjects = keyedObjects.ToDictionary(key => key.Key, value => value);

        public TKeyedObject this[TKey key] => _keyedObjects[key];

        public TKeyedObject Get(TKey key) => _keyedObjects[key];

        public bool TryGet(TKey key, out IKeyedObject<TKey> keyedObject)
        {
            keyedObject = null;

            if (_keyedObjects.TryGetValue(key, out var value))
            {
                keyedObject = value;
                return true;
            }

            return false;
        }
    }
}
