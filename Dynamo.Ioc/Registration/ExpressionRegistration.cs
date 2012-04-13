using System;
using System.Linq.Expressions;

// Create IInstanceFactory interface with CreateInstance(IResolver) method instead of injecting Func<IResolver, object> directly into the GetInstance() method on ILifetime?



// Registration also needs to be disposable - etc InstanceRegistration could have instance using unmanaged resources that needs to be disposed ?

// What about writing a generic wrapper for any kind of object ? - the object should implement a desctructor that automatically calls Dispose on the object ?



// Use generic parameter
// Also make generic IRegistration interface which can be returned by the Register method and automatically used when using Resolve(IRegistration<T>) etc ?
	// !!! - IRegistration could have generic implementation of GetInstance() - T GetInstance(IResolver) ?
	// OBS.: Should be covariant then because the <T> value should not be the type registered but the type it should use for the lookup ?
	// But not really needed ? using the the implementation Type as T will just give the exact type and not an interface etc !?

// Make lifetime optional in constructor and let it automatically use TransientLifetime if null etc ?

// Fix problem where Func<IResolver, object> factory always return an object - if generic it doesnt make sense to return anything else than the T
// Requires fix of Expression<Func<IResolver, object>> to also include the T
// The problem bascially comes from when the Expression is compiled by the compiler which currently doesnt preserve the original T but only works with object ?



// Wrap the ExpressionCompiler - So there is a Worker/Visitior and a Compiler which is just a wrapper around it where a cache for the current compiling could be implemented etc ?



// Doesnt need to be generic!? - only there to enforce the constraints

namespace Dynamo.Ioc
{
	public class ExpressionRegistration<T> : IExpressionRegistration, ICompilableRegistration
	{
		#region Fields
		private readonly Type _returnType;
		private readonly Type _implementationType;
		private readonly Expression<Func<IResolver, object>> _expression;
		private Func<IResolver, object> _factory;
		private ILifetime _lifetime;
		private CompileMode _compileMode;	
		#endregion

		#region Constructors
		public ExpressionRegistration(Expression<Func<IResolver, T>> expression, ILifetime lifetime, CompileMode compileMode = CompileMode.Delegate)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (lifetime == null)
				throw new ArgumentNullException("lifetime");

			_expression = expression.Convert();
			
			_returnType = typeof(T);
			_implementationType = expression.Body.Type;	// _returnType.IsValueType ? ((UnaryExpression)expression.Body).Operand.Type : expression.Body.Type; // Stores the actual type which is returned and not the Type (interface etc) it is registered for in the index 

			SetCompileMode(compileMode);
			SetFactory(_expression);
			SetLifetime(lifetime);
		}
		#endregion

		#region Properties
		public Type ReturnType { get { return _returnType; } }
		public Type ImplementationType { get { return _implementationType; } }
		public CompileMode CompileMode { get { return _compileMode; } }	
		public Expression<Func<IResolver, object>> Expression { get { return _expression; } }
		public ILifetime Lifetime { get { return _lifetime; } }
		#endregion

		#region Methods
		private void SetFactory(Expression<Func<IResolver, object>> expression)
		{
			switch (_compileMode)
			{
				case CompileMode.Dynamic:
					_factory = expression.CompileDynamic();
					break;
				case CompileMode.Delegate:
					_factory = expression.Compile();
					break;
				default:
					throw new NotSupportedException("CompileMode: " + _compileMode + " is not supported");	// NotImplementedException instead ?
			}
		}
		
		public void Compile(IIocContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			var compiler = new ExpressionCompiler(container);
			var compiledExpression = compiler.Compile(this);

			if (compiledExpression != _expression)
			{
				SetFactory(compiledExpression);
			}
		}

		public IExpressionRegistration SetLifetime(ILifetime lifetime)
		{
			// Currently allows changing lifetime no matter if compiled or not and/or used in other compiled registrations
			// The change wont be reflected if changed after it have been compiled (and not re-compiled) which is a problem.
			// Throw an exception ? or just  dont care ? or recompile (if already compiled - requires all registrations to be recompiled as they might refer to this registration)
			
			if (lifetime == null)
				throw new ArgumentNullException("lifetime");

			// Init new lifetime
			//lifetime.Init(this);

			// If _lifetime is already set dispose it first ? or call Clear/Reset() if supported ?
			//if (_lifetime != null)
			//   _lifetime.Dispose();

			_lifetime = lifetime;

			return this;
		}

		public IExpressionRegistration SetCompileMode(CompileMode compileMode)
		{
			if (!Enum.IsDefined(typeof(CompileMode), compileMode))
				throw new ArgumentException("Invalid CompileMode value");

			_compileMode = compileMode;

			return this;
		}

		public object GetInstance(IResolver resolver)
		{
			return _lifetime.GetInstance(_factory, resolver);
		}
		#endregion
	}
}