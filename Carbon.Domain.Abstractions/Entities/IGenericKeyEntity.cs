using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    /// An interface for entity objects with generic unique id.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
