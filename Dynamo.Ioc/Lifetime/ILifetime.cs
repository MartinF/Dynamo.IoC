using System;

namespace Dynamo.Ioc
{
	public interface ILifetime
	{
		// Methods
		
		// If RequestLifetime doesnt use Init then dont use it at all 
		void Init(IRegistration registration);										// Just include IExpressionRegistration / ExpressionRegistration ?
		
		object GetInstance(Func<IResolver, object> factory, IResolver resolver);
		
		// void Reset/Clear/Clean/Cleanup ?

		// IDisposable ? - so _instance or resources used can be disposed ? else you would have to write a wrapper for each object used if it uses managed resources ?
	}
}