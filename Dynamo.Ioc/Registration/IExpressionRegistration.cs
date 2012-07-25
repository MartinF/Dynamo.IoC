using System;
using System.Linq.Expressions;

namespace Dynamo.Ioc
{
	public interface IExpressionRegistration : ILifetimeRegistration
	{
		// Properties
		CompileMode CompileMode { get; set; }
		Expression<Func<IResolver, object>> Expression { get; set; }
		IResolver Resolver { get; }
		
		// bool IsCompiled? - can be set automatically when Expression is changed?
	}
}