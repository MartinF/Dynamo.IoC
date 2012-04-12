using System;

namespace Dynamo.Ioc
{
	public sealed class TransientLifetime : ILifetime
	{
		public void Init(IRegistration registration)
		{
		}

		public object GetInstance(Func<IResolver, object> factory, IResolver resolver)
		{
			return factory(resolver);
		}
	}
}