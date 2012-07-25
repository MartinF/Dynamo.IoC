using System;
using System.Collections.Generic;
using Dynamo.Ioc.Fluent;
using Dynamo.Ioc.Index;

// Rename to IDependencyResolver ?

// Should IResolver implement IEnumerable<object> so all instances can be resolved no matter type/key ? Or Add ResolveAll() method?

namespace Dynamo.Ioc
{
	public interface IResolver : IFluentInterface
	{
		// Properties
		IIndexReader Index { get; }

		// Methods
		object Resolve(Type type);
		object Resolve(Type type, object key);
		T Resolve<T>();
		T Resolve<T>(object key);

		bool TryResolve(Type type, out object instance);
		bool TryResolve(Type type, object key, out object instance);
		bool TryResolve<T>(out T instance);
		bool TryResolve<T>(object key, out T instance);

		IEnumerable<object> ResolveAll(Type type);
		IEnumerable<T> ResolveAll<T>();

		IEnumerable<object> TryResolveAll(Type type);
		IEnumerable<T> TryResolveAll<T>();
	}
}