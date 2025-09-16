using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.DI
{
    /// <summary>
    /// Utility class for common DI operations
    /// </summary>
    public static class DIUtils
    {
        /// <summary>
        /// Checks if a type is a DICollection<T> and extracts the element type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="elementType">The element type if it's a DICollection</param>
        /// <returns>True if the type is a DICollection<T></returns>
        public static bool IsDICollection(Type type, out Type elementType)
        {
            elementType = null;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DICollection<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a type is non-constructible (primitive, string, enum, value type)
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type cannot be constructed by DI</returns>
        public static bool IsNonConstructible(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
                   type.IsEnum || type.IsValueType;
        }

        /// <summary>
        /// Gets all fields marked with [Inject] attribute
        /// </summary>
        /// <param name="type">The type to search</param>
        /// <returns>Enumerable of injectable fields</returns>
        public static IEnumerable<FieldInfo> GetInjectableFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(InjectAttribute), inherit: true));
        }

        /// <summary>
        /// Gets all properties marked with [Inject] attribute
        /// </summary>
        /// <param name="type">The type to search</param>
        /// <returns>Enumerable of injectable properties</returns>
        public static IEnumerable<PropertyInfo> GetInjectableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(InjectAttribute), inherit: true) && p.CanWrite);
        }

        /// <summary>
        /// Resolves a value for injection based on type
        /// </summary>
        /// <param name="type">The type to resolve</param>
        /// <param name="container">The DI container</param>
        /// <param name="collectionManager">The collection manager</param>
        /// <returns>Resolved value</returns>
        public static object ResolveForInjection(Type type, ServiceContainer container, DICollectionManager collectionManager)
        {
            if (IsDICollection(type, out var elementType))
            {
                return collectionManager.GetOrCreateCollection(elementType);
            }
            else
            {
                return container.Resolve(type);
            }
        }

        /// <summary>
        /// Gets the best constructor for a type (the one with most parameters)
        /// </summary>
        /// <param name="type">The type to get constructor for</param>
        /// <returns>The best constructor or null if none found</returns>
        public static ConstructorInfo GetBestConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length);
            
            return constructors.FirstOrDefault();
        }

        /// <summary>
        /// Resolves constructor parameters
        /// </summary>
        /// <param name="constructor">The constructor</param>
        /// <param name="container">The DI container</param>
        /// <param name="collectionManager">The collection manager</param>
        /// <returns>Array of resolved parameters</returns>
        public static object[] ResolveConstructorParameters(ConstructorInfo constructor, ServiceContainer container, DICollectionManager collectionManager)
        {
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                args[i] = ResolveForInjection(paramType, container, collectionManager);
            }

            return args;
        }
    }
}
