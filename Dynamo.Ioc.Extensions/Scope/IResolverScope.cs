using System;

// Rename to IDependencyResolverScope ?

namespace Dynamo.Ioc
{
	public interface IResolverScope : IResolver, IDisposable
	{
	}
}
