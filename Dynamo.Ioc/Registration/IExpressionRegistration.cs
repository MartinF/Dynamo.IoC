using System;
using System.Linq.Expressions;

// Add IsCompiled Property ?
// Keep ICompilableRegistration interface ? on the IExpressionRegistration interface or put on the implementation ExpressionRegistration ?

// Remove generic interfaces / implementations again !? doesnt really add much other than a generic GetInstance method!?
// Could just return the actual implementation type (ExpressionRegistration) instead of the IExpressionRegistration interface

namespace Dynamo.Ioc
{
	public interface IExpressionRegistration : IRegistration, ICompilableRegistration, IFluentInterface
	{
		// Properties
		CompileMode CompileMode { get; }
		ILifetime Lifetime { get; }
		Expression<Func<IResolver, object>> Expression { get; }

		// Methods
		IExpressionRegistration SetLifetime(ILifetime lifetime);
		IExpressionRegistration SetCompileMode(CompileMode compileMode);
	}

	public interface IExpressionRegistration<out T> : IExpressionRegistration, IRegistration<T>
	{
		// Methods
		new IExpressionRegistration<T> SetLifetime(ILifetime lifetime);
		new IExpressionRegistration<T> SetCompileMode(CompileMode compileMode);
	}
}