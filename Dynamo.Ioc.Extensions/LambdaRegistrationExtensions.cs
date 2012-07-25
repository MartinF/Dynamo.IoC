using System;

namespace Dynamo.Ioc
{
	public static class LambdaRegistrationExtensions
	{
		public static ILifetimeRegistration RegisterLambda<T>(this IIocContainer container, Func<IResolver, T> lambda, object key = null, ILifetime lifetime = null)
			where T : class
		{
			if (container == null)
				throw new ArgumentNullException("container");
			
			if (lifetime == null)
				lifetime = container.DefaultLifetimeFactory();

			var registration = new LambdaRegistration<T>(container, lambda, lifetime, key);
			container.Index.Add(registration);

			return registration;
		}
	}
}
