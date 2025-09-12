using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.DI
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, Func<ServiceContainer, object>> _registrations = new();
        private readonly Dictionary<Type, object> _singletons = new();
        private readonly Dictionary<Type, List<Func<ServiceContainer, object>>> _collections = new();

        public void Bind<TService>() where TService : class
        {
            var serviceType = typeof(TService);
            BindType(serviceType, c => Construct(serviceType));
        }

        public void Bind<TService>(params object[] constructorArgs) where TService : class
        {
            var serviceType = typeof(TService);
            BindType(serviceType, c => ConstructWithArgs(serviceType, constructorArgs));
        }

        public void Bind<TService>(Func<ServiceContainer, TService> factory) where TService : class
        {
            var serviceType = typeof(TService);
            BindType(serviceType, c => factory(c));
        }

        public void Bind<TService>(TService instance) where TService : class
        {
            var serviceType = typeof(TService);
            _singletons[serviceType] = instance;
            BindType(serviceType, c => instance);
        }

        public void BindCollection<TService>() where TService : class
        {
            var serviceType = typeof(TService);
            var collectionType = typeof(DICollection<TService>);
            
            // Register the collection type
            _registrations[collectionType] = c => CreateCollection(serviceType);
            
            // Register the service type for collection
            if (!_collections.ContainsKey(serviceType))
            {
                _collections[serviceType] = new List<Func<ServiceContainer, object>>();
            }
        }

        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        public object Resolve(Type serviceType)
        {
            // Check singletons first
            if (_singletons.TryGetValue(serviceType, out var singleton))
            {
                return singleton;
            }

            // Check registrations
            if (_registrations.TryGetValue(serviceType, out var factory))
            {
                return factory(this);
            }

            // Check if it's a non-constructible type
            if (IsNonConstructible(serviceType))
            {
                throw new InvalidOperationException($"Cannot resolve type {serviceType}. Type is not registered and cannot be constructed. Please register this type explicitly.");
            }

            // Try to construct
            return Construct(serviceType);
        }

        private void BindType(Type serviceType, Func<ServiceContainer, object> factory)
        {
            // Register for the concrete type
            _registrations[serviceType] = factory;

            // Register for all interfaces
            var interfaces = serviceType.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                _registrations[interfaceType] = factory;
            }

            // Add to collections if they exist
            AddToCollections(serviceType, factory);
        }

        private void AddToCollections(Type serviceType, Func<ServiceContainer, object> factory)
        {
            foreach (var kvp in _collections)
            {
                var collectionType = kvp.Key;
                var factories = kvp.Value;

                // Check if this service type should be added to this collection
                if (collectionType.IsAssignableFrom(serviceType))
                {
                    factories.Add(factory);
                }
            }
        }

        private object CreateCollection(Type serviceType)
        {
            var items = new List<object>();
            
            if (_collections.TryGetValue(serviceType, out var factories))
            {
                foreach (var factory in factories)
                {
                    items.Add(factory(this));
                }
            }

            // Also check for single registrations that match the collection type
            foreach (var kvp in _registrations)
            {
                var registeredType = kvp.Key;
                var factory = kvp.Value;
                
                if (registeredType != serviceType && serviceType.IsAssignableFrom(registeredType))
                {
                    items.Add(factory(this));
                }
            }

            // Convert List<object> to List<T> and then to IReadOnlyList<T>
            var typedItems = Cast(serviceType, items);
            var asReadOnlyMethod = typedItems.GetType().GetMethod("AsReadOnly");
            var readonlyList = asReadOnlyMethod.Invoke(typedItems, null);
            
            var collectionType = typeof(DICollection<>).MakeGenericType(serviceType);
            return Activator.CreateInstance(collectionType, readonlyList);
        }

        private object Cast(Type targetType, IEnumerable<object> items)
        {
            var listType = typeof(List<>).MakeGenericType(targetType);
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");
            
            foreach (var item in items)
            {
                addMethod.Invoke(list, new[] { item });
            }
            
            return list;
        }

        private object Construct(Type type)
        {
            if (IsNonConstructible(type))
            {
                throw new InvalidOperationException($"Cannot construct type {type}. Type is not constructible.");
            }

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length);
            
            var constructor = constructors.FirstOrDefault();
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {type} has no public constructors.");
            }

            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                args[i] = Resolve(paramType);
            }

            var instance = constructor.Invoke(args);
            InjectMembers(instance);
            return instance;
        }

        private object ConstructWithArgs(Type type, object[] providedArgs)
        {
            if (IsNonConstructible(type))
            {
                throw new InvalidOperationException($"Cannot construct type {type}. Type is not constructible.");
            }

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length);
            
            var constructor = constructors.FirstOrDefault();
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {type} has no public constructors.");
            }

            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];
            var usedArgs = new HashSet<int>();

            // First pass: match provided arguments by type
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                
                for (int j = 0; j < providedArgs.Length; j++)
                {
                    if (!usedArgs.Contains(j) && paramType.IsAssignableFrom(providedArgs[j].GetType()))
                    {
                        args[i] = providedArgs[j];
                        usedArgs.Add(j);
                        break;
                    }
                }
            }

            // Second pass: resolve remaining parameters from DI
            for (int i = 0; i < parameters.Length; i++)
            {
                if (args[i] != null) continue;

                var paramType = parameters[i].ParameterType;
                
                if (IsNonConstructible(paramType))
                {
                    throw new InvalidOperationException($"Parameter '{parameters[i].Name}' of type {paramType} requires a provided argument but none was found.");
                }
                
                args[i] = Resolve(paramType);
            }

            var instance = constructor.Invoke(args);
            InjectMembers(instance);
            return instance;
        }

        private bool IsNonConstructible(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || 
                   type.IsEnum || type.IsValueType || type.IsInterface || type.IsAbstract;
        }

        private void InjectMembers(object instance)
        {
            if (instance == null) return;
            var type = instance.GetType();

            // Field injection
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(InjectAttribute), inherit: true));
            
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                object value;
                
                if (IsDICollection(fieldType, out var elementType))
                {
                    value = CreateCollection(elementType);
                }
                else
                {
                    value = Resolve(fieldType);
                }
                
                field.SetValue(instance, value);
            }

            // Property injection
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(InjectAttribute), inherit: true) && p.CanWrite);
            
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                object value;
                
                if (IsDICollection(propertyType, out var elementType))
                {
                    value = CreateCollection(elementType);
                }
                else
                {
                    value = Resolve(propertyType);
                }
                
                property.SetValue(instance, value);
            }
        }

        private bool IsDICollection(Type type, out Type elementType)
        {
            elementType = null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DICollection<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            return false;
        }
    }
}