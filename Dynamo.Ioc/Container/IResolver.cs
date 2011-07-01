using System;
using System.Collections.Generic;

namespace Dynamo.Ioc
{
	public interface IResolver : IFluentInterface
	{
		object Resolve(Type type);
		object Resolve(Type type, object key);

		bool TryResolve(Type type, out object obj);
		bool TryResolve(Type type, object key, out object obj);

		IEnumerable<object> ResolveAll(Type type);
		IEnumerable<object> TryResolveAll(Type type);
	}
}
