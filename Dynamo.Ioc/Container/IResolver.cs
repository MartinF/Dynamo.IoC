using System;
using System.Collections.Generic;

// Rename to IIocResolver ?

namespace Dynamo.Ioc
{
	public interface IResolver : IFluentInterface
	{
		object Resolve(Type type);
		object Resolve(Type type, object key);
		T Resolve<T>();
		T Resolve<T>(object key);

		// Keep ?
		object Resolve(IRegistration registration);
		T Resolve<T>(IRegistration<T> info);

		bool TryResolve(Type type, out object instance);
		bool TryResolve(Type type, object key, out object instance);
		bool TryResolve<T>(out T instance);
		bool TryResolve<T>(object key, out T instance);

		// bool TryResolve(IRegistration, out object instance)
		// bool TryResolve<T>(IRegistration<T>, out T instance) ? 
			// Problem is that i cant be sure the IRegistration<T> is from this index 
			// TryResolve implies that it wont throw an exception if it is not ?
			// Implement Try/Catch inside ?
			// When Trying to Resolve it it might have something like x => new Foo(x.Resolve<Bar>()) inside it which will throw an exception if it cant resolve Bar

		IEnumerable<object> ResolveAll(Type type);
		IEnumerable<T> ResolveAll<T>();

		IEnumerable<object> TryResolveAll(Type type);
		IEnumerable<T> TryResolveAll<T>();
	}
}