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

		

		// automatically try to dispose lifetimes ? - implement a desctructor in every lifetime ?
		// I never know what kind of _instance will be stored - maybe it will need to be disposed when GC kicks in
		// but if that is the case the object is managed and that object it self could just implement a desctructor

		//public override void Dispose()
		//{
		//   var disposable = _instance as IDisposable;
		//   if (disposable != null)
		//      disposable.Dispose();

		//   _instance = null;
		//}
	}
}