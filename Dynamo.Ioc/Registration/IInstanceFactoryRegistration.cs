
namespace Dynamo.Ioc
{
	public interface IInstanceFactoryRegistration : IRegistration
	{
		object CreateInstance();
	}
}
