
namespace Dynamo.Ioc
{
	public interface ILifetime : ILifetimeInfo
	{
		void Init(IRegistrationInfo key);
		object GetInstance(IInstanceFactory factory, IResolver resolver);	// Inject both the IInstanceFactory and the IResolver (or IContainer) ?

		// void Reset/Clear() ?
	}
}