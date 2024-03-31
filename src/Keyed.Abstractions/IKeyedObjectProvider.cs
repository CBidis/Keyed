namespace Keyed.Abstractions
{
    public interface IKeyedObjectProvider<TKey, out TKeyedObject> 
        where TKeyedObject : IKeyedObject<TKey>
    {
        /// <summary>
        /// Gets the value associated for the given <see cref="TKey"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TKeyedObject this[TKey key] { get; }
        /// <summary>
        /// resolves <see cref="TKeyedObject"/> given a <see cref="TKey"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>concrete implementation of <see cref="TKeyedObject"/></returns>
        TKeyedObject Get(TKey key);
        /// <summary>
        /// resolves <see cref="TKeyedObject"/> given a <see cref="TKey"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>concrete implementation of <see cref="TKeyedObject"/></returns>
        bool TryGet(TKey key, out IKeyedObject<TKey> keyedObject);
    }
}
