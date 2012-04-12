
// IIndex - Writer / Creator / Builder ?

namespace Dynamo.Ioc.Index
{
	public interface IIndexBuilder
	{
		// AddWithKey
		// Add

		void Add<T>(IRegistration<T> registration, object key = null);
		// bool TryAdd(IRegistration<T> registration, object key = null); ? - just adds if it not already contains registration

		// void Add(IRegistration registration, Type type, object key = null);
		// bool TryAdd(IRegistration registration, Type type, object key = null);
	}
}