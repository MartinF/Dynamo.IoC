using System;
using System.Web.Caching;

// Name how ? 
// Set-Request/Session/Cached-Lifetime ?
// Prefix with Http ? - HttpRequestLifetime / SetHttpRequestLifetime ?

// Instead of Http -RequestLifetime - prefix with Web ? Web-RequestLifetime / WebSessionLifetime ?

namespace Dynamo.Ioc.Web
{
	public static class LifetimeExtensions
	{
		public static IExpressionRegistration RequestLifetime(this IExpressionRegistration registration, bool disposeOnEnd = false)
		{
			// HttpRequestLifetime / InstancePerRequest / RequestLifetime ? 

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new RequestLifetime(disposeOnEnd));
			return registration;
		}

		public static IExpressionRegistration SessionLifetime(this IExpressionRegistration registration)
		{
			// HttpSessionLifetime / InstancePerSession / SessionLifetime ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new SessionLifetime());
			return registration;
		}

		public static IExpressionRegistration CachedLifetime(this IExpressionRegistration registration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			// HttpCachedLifetime / CachedLifetime ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new CachedLifetime(dependency, itemPriority, itemRemovedCallback));
			return registration;
		}

		public static IExpressionRegistration CachedLifetime(this IExpressionRegistration registration, TimeSpan slidingExpiration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			// HttpCachedLifetime / CachedLifetime ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new CachedLifetime(slidingExpiration, dependency, itemPriority, itemRemovedCallback));
			return registration;
		}
	}
}