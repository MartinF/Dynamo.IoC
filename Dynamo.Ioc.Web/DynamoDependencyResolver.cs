using System;
using System.Collections.Generic;

// http://bradwilson.typepad.com/blog/2010/10/service-location-pt5-idependencyresolver.html

// Adds support for both Mvc and Web Api

namespace Dynamo.Ioc.Web
{
	public class DynamoDependencyResolver : IServiceProvider, System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
	{
		private readonly IIocContainer _container;

		public DynamoDependencyResolver(IIocContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			
			_container = container;
		}

		public object GetService(Type serviceType)
		{
			object obj;
			_container.TryResolve(serviceType, out obj);
			return obj;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _container.TryResolveAll(serviceType);
		}

		public System.Web.Http.Dependencies.IDependencyScope BeginScope()
		{
			return null;
		}

		public void Dispose()
		{
		}
	}
}