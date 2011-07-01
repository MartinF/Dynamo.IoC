using System;
using System.Collections.Generic;

// Simplify this Interface ?

// The reason why it have methods for all supported "Resolve methods" in the Container
// is so it can implement the best / fastest possible solution for each.

// Contains is not really needed (could just use linq) - remove and only focus on performance when querying (get methods) ?

// Keep both GetAll and TryGetAll ? Difference is that GetAll expects to find the type else throws exception like Get() and TryGetAll just returns empty enumerable
// If TryGetAll should use normal TryGetXxx pattern it cant use yield return and execute lazily

namespace Dynamo.Ioc.Index
{
	public interface IIndexAccessor : IEnumerable<IRegistration>
	{
		IRegistration Get(Type type);
		IRegistration Get(Type type, object key);
		bool TryGet(Type type, out IRegistration registration);
		bool TryGet(Type type, object key, out IRegistration registration);

		IEnumerable<IRegistration> GetAll(Type type);
		IEnumerable<IRegistration> TryGetAll(Type type);

		// Better names - that make the difference between Contains and ContainsAny more clear ?
		bool Contains(Type type);
		bool Contains(Type type, object key);
		bool ContainsAny(Type type);

		// Count?
		// IsEmpty ?
		// But both can be determined via Linq index.any()/count() though they are a little slower ? 
		// But doesnt matter - only focus on performance for the Querying the Index ?
	}
}