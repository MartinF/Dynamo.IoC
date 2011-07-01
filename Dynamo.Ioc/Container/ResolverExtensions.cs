using System;
using System.Collections.Generic;
using System.Linq;

// Need to write tests for both GetKeyResolver and LazyResolve 

namespace Dynamo.Ioc
{
	public static class ResolverExtensions
	{
		public static T Resolve<T>(this IResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			return (T)resolver.Resolve(typeof(T));
		}
		public static T Resolve<T>(this IResolver resolver, object key)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			return (T)resolver.Resolve(typeof(T), key);
		}
		public static object Resolve(this IResolver resolver, IRegistrationInfo info)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			if (info == null)
				throw new ArgumentNullException("info");
			
			return info.Key == null ? resolver.Resolve(info.Type) : resolver.Resolve(info.Type, info.Key);
		}

		public static bool TryResolve<T>(this IResolver resolver, out T instance)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			object objOut;
			if (resolver.TryResolve(typeof(T), out objOut))
			{
				instance = (T)objOut;
				return true;
			}

			instance = default(T);
			return false;
		}
		public static bool TryResolve<T>(this IResolver resolver, object key, out T instance)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			object objOut;
			if (resolver.TryResolve(typeof(T), key, out objOut))
			{
				instance = (T)objOut;
				return true;
			}

			instance = default(T);
			return false;
		}
		public static bool TryResolve(this IResolver resolver, IRegistrationInfo info, out object instance)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			if (info == null)
				throw new ArgumentNullException("info");

			return info.Key == null ? resolver.TryResolve(info.Type, out instance) : resolver.TryResolve(info.Type, info.Key, out instance);
		}

		public static IEnumerable<T> ResolveAll<T>(this IResolver container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.ResolveAll(typeof(T)).Cast<T>();
		}
	
		public static IEnumerable<T> TryResolveAll<T>(this IResolver container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.TryResolveAll(typeof(T)).Cast<T>();
		}



		// Create GetKeyResolver which uses object instead of only generic implementation

		public static IKeyResolver<TType, TKey> GetKeyResolver<TType, TKey>(this IResolver resolver)
		{
			return new KeyResolver<TType, TKey>(resolver);
		}



		// Include or not ? - if yes also include a TryLazyResolve() method ? 

		// Check index contains first and throw exception before creating the Func ? 
		// Else it will first throw the exception when trying to resolve later?
		// But could still throw exception even if the index contains the registration though...
			
		public static Func<T> LazyResolve<T>(this IResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			return () => (T)resolver.Resolve(typeof(T));
		}

		public static Func<T> LazyResolve<T>(this IResolver resolver, object key)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			return () => (T)resolver.Resolve(typeof(T), key);
		}

		public static Func<object> LazyResolve(this IResolver resolver, Type type)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			return () => resolver.Resolve(type);
		}

		public static Func<object> LazyResolve(this IResolver resolver, Type type, object key)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			return () => resolver.Resolve(type, key);
		}
	}
}