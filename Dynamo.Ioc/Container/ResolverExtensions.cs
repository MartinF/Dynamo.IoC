using System;

// Need to write tests for both GetKeyResolver and LazyResolve 

// Move to Container and IResolver interface ?

namespace Dynamo.Ioc
{
	public static class ResolverExtensions
	{
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