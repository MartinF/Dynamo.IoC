using System;
using System.Threading;

// Rename to ThreadLifetime / PerThreadLifetime
// ThreadLocal<> needs to be disposed.

namespace Dynamo.Ioc
{
	public sealed class ThreadLocalLifetime : ILifetime
	{
		private readonly ThreadLocal<object> _threadLocalInstance = new ThreadLocal<object>();

		public void Init(IRegistration registration)
		{
		}

		public object GetInstance(Func<IResolver, object> factory, IResolver resolver)
		{
			if (!_threadLocalInstance.IsValueCreated)
				_threadLocalInstance.Value = factory(resolver);

			return _threadLocalInstance.Value;
		}

		// Dispose
		// Or implement desctructor to make sure _threadLocalInstance is disposed
	}
}