using System;
using System.Web;

namespace Dynamo.Ioc.Web
{
	public abstract class HttpContextAwareLifetimeBase : ILifetime
	{
		private readonly Func<HttpContextBase> _func;

		protected internal HttpContextAwareLifetimeBase(Func<HttpContextBase> func)
		{
			if (func == null)
				throw new ArgumentNullException("func");
			
			_func = func;
		}
		protected HttpContextAwareLifetimeBase() : this(() => new HttpContextWrapper(HttpContext.Current))	
		{
		}

		protected HttpContextBase Context { get { return _func(); } }

		public virtual void Init(IRegistration registration)
		{
		}
		
		public abstract object GetInstance(Func<IResolver, object> factory, IResolver resolver);
	}
}
