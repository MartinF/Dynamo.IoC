using Dynamo.Ioc.Index;

// ICompilableRegistration instead ?

namespace Dynamo.Ioc
{
	public interface ICompilable
	{
		void Compile(IIndexAccessor index);
	}
}