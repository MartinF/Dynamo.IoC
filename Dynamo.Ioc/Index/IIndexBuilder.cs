
namespace Dynamo.Ioc.Index
{
	public interface IIndexBuilder
	{
		void Add(IRegistration registration);

		// bool TryAdd(IRegistration registration, object key = null);
		// void Set(IRegistration registration, object key = null); - always adds/sets no matter if one already exists?
		// void Remove ?
	}
}