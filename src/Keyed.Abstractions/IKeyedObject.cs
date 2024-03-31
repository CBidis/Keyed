using System;

namespace Keyed.Abstractions
{

    /// <summary>
    /// indicates an object that is identifiable by a <see cref="TKey"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IKeyedObject<TKey>
    {
        /// <summary>
        /// key identifier
        /// </summary>
       TKey Key { get; }
    }
}
