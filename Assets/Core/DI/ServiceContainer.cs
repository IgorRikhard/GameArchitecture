using System;

namespace Core.DI
{
    /// <summary>
    /// Main DI container that coordinates all DI components
    /// </summary>
    public class ServiceContainer
    {
        private readonly ServiceRegistry _registry;
        private readonly ServiceResolver _resolver;
        private readonly ServiceInjector _injector;
        private readonly DICollectionManager _collectionManager;

        // Internal access for components
        internal ServiceInjector Injector => _injector;

        public ServiceContainer()
        {
            _registry = new ServiceRegistry();
            _collectionManager = new DICollectionManager(this);
            _resolver = new ServiceResolver(_registry, this, _collectionManager);
            _injector = new ServiceInjector(this, _collectionManager);
        }

        #region Registration Methods

        public void Bind<TService>() where TService : class
        {
            var serviceType = typeof(TService);
            _registry.RegisterFactory(serviceType, c => Construct(serviceType));
        }

        public void Bind<TService>(params object[] constructorArgs) where TService : class
        {
            var serviceType = typeof(TService);
            _registry.RegisterFactory(serviceType, c => ConstructWithArgs(serviceType, constructorArgs));
        }

        public void Bind<TService>(Func<ServiceContainer, TService> factory) where TService : class
        {
            _registry.RegisterFactory(factory);
        }

        public void Bind<TService>(TService instance) where TService : class
        {
            _registry.RegisterSingleton(instance);
            AddToCollections(typeof(TService));
        }

        public void BindCollection<TService>() where TService : class
        {
            _collectionManager.GetOrCreateCollection<TService>();
        }

        #endregion

        #region Instantiation Methods

        public TService Instantiate<TService>(params object[] constructorArgs) where TService : class
        {
            var serviceType = typeof(TService);
            object instance;

            if (constructorArgs != null && constructorArgs.Length > 0)
            {
                instance = ConstructWithArgs(serviceType, constructorArgs);
            }
            else
            {
                instance = Construct(serviceType);
            }

            return (TService)instance;
        }

        public TService InstantiateAndBind<TService>(params object[] constructorArgs) where TService : class
        {
            var instance = Instantiate<TService>(constructorArgs);
            _registry.RegisterSingleton(instance);
            AddToCollections(typeof(TService));
            return instance;
        }

        #endregion

        #region Resolution Methods

        public TService Resolve<TService>()
        {
            return _resolver.Resolve<TService>();
        }

        public object Resolve(Type serviceType)
        {
            return _resolver.Resolve(serviceType);
        }

        #endregion

        #region Private Methods

        private object Construct(Type type)
        {
            return _resolver.Construct(type);
        }

        private object ConstructWithArgs(Type type, object[] providedArgs)
        {
            return _resolver.ConstructWithArgs(type, providedArgs);
        }

        private void AddToCollections(Type serviceType)
        {
            // Only add singletons to collections
            if (_registry.Singletons.TryGetValue(serviceType, out var instance))
            {
                _collectionManager.AddToCollection(instance);
            }
        }


        #endregion
    }
}