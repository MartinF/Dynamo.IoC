using System;
using System.Web;

// Needs to be thread-safe or will Session automatically assign Exclusive access to the thread until finished?

namespace Dynamo.Ioc.Web
{
	public class SessionLifetime : ILifetime
	{
		#region Fields
		private readonly string _key = Guid.NewGuid().ToString();
		private readonly Func<HttpContextBase> _func;
		#endregion

		#region Constructors
		public SessionLifetime() : this(() => new HttpContextWrapper(HttpContext.Current))
		{
		}
		public SessionLifetime(Func<HttpContextBase> func)
		{
			if (func == null)
				throw new ArgumentNullException("func");

			_func = func;
		}
		#endregion

		#region Properties
		protected HttpContextBase Context { get { return _func(); } }
		#endregion

		#region Methods
		public object GetInstance(IInstanceFactoryRegistration registration)
		{
			var session = Context.Session;

			// if run in Application_Start() Session will be null - throw exception, return new instance or try to create temporary storeage (thread storage etc) ?
			if (session == null)
				return registration.CreateInstance();
				//throw new Exception("Type is registered using a SessionLifetime, but the session is not available at this point (maybe you are calling from application startup state)");

			object instance = session[_key];

			if (instance == null)
			{
				instance = registration.CreateInstance();
				session[_key] = instance;
			}

			return instance;
		}

		public void Dispose()
		{
			// Only handle disposing of dynamo ioc related references
			// If instance registered uses unmanaged resources and should be disposed the user should implement a finalizer

			// Or ?
			//var session = Context.Session;
			//if (session != null)
			//{
			//	var disposable = session[_key] as IDisposable;
			//	if (disposable != null)
			//		disposable.Dispose();

			//	session.Remove(_key);
			//}
		}
		#endregion
	}
}