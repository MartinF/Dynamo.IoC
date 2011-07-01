using System;

namespace Dynamo.Ioc
{
	public static class LifetimeExtensions
	{
		public static IConfigurableRegistration TransientLifetime(this IConfigurableRegistration registration)
		{
			// TransientLifetime / TransientInstance ?

			if (registration == null)
				throw new ArgumentNullException("registration");
			
			registration.SetLifetime(new TransientLifetime());
			return registration;
		}

		public static IConfigurableRegistration ContainerLifetime(this IConfigurableRegistration registration)
		{
			// ContainerLifetime / SingletonInstance ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ContainerLifetime());
			return registration;
		}

		public static IConfigurableRegistration ThreadLocalLifetime(this IConfigurableRegistration registration)
		{
			// ThreadLocalLifetime / ThreadLocalInstance ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ThreadLocalLifetime());
			return registration;
		}
	}
}