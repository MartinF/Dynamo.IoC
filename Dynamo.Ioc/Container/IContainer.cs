using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dynamo.Ioc.Index;

// Include IDisposable or not ? No managed resources currently (or what about the ThreadLifetime ?)
// But could be included incase needed in the future ?

namespace Dynamo.Ioc
{
	public interface IContainer : IResolver, IDisposable
	{
		// Properties
		IIndex Index { get; }
		ILifetimeInfo DefaultLifetime { get; }
		CompileMode DefaultCompileMode { get; }

		// Methods
		IConfigurableRegistration Register(Type type, Expression<Func<IResolver, object>> expression);
		IConfigurableRegistration Register(Type type, object key, Expression<Func<IResolver, object>> expression);

		IRegistration RegisterInstance(Type type, object instance);
		IRegistration RegisterInstance(Type type, object key, object instance);

		void Compile();			// CompileException ?

		void Verify();			// VerifyException ?
		//bool TryVerify() ?
	}
}
