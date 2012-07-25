using System;

namespace Dynamo.Ioc
{
	public static class ResolverExtensions
	{
		public static IServiceProvider GetServiceProvider(this IResolver resolver)
		{
			return new ServiceProviderAdapter(resolver);
		}
	}
}
