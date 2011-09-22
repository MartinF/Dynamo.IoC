using System;
using System.Collections.Generic;

namespace Dynamo.Ioc
{
	public interface IResolver : IFluentInterface
	{
		object Resolve(Type type);
		object Resolve(Type type, object key);
		object Resolve(IRegistrationInfo info);
		T Resolve<T>();
		T Resolve<T>(object key);

		bool TryResolve(Type type, out object obj);
		bool TryResolve(Type type, object key, out object instance);
		bool TryResolve(IRegistrationInfo info, out object instance);
		bool TryResolve<T>(out T instance);
		bool TryResolve<T>(object key, out T instance);

		IEnumerable<object> ResolveAll(Type type);
		IEnumerable<T> ResolveAll<T>();

		IEnumerable<object> TryResolveAll(Type type);
		IEnumerable<T> TryResolveAll<T>();
	}
}