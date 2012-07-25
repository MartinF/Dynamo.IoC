using System;
using System.Collections.Generic;

// Dynamo Dependency Resolver Scope ?

namespace Dynamo.Ioc.Web
{
	public class DynamoDependencyScope : System.Web.Http.Dependencies.IDependencyScope
	{
		private IResolverScope _scope;

		public DynamoDependencyScope(IResolverScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException("scope");

			_scope = scope;
		}

		public object GetService(Type serviceType)
		{
			object obj;
			_scope.TryResolve(serviceType, out obj);
			return obj;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _scope.TryResolveAll(serviceType);
		}

		public void Dispose()
		{
			// Thread safe?

			if (_scope != null)
			{
				_scope.Dispose();
				_scope = null;
			}
		}
	}
}
