
namespace Dynamo.Ioc
{
	public interface ILifetimeRegistration : IInstanceFactoryRegistration
	{
		// Properties
		ILifetime Lifetime { get; set; }
	}
}