using System;
using System.Runtime.Caching;

// Adds support for fluent like syntax

// SetLifetime vs WithLifetime ?

namespace Dynamo.Ioc
{
	public static class LifetimeRegistrationExtensions
	{
		public static T SetLifetime<T>(this T registration, ILifetime lifetime)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.Lifetime = lifetime;
			
			return registration;
		}

		public static T WithTransientLifetime<T>(this T registration)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new TransientLifetime());
			return registration;
		}

		public static T WithContainerLifetime<T>(this T registration)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ContainerLifetime());
			return registration;
		}

		public static T WithThreadLocalLifetime<T>(this T registration)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ThreadLocalLifetime());
			return registration;
		}

		public static T WithCachedLifetime<T>(this T registration, CacheItemPolicy policy)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");
			if (policy == null)
				throw new ArgumentNullException("policy");

			registration.SetLifetime(new CachedLifetime(policy));

			return registration;
		}
	}
}
