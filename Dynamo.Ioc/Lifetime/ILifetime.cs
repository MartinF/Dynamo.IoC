using System;

namespace Dynamo.Ioc
{
	public interface ILifetime : IDisposable
	{
		// Methods
		object GetInstance(IInstanceFactoryRegistration registration);

		//bool Verify(IInstanceFactoryRegistration registration);
		
		//void Clear();	// Reset

		//void Init(IInstanceFactoryRegistration registration);
	}
}
