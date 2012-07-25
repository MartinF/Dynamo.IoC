using System;
using System.Web;
using System.Web.WebPages;

// Could Remove the dependency on System.Web.WebPages by creating a module to handle the disposing, and just include it using:
// [assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]
// public static void Start() { Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(MyModule)); }

// Needs to be thread-safe?

namespace Dynamo.Ioc.Web
{
	public class RequestLifetime : ILifetime
	{
		#region Fields
		private readonly object _key = new object();
		private readonly Func<HttpContextBase> _func;
		private readonly bool _disposeOnEnd;
		private readonly object _lock = new object();
		#endregion

		#region Constructors
		public RequestLifetime(bool disposeOnEnd = false) : this(() => new HttpContextWrapper(HttpContext.Current), disposeOnEnd)
		{
		}
		public RequestLifetime(Func<HttpContextBase> func, bool disposeOnEnd = false)
		{
			if (func == null)
				throw new ArgumentNullException("func");

			_func = func;
			_disposeOnEnd = disposeOnEnd;
		}
		#endregion

		#region Properties
		protected HttpContextBase Context { get { return _func(); } }	
		#endregion

		#region Methods
		public object GetInstance(IInstanceFactoryRegistration registration)
		{
			var context = Context;
			var instance = context.Items[_key];

			if (instance == null)
			{
				lock (_lock)
				{
					instance = context.Items[_key];
					if (instance == null)
					{
						instance = registration.CreateInstance();
						context.Items[_key] = instance;

						if (_disposeOnEnd)
							context.RegisterForDispose((IDisposable)instance);	// throws exception to make you aware that it doesnt implement IDisposable as expected
							// But should throw different Exception than InvalidCastException !?
							// Could make Initialize method which gets IRegistration where i can be checked?
					}
				}
			}

			return instance;
		}

		public void Dispose()
		{
			// Only handle disposing of dynamo ioc related references
			// If instance registered uses unmanaged resources and should be disposed the user should implement a finalizer
		}
		#endregion
	}
}