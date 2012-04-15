using System;
using System.Web;

// How to dispose object when session ends?
// Create a wrapper implementing an destructor that automatically calls dispose?
// but why not let the actual object implement it itself then?

namespace Dynamo.Ioc.Web
{
	public sealed class SessionLifetime : HttpContextAwareLifetimeBase
	{
		#region Fields
		private readonly string _key = Guid.NewGuid().ToString();
		#endregion

		#region Constructors
		public SessionLifetime() : base()
		{
		}
		internal SessionLifetime(Func<HttpContextBase> func) : base(func)
		{
			// Used for testing only
		}
		#endregion

		#region Methods
		public override object GetInstance(Func<IResolver, object> factory, IResolver resolver)
		{
			var session = Context.Session;

			// if run in Application_Start() Session will be null - just return new instance ?
			// throw exception instead and let it be known that you are trying tor resolve through SessionLifetime when Session doesnt exist ?
			// or create some temporary storage (thread storage) ?
			if (session == null)
				return factory(resolver);
				//throw new Exception("Xxx Type is registered using a SessionLifetime, but the session is not available at this point (maybe you are calling from a application startup) ... some good description");

			object instance = session[_key];

			if (instance == null)
			{
				instance = factory(resolver);
				session[_key] = instance;
			}

			return instance;
		}
		#endregion
	}
}