using System;
using System.Web.Caching;

namespace Dynamo.Ioc.Web
{
	public static class LifetimeExtensions
	{
		public static IConfigurableRegistration RequestLifetime(this IConfigurableRegistration registration, bool disposeOnEnd = false)
		{
			// InstancePerRequest / RequestLifetime ? 

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new RequestLifetime(disposeOnEnd));
			return registration;
		}

		public static IConfigurableRegistration SessionLifetime(this IConfigurableRegistration registration)
		{
			// InstancePerSession / SessionLifetime ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new SessionLifetime());
			return registration;
		}

		public static IConfigurableRegistration CachedLifetime(this IConfigurableRegistration registration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			// HttpCachedLifetime / CachedLifetime ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new CachedLifetime(dependency, itemPriority, itemRemovedCallback));
			return registration;
		}

		public static IConfigurableRegistration CachedLifetime(this IConfigurableRegistration registration, TimeSpan slidingExpiration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			// HttpCachedLifetime / CachedLifetime ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new CachedLifetime(slidingExpiration, dependency, itemPriority, itemRemovedCallback));
			return registration;
		}
	}
}