using System;
using System.Web;
using System.Web.WebPages;

// Session is thread-safe as thread will get exlusive access or how does it work ? or do i need to lock manually?
// According to stackoverflow - no...

namespace Dynamo.Ioc.Web
{
	public static class ResolverExtensions
	{
		private static readonly object _requestKey = new object();
		private static readonly string _sessionKey = Guid.NewGuid().ToString();



		public static IResolverScope GetRequestScope(this IResolver resolver, HttpContextBase context = null)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			if (context == null)
				context = new HttpContextWrapper(HttpContext.Current);

			var item = context.Items[_requestKey];

			if (item == null)
			{
				lock (context.Items.SyncRoot)	// Lock on different object only being used to get the Scope to minimize locking?
				{
					item = context.Items[_requestKey];
					if (item == null)
					{
						var scope = resolver.GetScope();
						context.Items[_requestKey] = scope;

						// Register for automatica disposal at end of request
						context.RegisterForDispose(scope);
						
						return scope;
					}
				}
			}

			return (IResolverScope)item;
		}

		public static IResolverScope GetSessionScope(this IResolver resolver, HttpContextBase context = null)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			if (context == null)
				context = new HttpContextWrapper(HttpContext.Current);

			// Try to get already existing item
			// Let it throw an null pointer exception if context.Session is used in a non web/Session context.
			var item = context.Session[_sessionKey];

			if (item == null)
			{
				var scope = resolver.GetScope();
				context.Session[_sessionKey] = scope;
				return scope;
			}

			return (IResolverScope)item;
		}
	}
}
