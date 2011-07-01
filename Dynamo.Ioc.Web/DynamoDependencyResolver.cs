using System;
using System.Collections.Generic;
using System.Web.Mvc;

// http://bradwilson.typepad.com/blog/2010/10/service-location-pt5-idependencyresolver.html

namespace Dynamo.Ioc.Web
{
	public class DynamoDependencyResolver : IDependencyResolver
	{
		private readonly IContainer _container;

		public DynamoDependencyResolver(IContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			
			_container = container;
		}

		public object GetService(Type serviceType)
		{
			object obj;
			if (_container.TryResolve(serviceType, out obj))
				return obj;

			return null;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _container.TryResolveAll(serviceType);
		}
	}
}