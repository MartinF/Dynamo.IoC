using System;
using System.Web;

namespace Dynamo.Ioc
{
	public sealed class SessionLifetime : HttpContextAwareLifetimeBase
	{
		#region Fields
		private string _key;
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
		public override void Init(IRegistrationInfo registration)
		{
			// TODO: Needs fixing - use Key.GetType().Name or Key.GetHashcode or something. Should be Unique - Use Guid ?

			// Create key
			_key = "#" + registration.Type.Name;	// Fullname ?
			if (registration.Key != null)
				_key += "-" + registration.Key;
		}
		public override object GetInstance(IInstanceFactory factory, IResolver resolver)
		{
			var session = Context.Session;

			// if run in Application_Start() Session will be null - just return new instance ?
			if (session == null)
				return factory.CreateInstance(resolver);

			object instance = session[_key];

			if (instance == null)
			{
				instance = factory.CreateInstance(resolver);
				session[_key] = instance;
			}

			return instance;
		}
		#endregion
	}
}