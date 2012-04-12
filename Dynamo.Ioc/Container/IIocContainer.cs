using System;
using System.Linq.Expressions;
using System.Reflection;
using Dynamo.Ioc.Index;

// Include IDisposable or not ? 
	// No managed resources in the Container currently (only the ThreadLocalLifetime - which is in the Index)
	// Could just put disposable on Index or use a Finalizer to clean it up ?

// Should key be the last parameter or after type parameter ?

// Let all Register methods take ILifetime as parameter - instead of using the default
// Or remove CompileMode and always use default and expect it to be set afterwards using SetCompileMode() etc ?

// Register Auto - use / include - InternalCtor ? - change so it includes the internal constructors when selecting ? and set default to false !

// Remove Generic IExpressionRegistration and implementations - doesnt really add much - only a Genreic GetInstance method very rarely used !?

namespace Dynamo.Ioc
{
	public interface IIocContainer : IResolver, IDisposable
	{
		// Properties
		IIndex Index { get; }						// Expose as IIndexReader - so it is not possible to directly modify it?
		CompileMode DefaultCompileMode { get; }
		Func<ILifetime> DefaultLifetimeFactory { get; }

		// Methods

			// Register
		IExpressionRegistration<T> Register<T>(Expression<Func<IResolver, T>> expression, ILifetime lifetime = null, CompileMode? compileMode = null);
		IExpressionRegistration<T> Register<T>(Expression<Func<IResolver, T>> expression, object key, ILifetime lifetime = null, CompileMode? compileMode = null);

			// Register Auto
		IExpressionRegistration Register(Type type, Type implType, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null);
		IExpressionRegistration Register(Type type, Type implType, object key, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null);
		IExpressionRegistration<TType> Register<TType, TImpl>(ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType;
		IExpressionRegistration<TType> Register<TType, TImpl>(object key, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType;

			// Register Instance
		IRegistration RegisterInstance(Type type, object instance);	
		IRegistration RegisterInstance(Type type, object instance, object key);
		IRegistration<T> RegisterInstance<T>(T instance);
		IRegistration<T> RegisterInstance<T>(T instance, object key);

		void Compile();
		// bool TryCompile()

		//void Verify();		// Removed until working like expected
		//bool TryVerify()
	}
}
