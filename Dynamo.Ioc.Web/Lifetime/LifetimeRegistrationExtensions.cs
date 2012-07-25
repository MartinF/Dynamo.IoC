using System;

namespace Dynamo.Ioc.Web
{
	public static class LifetimeRegistrationExtensions
	{
		public static T WithRequestLifetime<T>(this T registration, bool disposeOnEnd = false)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new RequestLifetime(disposeOnEnd));
			return registration;
		}

		public static T WithSessionLifetime<T>(this T registration)
			where T : ILifetimeRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.SetLifetime(new SessionLifetime());
			return registration;
		}
	}
}