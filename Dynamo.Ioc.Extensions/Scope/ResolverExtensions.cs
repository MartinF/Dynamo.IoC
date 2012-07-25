
// Get / Begin / Create - (New) - Scope ?

namespace Dynamo.Ioc
{
	public static partial class ResolverExtensions
	{
		public static IResolverScope GetScope(this IResolver resolver)
		{
			return new ResolverScope(resolver);
		}
	}
}
