using System;

namespace Dynamo.Ioc
{
	public interface IRegistration
	{
		// Properties
		Type ImplementationType { get; }
		Type ReturnType { get; }

		// Methods
		object GetInstance(IResolver resolver);
	}
}