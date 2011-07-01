
namespace Dynamo.Ioc
{
	public interface ILifetimeInfo
	{
		// TODO: Decide what to do ?

		// Include Type Property ? So GetType() dont need to be used ?
		// But you can just check the Lifetime Type by doing - reg.Lifetime is TransientLifetime ? but of course you dont get the actual type by doing that.

		// Every Lifetime gets IRegistrationInfo in Init 
		// So IRegistrationInfo could be exposed ?
	}
}