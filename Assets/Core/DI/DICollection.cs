using System.Collections;
using System.Collections.Generic;

namespace Core.DI
{
    public sealed class DICollection<T> : IEnumerable<T>
    {
        private readonly IReadOnlyList<T> _items;

        public DICollection(IReadOnlyList<T> items)
        {
            _items = items ?? System.Array.Empty<T>();
        }

        public int Count => _items.Count;
        public T this[int index] => _items[index];
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}