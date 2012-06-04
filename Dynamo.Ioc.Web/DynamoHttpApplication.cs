using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

// Dispose container at Application_End()

// Any way to run clean/clear on lifetime after Application_Start() have run ? - as there is no Session / Request context available
//[assembly: WebActivator.PostApplicationStartMethod(typeof(Rmote.Web.App_Start.MiniProfilerPackage), "PostStart")]

// Rename to DynamoIoCHttpApplication ?

namespace Dynamo.Ioc.Web
{
	public abstract class DynamoHttpApplication : HttpApplication
	{
		// Must be static to be shared accross all HttpApplication instances created
		protected static DynamoDependencyResolver DependencyResolver { get; private set; }
		
		// Methods
		protected virtual void Application_Start()
		{
			var container = new IocContainer();
			var depencenyResolver = new DynamoDependencyResolver(container);
			DependencyResolver = depencenyResolver;

			RegisterMvcDependencyResolver(depencenyResolver);
			RegisterWebApiDependencyResolver(depencenyResolver);

			RegisterModelValidators(depencenyResolver);

			RegisterDependencies(container);
			
			AreaRegistration.RegisterAllAreas(container);
		}

		protected virtual void RegisterMvcDependencyResolver(System.Web.Mvc.IDependencyResolver resolver)
		{
			// Mvc
			System.Web.Mvc.DependencyResolver.SetResolver(resolver);
		}

		protected virtual void RegisterWebApiDependencyResolver(System.Web.Http.Dependencies.IDependencyResolver resolver)
		{
			// Web Api
			GlobalConfiguration.Configuration.DependencyResolver = resolver;
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
