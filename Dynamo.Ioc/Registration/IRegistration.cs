using System;

// Should IRegistration implement IDisposable ?
// Doesnt need to be required - right now there is no unmananged resources (or what about ThreadLifetime ?)
// But someone might implement their own Registration type implementing IRegistration and would want the Index to call dispose when they are disposing the container/index ?
// What if some Instance registered using RegisterInstance need to be disposed ? -expect it to handle it itself using a deconstructor/finalizer ? 
	// But it is not always possible to do this is it (other than you have to make a wrapper) ? 
	// But on the other hand no resource should be open for the lifetime of the contaienr should it ? If something is resource extensive it should be opened when used and closed / disposed immediatly after
	// Maybe you dont want to instance to be disposed when the container is disposed ? - needs a way to configure that in that case

// The index could just loop all registrations and try cast all registrations to IDisposable and call Dispose if possible ?

namespace Dynamo.Ioc
{
	public interface IRegistration : IRegistrationInfo, IDisposable
	{
		object GetInstance(IResolver resolver);

		// void Reset(IIndexAccessor)
	}
}
