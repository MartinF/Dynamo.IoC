using System;
using System.Linq;
using System.Reflection;

namespace Dynamo.Ioc
{
	internal static class ReflectionHelper
	{
		public static bool IsInstanceOfGenericType(Type genericType, object instance)
		{
			Type type = instance.GetType();
			while (type != null)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
				{
					return true;
				}
				type = type.BaseType;
			}

			return false;
		}

		public static bool IsImplementationOfGenericInterface(this Type baseType, Type interfaceType)
		{
			// IsImplementing ? IsImplementationOf ?
			return baseType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType);
		}

		public static object InvokeGenericMethod(object instance, MethodInfo methodInfo, Type[] typeArguments, object[] parameters)
		{
			if (methodInfo == null)
				throw new ArgumentNullException("methodInfo");

			// Could check that methodInfo.GetParameters().Count() == parameters.Count(), and methodInfo.GetGenericArguments().Count() == typeArguments.Count() ?

			var method = methodInfo.GetGenericMethodDefinition();
			var genericMethod = method.MakeGenericMethod(typeArguments);
			var result = genericMethod.Invoke(instance, parameters);

			return result;
		}

		public static ConstructorInfo GetConstructor(Type type, bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			// Make check if it is a class (not IsInterface or IsAbstract) ? 
			//if (!type.IsClass)
			//    throw new ArgumentException();	// InvalidOperationException() ?

			// Get all Public Constructors
			var constructors = type.GetConstructors(BindingFlags.Instance | (BindingFlags.Public)).ToList();

			// Get all internal constructors
			// IsAssembly = internal, IsFamilyOrAssembly = protected internal
			if (includeInternalCtor)
				constructors.AddRange(type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.IsAssembly || x.IsFamilyOrAssembly));

			if (!constructors.Any())
				throw new InvalidOperationException("Type: " + type.Name + " does not have a public" + (includeInternalCtor ? " or internal" : "") + " constructor.");

			// Select the constructor to use
			ConstructorInfo constructor;
			if (selector == null)
			{
				// Find the Constructor with the most parameters
				// Rank public over internal?
				constructor = constructors.OrderBy(c => c.GetParameters().Length).LastOrDefault();
			}
			else
			{
				constructor = selector(constructors.ToArray());
			}

			if (constructor == null)
				throw new InvalidOperationException("No constructor was selected for Type: " + type.Name);

			return constructor;
		}
	}
}
