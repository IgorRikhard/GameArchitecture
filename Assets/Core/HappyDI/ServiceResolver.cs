using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.DI
{
    /// <summary>
    /// Handles service resolution logic
    /// </summary>
    public class ServiceResolver
    {
        private readonly ServiceRegistry _registry;
        private readonly ServiceContainer _container;
        private readonly DICollectionManager _collectionManager;

        public ServiceResolver(ServiceRegistry registry, ServiceContainer container, DICollectionManager collectionManager)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _collectionManager = collectionManager ?? throw new ArgumentNullException(nameof(collectionManager));
        }

        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        public object Resolve(Type serviceType)
        {
            // Check if it's a DICollection<T>
            if (DIUtils.IsDICollection(serviceType, out var elementType))
            {
                return _collectionManager.GetOrCreateCollection(elementType);
            }

            // Check singletons first
            if (_registry.Singletons.TryGetValue(serviceType, out var singleton))
            {
                return singleton;
            }

            // Check registrations
            if (_registry.Registrations.TryGetValue(serviceType, out var factory))
            {
                return factory(_container);
            }

            // Check if it's a non-constructible type
            if (DIUtils.IsNonConstructible(serviceType))
            {
                throw new InvalidOperationException(
                    $"Cannot resolve type {serviceType}. Type is not registered and cannot be constructed. Please register this type explicitly.");
            }

            // Try to construct
            return Construct(serviceType);
        }

        public object Construct(Type type)
        {
            if (DIUtils.IsNonConstructible(type))
            {
                throw new InvalidOperationException($"Cannot construct type {type}. Type is not constructible.");
            }

            var constructor = DIUtils.GetBestConstructor(type);
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {type} has no public constructors.");
            }

            var args = DIUtils.ResolveConstructorParameters(constructor, _container, _collectionManager);
            var instance = constructor.Invoke(args);
            _container.Injector.InjectMembers(instance);
            return instance;
        }

        public object ConstructWithArgs(Type type, object[] providedArgs)
        {
            if (DIUtils.IsNonConstructible(type))
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
                
                if (DIUtils.IsNonConstructible(paramType))
                {
                    throw new InvalidOperationException($"Parameter '{parameters[i].Name}' of type {paramType} requires a provided argument but none was found.");
                }
                
                if (DIUtils.IsDICollection(paramType, out var elementType))
                {
                    args[i] = _collectionManager.GetOrCreateCollection(elementType);
                }
                else
                {
                    args[i] = Resolve(paramType);
                }
            }

            var instance = constructor.Invoke(args);
            _container.Injector.InjectMembers(instance);
            return instance;
        }

    }
}
