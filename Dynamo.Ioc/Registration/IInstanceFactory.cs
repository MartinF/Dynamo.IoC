
namespace Dynamo.Ioc
{
	public interface IInstanceFactory
	{
		object CreateInstance(IResolver resolver);
	}
}