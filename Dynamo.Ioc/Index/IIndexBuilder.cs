
namespace Dynamo.Ioc.Index
{
	public interface IIndexBuilder
	{
		void Add(IRegistration registration);

		// bool TryAdd(IRegistration registration); ?
	}
}