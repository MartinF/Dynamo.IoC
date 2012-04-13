using System;

namespace Dynamo.Ioc
{
	public sealed class TransientLifetime : ILifetime
	{
		public object GetInstance(Func<IResolver, object> factory, IResolver resolver)
		{
			return factory(resolver);
		}
	}
}