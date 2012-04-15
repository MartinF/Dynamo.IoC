using System;

// Container / Singleton - only created the first time the GetInstance is called - cached for the rest of the time.

namespace Dynamo.Ioc
{
	public sealed class ContainerLifetime : ILifetime
	{
		private object _instance;
		
		public object GetInstance(Func<IResolver, object> factory, IResolver resolver)
		{
			return _instance ?? (_instance = factory(resolver));
		}

		//public override void Dispose()
		//{
		//   var disposable = _instance as IDisposable;
		//   if (disposable != null)
		//      disposable.Dispose();

		//   _instance = null;
		//}
	}
}