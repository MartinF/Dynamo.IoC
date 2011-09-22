using System;

namespace Dynamo.Ioc
{
	public class ServiceProviderAdapter : IServiceProvider
	{
		private readonly IResolver _resolver;

		public ServiceProviderAdapter(IResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			_resolver = resolver;
		}

		public object GetService(Type serviceType)
		{
			object obj;
			_resolver.TryResolve(serviceType, out obj);
			return obj;
		}
	}
}
