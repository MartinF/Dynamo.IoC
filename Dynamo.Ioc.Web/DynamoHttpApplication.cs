using System.Web;
using System.Web.Mvc;

// Expose container at Application_End() ?
// Do not have IocContainer as static public property ? - make member of instance instead ?

namespace Dynamo.Ioc.Web
{
	public abstract class DynamoHttpApplication : HttpApplication
	{
		// Fields
		private static readonly IIocContainer _container = new IocContainer();
		
		// Properties
		public static IResolver IocContainer { get { return _container; } }

		// Methods
		protected virtual void Application_Start()
		{
			RegisterServices(_container);
			AreaRegistration.RegisterAllAreas(IocContainer);

			RegisterDependencyResolver(new DynamoDependencyResolver(_container));
		}

		protected virtual void RegisterDependencyResolver(IDependencyResolver resolver)
		{
			DependencyResolver.SetResolver(resolver);
		}

		protected abstract void RegisterServices(IIocContainer container);
	}
}
