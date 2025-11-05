using System.Collections.Generic;
using System.Linq;

namespace Core.DI
{
    public sealed class DICollection<T>
    {
        private readonly IReadOnlyList<T> _items;

        public DICollection(IReadOnlyList<T> items)
        {
            _items = items ?? System.Array.Empty<T>();
        }

        public int Count => _items.Count;
        public T this[int index] => _items[index];
        
        // Composition instead of inheritance
        public IReadOnlyList<T> Items => _items;
        
        // Helper methods for common operations
        public bool Contains(T item) => _items.Contains(item);
        public T[] ToArray() => _items.ToArray();
        public List<T> ToList() => new List<T>(_items);
        
        // For iteration, use Items property
        // Example: foreach(var item in collection.Items) { ... }
    }
}