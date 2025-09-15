using UnityEngine;

namespace Core.DI
{
    /// <summary>
    /// Example demonstrating how to use the new Instantiate method in ServiceContainer
    /// </summary>
    public class ServiceContainerUsageExample : MonoBehaviour
    {
        private ServiceContainer _container;
        
        private void Start()
        {
            _container = new ServiceContainer();
            DemonstrateInstantiateMethods();
        }
        
        private void DemonstrateInstantiateMethods()
        {
            // Example 1: Instantiate without constructor arguments
            // The container will resolve all dependencies automatically
            var service1 = _container.Instantiate<ExampleService>();
            Debug.Log($"Created service1: {service1.GetType().Name}");
            
            // Example 2: Instantiate with constructor arguments
            // Some dependencies will be provided, others resolved from DI
            var customDependency = new CustomDependency("Custom Value");
            var service2 = _container.Instantiate<ExampleServiceWithArgs>(customDependency, 42);
            Debug.Log($"Created service2: {service2.GetType().Name}");
            
            // Example 3: Instantiate and bind (alias method)
            var service3 = _container.InstantiateAndBind<AnotherService>();
            Debug.Log($"Created service3: {service3.GetType().Name}");
            
            // Now all services are bound and can be resolved
            var resolvedService1 = _container.Resolve<ExampleService>();
            var resolvedService2 = _container.Resolve<ExampleServiceWithArgs>();
            var resolvedService3 = _container.Resolve<AnotherService>();
            
            Debug.Log($"All services resolved successfully: {resolvedService1 != null}, {resolvedService2 != null}, {resolvedService3 != null}");
        }
    }
    
    // Example service classes
    public class ExampleService
    {
        public ExampleService()
        {
            Debug.Log("ExampleService constructor called");
        }
    }
    
    public class ExampleServiceWithArgs
    {
        private readonly CustomDependency _dependency;
        private readonly int _value;
        
        public ExampleServiceWithArgs(CustomDependency dependency, int value)
        {
            _dependency = dependency;
            _value = value;
            Debug.Log($"ExampleServiceWithArgs constructor called with value: {value}");
        }
    }
    
    public class AnotherService
    {
        public AnotherService()
        {
            Debug.Log("AnotherService constructor called");
        }
    }
    
    public class CustomDependency
    {
        public string Value { get; }
        
        public CustomDependency(string value)
        {
            Value = value;
        }
    }
}
