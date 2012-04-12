using System;
using System.Web;
using System.Web.WebPages;

// Could Remove the dependency on System.Web.WebPages by creating a module to handle the disposing, and just include it using:
// [assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]
// public static void Start() { Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(MyModule)); }

namespace Dynamo.Ioc.Web
{
	public sealed class RequestLifetime : HttpContextAwareLifetimeBase
	{
		#region Fields
		private readonly object _key = new object();
		private readonly bool _disposeOnEnd;
		#endregion

		#region Constructors
		public RequestLifetime(bool disposeOnEnd = false) : base()
		{
			_disposeOnEnd = disposeOnEnd;
		}
		internal RequestLifetime(Func<HttpContextBase> func, bool disposeOnEnd = false) : base(func)
		{
			// Used for testing only
			_disposeOnEnd = disposeOnEnd;
		}
		#endregion

		#region Methods
		public override object GetInstance(Func<IResolver, object> factory, IResolver resolver)
		{
			var context = Context;

			var instance = context.Items[_key];
			if (instance == null)
			{
				instance = factory(resolver);
				context.Items[_key] = instance;

				if (_disposeOnEnd)
					context.RegisterForDispose((IDisposable)instance);	// throws exception to make you aware that it doesnt implement IDisposable as expected 

				// But should throw different Exception than InvalidCastException ? Fix test ?
				// Could make the check in Init instead ?
			}

			return instance;
		}
		#endregion
	}
}