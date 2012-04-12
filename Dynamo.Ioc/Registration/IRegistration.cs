using System;

// Remove Generic IRegistration<T> interface again ? dont really add much!

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

	public interface IRegistration<out T> :	IRegistration
	{
		// Methods
		new T GetInstance(IResolver resolver);
	}
}