
namespace Dynamo.Ioc.Index
{
	public interface IIndexBuilder
	{
		void Add(IRegistration registration, object key = null);

		// bool TryAdd(IRegistration registration, object key = null);
	}
}