
namespace Dynamo.Ioc
{
	public interface ICompilableRegistration
	{
		void Compile(IIocContainer container);	// IResolver should be enough ?
	}
}