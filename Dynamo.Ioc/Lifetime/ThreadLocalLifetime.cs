using System;
using System.Threading;

// Rename to ThreadLifetime / PerThreadLifetime ?

namespace Dynamo.Ioc
{
	public sealed class ThreadLocalLifetime : ILifetime
	{
		private ThreadLocal<object> _threadLocalInstance = new ThreadLocal<object>();

		public object GetInstance(IInstanceFactoryRegistration registration)
		{
			if (!_threadLocalInstance.IsValueCreated)
				_threadLocalInstance.Value = registration.CreateInstance();

			return _threadLocalInstance.Value;
		}

		public void Dispose()
		{
			// Only handle disposing of dynamo ioc related references
			// If instance registered uses unmanaged resources and should be disposed the user should implement a finalizer

			if (_threadLocalInstance != null)
			{
				_threadLocalInstance.Dispose();
				_threadLocalInstance = null;
			}
		}
	}
}