using System.Web;
using System.Web.Mvc;

namespace Dynamo.Ioc.Web
{
	public abstract class DynamoHttpApplication : HttpApplication
	{
		protected DynamoHttpApplication()
		{
			IocContainer = new Container();
		}

		public IContainer IocContainer { get; private set; }

		protected abstract void RegisterServices(IContainer container);

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas(IocContainer);
			RegisterServices(IocContainer);

			RegisterDependencyResolver(new DynamoDependencyResolver(IocContainer));

			OnStart();
		}
		protected void Application_End()
		{
			OnEnd();

			IocContainer.Dispose();
			IocContainer = null;
		}
		
		protected virtual void RegisterDependencyResolver(IDependencyResolver resolver)
		{
			DependencyResolver.SetResolver(resolver);
		}

		/// <summary>
		/// Application_Start() Event
		/// </summary>
		protected virtual void OnStart()
		{
		}

		/// <summary>
		/// Application_End() Event
		/// </summary>
		protected virtual void OnEnd()
		{
		}
	}
}
