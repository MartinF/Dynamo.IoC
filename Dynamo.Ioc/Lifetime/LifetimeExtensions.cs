using System;

namespace Dynamo.Ioc
{
	public static class LifetimeExtensions
	{
		public static IExpressionRegistration<T> TransientLifetime<T>(this IExpressionRegistration<T> registration)
		{
			// TransientLifetime / TransientInstance ?

			if (registration == null)
				throw new ArgumentNullException("registration");
			
			registration.SetLifetime(new TransientLifetime());
			return registration;
		}

		public static IExpressionRegistration<T> ContainerLifetime<T>(this IExpressionRegistration<T> registration)
		{
			// ContainerLifetime / SingletonInstance ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ContainerLifetime());
			return registration;
		}

		public static IExpressionRegistration<T> ThreadLocalLifetime<T>(this IExpressionRegistration<T> registration)
		{
			// ThreadLocalLifetime / ThreadLocalInstance ?

			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ThreadLocalLifetime());
			return registration;
		}
	}
}