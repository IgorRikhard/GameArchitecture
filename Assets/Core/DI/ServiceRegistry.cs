using System;
using System.Collections.Generic;

namespace Core.DI
{
    /// <summary>
    /// Handles service registration logic
    /// </summary>
    public class ServiceRegistry
    {
        private readonly Dictionary<Type, Func<ServiceContainer, object>> _registrations = new();
        private readonly Dictionary<Type, object> _singletons = new();

        // Internal access for other DI components
        internal Dictionary<Type, Func<ServiceContainer, object>> Registrations => _registrations;
        internal Dictionary<Type, object> Singletons => _singletons;

        public void RegisterFactory<TService>(Func<ServiceContainer, TService> factory) where TService : class
        {
            var serviceType = typeof(TService);
            _registrations[serviceType] = c => factory(c);

            // Register for all interfaces
            RegisterForInterfaces(serviceType, c => factory(c));
        }

        public void RegisterSingleton<TService>(TService instance) where TService : class
        {
            var serviceType = typeof(TService);
            _singletons[serviceType] = instance;

            // Register for the concrete type
            _registrations[serviceType] = c => instance;

            // Register for all interfaces
            RegisterForInterfaces(serviceType, c => instance);
        }

        public void RegisterFactory(Type serviceType, Func<ServiceContainer, object> factory)
        {
            _registrations[serviceType] = factory;
            RegisterForInterfaces(serviceType, factory);
        }

        public void RegisterSingleton(Type serviceType, object instance)
        {
            _singletons[serviceType] = instance;
            _registrations[serviceType] = c => instance;
            RegisterForInterfaces(serviceType, c => instance);
        }

        private void RegisterForInterfaces(Type serviceType, Func<ServiceContainer, object> factory)
        {
            var interfaces = serviceType.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                _registrations[interfaceType] = factory;
            }
        }

        public bool IsRegistered(Type serviceType)
        {
            return _registrations.ContainsKey(serviceType) || _singletons.ContainsKey(serviceType);
        }

        public void Clear()
        {
            _registrations.Clear();
            _singletons.Clear();
        }
    }
}
