using System;
using System.Web;
using System.Web.WebPages;

// Could Remove the dependency on System.Web.WebPages by creating a module to handle the disposing, and just include it using:
// [assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]
// public static void Start() { Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(MyModule)); }

namespace Dynamo.Ioc
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
		public override object GetInstance(IInstanceFactory factory, IResolver resolver)
		{
			var context = Context;

			var instance = context.Items[_key];
			if (instance == null)
			{
				instance = factory.CreateInstance(resolver);
				context.Items[_key] = instance;

				if (_disposeOnEnd)
					context.RegisterForDispose((IDisposable)instance);	// throws exception to make you aware that it doesnt implement IDisposable as expected 
																		// But should throw different Exception than InvalidCastException ? Fix test ?
																		// Could make the check in Init instead ?
			}

			return instance;
		}

		public override void Init(IRegistrationInfo key)
		{
			base.Init(key);
			
			// Could check here if Type implements IDisposable ? 
			// Will it work checking the actual type ? 
			//if (_disposeOnEnd && !(key.Type is IDisposable))
			//{
			//    throw new DynamoRegistrationException(key, "To use the Dispose On End feature the instance registered needs to implement IDisposable");
			//}
		}
		#endregion
	}
}