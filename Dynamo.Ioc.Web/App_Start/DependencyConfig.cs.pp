using System;
using Dynamo.Ioc;
using Dynamo.Ioc.Web;

// Could as-well use the built-in attribute if you don't care about the order
// [assembly: System.Web.PreApplicationStartMethodAttribute(typeof($rootnamespace$.App_Start.DependencyConfig), "PreStart")]

// When Order is not defined it defaults to -1, so -2 is used to make it run first
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.DependencyConfig), "PreStart", Order = -2)]
namespace $rootnamespace$.App_Start
{
	internal static class DependencyConfig
	{
		public static void RegisterDependencies(IIocContainer container)
		{
			// Register your dependencies here.
		}

		public static void RegisterMvcDependencyResolver(System.Web.Mvc.IDependencyResolver resolver)
		{
			// Mvc
			System.Web.Mvc.DependencyResolver.SetResolver(resolver);
		}

		public static void RegisterWebApiDependencyResolver(System.Web.Http.Dependencies.IDependencyResolver resolver)
		{
			// Web Api
			System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = resolver;
		}

		public static void RegisterModelValidators(IServiceProvider provider)
		{
			// Register Custom Model Validators that enables exposing the Container as IServiceProvider through ValidationContext.GetService() (Validation Attribute etc)
			System.Web.Mvc.DataAnnotationsModelValidatorProvider.RegisterDefaultAdapterFactory((metadata, context, attribute) => new DynamoDataAnnotationsModelValidator(provider, metadata, context, attribute));
			System.Web.Mvc.DataAnnotationsModelValidatorProvider.RegisterDefaultValidatableObjectAdapterFactory((metadata, context) => new DynamoValidatableObjectAdapter(provider, metadata, context));
		}

		public static void PreStart()
		{
			var container = new IocContainer();
			var resolver = new DynamoDependencyResolver(container);

			RegisterMvcDependencyResolver(resolver);
			RegisterWebApiDependencyResolver(resolver);
			RegisterModelValidators(resolver);

			RegisterDependencies(container);
		}
	}
}
