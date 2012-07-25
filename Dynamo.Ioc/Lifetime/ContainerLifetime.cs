using System;

// Container / Singleton - only created the first time the GetInstance is called - cached for the rest of the time.

// lock could be removed by initializing it early instead of lazy

namespace Dynamo.Ioc
{
	public sealed class ContainerLifetime : ILifetime
	{
		private volatile object _instance = null;
		private readonly object _lock = new object();
		
		public object GetInstance(IInstanceFactoryRegistration registration)
		{
			// return _instance ?? (_instance = factory(resolver));

			// Need to be thread safe
			if (_instance == null)
			{
				lock (_lock)
				{
					// If still null when lock is obtained
					if (_instance == null)
					{
						_instance = registration.CreateInstance();
					}
				}
			}

			return _instance;
		}

		public void Dispose()
		{
			// Only handle disposing of dynamo ioc related references
			// If instance registered uses unmanaged resources and should be disposed the user should implement a finalizer

			// Or dispose ?
			//var disposable = _instance as IDisposable;
			//if (disposable != null)
			//{
			//	disposable.Dispose();
			//	_instance = null;
			//}
		}
	}
}