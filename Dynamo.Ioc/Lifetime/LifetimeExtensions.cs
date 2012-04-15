using System;

// How to name it?
// Set TransientLifetime
// Use TransientLifetime
// With TransientLifetime *** Register<IFoo, Foo1>().WithTransientLifetime
// But it already have a SetLifetime method - so SetTransientLifetime would make more sense?

namespace Dynamo.Ioc
{
	public static class LifetimeExtensions
	{
		public static IExpressionRegistration TransientLifetime(this IExpressionRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");
			
			registration.SetLifetime(new TransientLifetime());
			return registration;
		}

		public static IExpressionRegistration ContainerLifetime(this IExpressionRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ContainerLifetime());
			return registration;
		}

		public static IExpressionRegistration ThreadLocalLifetime(this IExpressionRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new ThreadLocalLifetime());
			return registration;
		}
	}
}