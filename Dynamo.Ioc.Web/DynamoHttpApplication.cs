using System;
using System.Web;
using System.Web.Mvc;

// Dispose container at Application_End() ?

// Any way to run clean/clear on lifetime after Application_Start() have run ?
//[assembly: WebActivator.PostApplicationStartMethod(typeof(Rmote.Web.App_Start.MiniProfilerPackage), "PostStart")]

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
			RegisterDependencies(_container);
			AreaRegistration.RegisterAllAreas(IocContainer);

			var depencenyResolver = new DynamoDependencyResolver(_container);

			RegisterDependencyResolver(depencenyResolver);
			RegisterModelValidators(depencenyResolver);
		}

		protected virtual void RegisterDependencyResolver(IDependencyResolver resolver)
		{
			DependencyResolver.SetResolver(resolver);
		}

		protected virtual void RegisterModelValidators(IServiceProvider provider)
		{
			// Register Custom Model Validators that enables exposing the IOC Container as IServiceProvider through ValidationContext.GetService() (Validation Attribute etc)
			DataAnnotationsModelValidatorProvider.RegisterDefaultAdapterFactory((metadata, context, attribute) => new DynamoDataAnnotationsModelValidator(provider, metadata, context, attribute));
			DataAnnotationsModelValidatorProvider.RegisterDefaultValidatableObjectAdapterFactory((metadata, context) => new DynamoValidatableObjectAdapter(provider, metadata, context));
		}

		protected abstract void RegisterDependencies(IIocContainer container);
	}
}
