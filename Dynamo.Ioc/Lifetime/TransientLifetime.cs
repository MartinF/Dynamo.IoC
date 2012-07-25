using System;

namespace Dynamo.Ioc
{
	public sealed class TransientLifetime : ILifetime
	{
		public object GetInstance(IInstanceFactoryRegistration registration)
		{
			return registration.CreateInstance();
		}

		public void Dispose()
		{
		}
	}
}