using System;

namespace Core.DI
{
    /// <summary>
    /// Handles dependency injection logic
    /// </summary>
    public class ServiceInjector
    {
        private readonly ServiceContainer _container;
        private readonly DICollectionManager _collectionManager;

        public ServiceInjector(ServiceContainer container, DICollectionManager collectionManager)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _collectionManager = collectionManager ?? throw new ArgumentNullException(nameof(collectionManager));
        }

        public void InjectMembers(object instance)
        {
            if (instance == null) return;

            var type = instance.GetType();
            InjectFields(instance, type);
            InjectProperties(instance, type);
        }

        private void InjectFields(object instance, Type type)
        {
            foreach (var field in DIUtils.GetInjectableFields(type))
            {
                var value = DIUtils.ResolveForInjection(field.FieldType, _container, _collectionManager);
                field.SetValue(instance, value);
            }
        }

        private void InjectProperties(object instance, Type type)
        {
            foreach (var property in DIUtils.GetInjectableProperties(type))
            {
                var value = DIUtils.ResolveForInjection(property.PropertyType, _container, _collectionManager);
                property.SetValue(instance, value);
            }
        }

    }
}
