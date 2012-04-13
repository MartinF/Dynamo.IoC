using System;
using System.Linq.Expressions;

// Add IsCompiled Property ?
// Keep ICompilableRegistration interface ? on the IExpressionRegistration interface or put on the implementation ExpressionRegistration ?

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
}