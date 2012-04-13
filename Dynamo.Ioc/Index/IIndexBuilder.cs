
// IIndex - Writer / Creator / Builder ?

namespace Dynamo.Ioc.Index
{
	public interface IIndexBuilder
	{
		void Add(IRegistration registration);
		void Add(IRegistration registration, object key);

		// bool TryAdd(IRegistration<T> registration, object key = null); ? - just adds if it not already contains registration

		// void Add(IRegistration registration, Type type, object key = null);
		// bool TryAdd(IRegistration registration, Type type, object key = null);
	}
}