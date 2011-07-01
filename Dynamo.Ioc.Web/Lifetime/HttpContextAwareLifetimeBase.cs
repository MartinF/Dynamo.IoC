using System;
using System.Web;

namespace Dynamo.Ioc
{
	public abstract class HttpContextAwareLifetimeBase : LifetimeBase
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
	}
}
