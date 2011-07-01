
// Rename to IExpressionRegistration ?

namespace Dynamo.Ioc
{
	public interface IConfigurableRegistration : IRegistrationInfo, IFluentInterface
	{
		ILifetimeInfo Lifetime { get; }		// GetLifetime() ?

		IConfigurableRegistration SetLifetime(ILifetime lifetime);
	}
}