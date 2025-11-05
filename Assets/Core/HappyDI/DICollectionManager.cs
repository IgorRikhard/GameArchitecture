using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.DI
{
    public class DICollectionManager
    {
        private readonly Dictionary<Type, object> _collections = new();
        private readonly ServiceContainer _container;

        public DICollectionManager(ServiceContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public DICollection<T> GetOrCreateCollection<T>() where T : class
        {
            var collectionType = typeof(T);
            
            if (_collections.TryGetValue(collectionType, out var existingCollection))
            {
                return (DICollection<T>)existingCollection;
            }

            // Create new collection
            var items = new List<T>();
            var collection = new DICollection<T>(items);
            _collections[collectionType] = collection;
            
            return collection;
        }

        public object GetOrCreateCollection(Type elementType)
        {
            if (_collections.TryGetValue(elementType, out var existingCollection))
            {
                return existingCollection;
            }

            // Create new collection using reflection
            var listType = typeof(List<>).MakeGenericType(elementType);
            var items = Activator.CreateInstance(listType);
            
            var collectionType = typeof(DICollection<>).MakeGenericType(elementType);
            var collection = Activator.CreateInstance(collectionType, items);
            
            _collections[elementType] = collection;
            return collection;
        }

        public void AddToCollection(object instance)
        {
            var instanceType = instance.GetType();
            
            // Find all collections that can contain this type
            foreach (var kvp in _collections)
            {
                var collectionElementType = kvp.Key;
                var collection = kvp.Value;
                
                if (collectionElementType.IsAssignableFrom(instanceType))
                {
                    // Get the internal list and add the instance
                    var itemsProperty = collection.GetType().GetProperty("Items");
                    var items = itemsProperty.GetValue(collection);
                    
                    // Get the Add method and call it
                    var addMethod = items.GetType().GetMethod("Add");
                    addMethod.Invoke(items, new[] { instance });
                }
            }
        }

        public void Clear()
        {
            _collections.Clear();
        }
    }
}