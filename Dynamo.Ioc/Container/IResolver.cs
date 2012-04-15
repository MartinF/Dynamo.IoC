using System;
using System.Collections.Generic;

// Rename to IIocResolver ?

// LazyResolve?

namespace Dynamo.Ioc
{
	public interface IResolver : IFluentInterface
	{
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

		// Could add the following methods
		// But requires additional work to the compiler, because it needs to distinguish between methods signatures (Resolve(Type) and Resolve(IRegistration))
		// etc. for generic one it would be something like - if (arguments.Count == 1 && (!arguments[0].Type.IsImplementationOfGenericInterface(typeof(IRegistration<>)))) // T Resolve<T>(object key)
 
		//object Resolve(IRegistration registration);
		//T Resolve<T>(IRegistration<T> registration);

		// bool TryResolve(IRegistration, out object instance)
		// bool TryResolve<T>(IRegistration<T>, out T instance) ? 
			// Problem is that i cant be sure the IRegistration<T> is from this index 
			// TryResolve implies that it wont throw an exception if it is not ?
			// Implement Try/Catch inside ?
			// When Trying to Resolve it it might have something like x => new Foo(x.Resolve<Bar>()) inside it which will throw an exception if it cant resolve Bar
	}
}