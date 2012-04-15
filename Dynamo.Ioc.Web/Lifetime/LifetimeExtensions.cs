using System;
using System.Web.Caching;

// Name how ? 
// Set / With -Request/Session/Cached-Lifetime ?
// Prefix with Http or Web? - HttpRequestLifetime / SetHttpRequestLifetime ?

namespace Dynamo.Ioc.Web
{
	public static class LifetimeExtensions
	{
		public static IExpressionRegistration RequestLifetime(this IExpressionRegistration registration, bool disposeOnEnd = false)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new RequestLifetime(disposeOnEnd));
			return registration;
		}

		public static IExpressionRegistration SessionLifetime(this IExpressionRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new SessionLifetime());
			return registration;
		}

		public static IExpressionRegistration CachedLifetime(this IExpressionRegistration registration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new CachedLifetime(dependency, itemPriority, itemRemovedCallback));
			return registration;
		}

		public static IExpressionRegistration CachedLifetime(this IExpressionRegistration registration, TimeSpan slidingExpiration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new CachedLifetime(slidingExpiration, dependency, itemPriority, itemRemovedCallback));
			return registration;
		}
	}
}