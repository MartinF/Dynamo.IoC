using System;

namespace Dynamo.Ioc
{
	public interface ILifetime
	{
		// Methods

		// void Init(IRegistration registration);
		// void Reset/Clear/Clean/Cleanup ?
		
		// IDisposable ? - so _instance or resources used can be disposed ? else you would have to write a wrapper for each object used if it uses managed resources ?

		object GetInstance(Func<IResolver, object> factory, IResolver resolver);
	}
}