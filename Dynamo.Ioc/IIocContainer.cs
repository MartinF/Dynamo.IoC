using System;
using System.Linq.Expressions;
using System.Reflection;
using Dynamo.Ioc.Index;

// In register - should Key come before Lifetime ? if yes should key also be before lifetime in constructor of Registrations ? currently it is not

namespace Dynamo.Ioc
{
	public interface IIocContainer : IResolver, IDisposable
	{
		// Properties
		new IIndex Index { get; }
		CompileMode DefaultCompileMode { get; }
		Func<ILifetime> DefaultLifetimeFactory { get; }

		// Methods
		IExpressionRegistration Register<T>(Expression<Func<IResolver, T>> expression, object key = null, ILifetime lifetime = null, CompileMode? compileMode = null);
		
		IExpressionRegistration Register(Type type, Type implType, object key = null, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null);
		IExpressionRegistration Register<TType, TImpl>(object key = null, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType;

		// Need to split non-generic RegisterInstance into 2 methods else it will hit the generic version when not intended
		IRegistration RegisterInstance(Type type, object instance);
		IRegistration RegisterInstance(Type type, object instance, object key);
		IRegistration RegisterInstance<T>(T instance, object key = null);

		void Compile();

		void Verify();
	}
}
