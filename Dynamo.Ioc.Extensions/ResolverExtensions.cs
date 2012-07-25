using System;

// Write tests

namespace Dynamo.Ioc
{
	public static partial class ResolverExtensions
	{
		#region LazyResolve
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
		#endregion

		#region GetFactory
		public static Func<object> GetFactory(this IResolver container, Type type)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			var reg = container.Index.Get(type);

			return () => reg.GetInstance();
		}
		public static Func<object> GetFactory(this IResolver container, Type type, object key)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			var reg = container.Index.Get(type, key);

			return () => reg.GetInstance();
		}
		public static Func<T> GetFactory<T>(this IResolver container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			var reg = container.Index.Get<T>();

			return () => (T)reg.GetInstance();
		}
		public static Func<T> GetFactory<T>(this IResolver container, object key)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			var reg = container.Index.Get<T>(key);

			return () => (T)reg.GetInstance();
		}
		#endregion

		public static KeyResolver<TType, TKey> GetKeyResolver<TType, TKey>(this IResolver resolver)
		{
			return new KeyResolver<TType, TKey>(resolver);
		}
	}
}
