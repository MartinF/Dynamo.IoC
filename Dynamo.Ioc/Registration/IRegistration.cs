using System;
using Dynamo.Ioc.Fluent;

namespace Dynamo.Ioc
{
	public interface IRegistration : IDisposable, IFluentInterface
	{
		// Properties
		Type ReturnType { get; }
		object Key { get; }

		// Methods
		object GetInstance();
		bool Verify();

		//void Clear();	// Reset
	}
}