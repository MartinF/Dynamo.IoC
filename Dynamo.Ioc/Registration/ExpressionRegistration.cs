using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dynamo.Ioc.Index;
using Dynamo.Ioc.Registration;

// *** Decide ? 
// Could remove the Lifetime and make an implementation of IRegistration for each lifetime instead to make it faster.
// Just make an base implementation and override the GetInstance method ?
// Then there should just be different Register methods or one that takes an Enum defining the Registration/Liftime type to create and add to the index.
// But how should the Registration/Lifetime be configured ? - needs a Register method for each type supported then, or come up with some seperate fluent like syntax/interface ...
// Instead of doing: container.Register<IFoo>(x => new Foo(x.Resolve<IBar>())).RequestLifetime()
// you could do something like: var reg = new RequestLifetimeRegistration<IFoo>(x => new Foo(x.Resolve<IBar>()), (configuration here?)); reg.DisposeOnEnd(); container.Register(reg); ? - not fluent, but will work.
// If people create their own Registration they can just add it directly to the index or create an extension for the Container doing it.

// Change IConfigurableRegistration to IExpressionRegistration ?

namespace Dynamo.Ioc
{
	public class ExpressionRegistration : IRegistration, IInstanceFactory, IConfigurableRegistration, IRegistrationInfo, ICompilable
	{
		#region Fields
		private readonly Type _type;
		private readonly object _key;
		private readonly CompileMode _compileMode;
		private readonly Expression<Func<IResolver, object>> _expression;
		private Func<IResolver, object> _factory;
		private ILifetime _lifetime;
		#endregion

		#region Constructors
		public ExpressionRegistration(Type type, Expression<Func<IResolver, object>> expression, ILifetime lifetime, object key = null, CompileMode compileMode = CompileMode.Dynamic)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (expression == null)
				throw new ArgumentNullException("expression");

			_type = type;
			_expression = expression;

			SetFactory(expression);
			SetLifetime(lifetime);

			_key = key;
			_compileMode = compileMode;
		}
		#endregion

		#region Properties
		public Type Type { get { return _type; } }
		public object Key { get { return _key; } }
		internal Expression<Func<IResolver, object>> Expression { get { return _expression; } }		// make public / protected ? expose via interface (IConfigurableRegistration/IExpressionRegistration) ?
		public ILifetimeInfo Lifetime { get { return _lifetime; } }									// What to do with the ILifetimeInfo interface ? Right now doesnt make sense, only used to test for type of lifetime really - reg.Lifetime is TransientLifetime etc 
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
					throw new ArgumentException("Unknown CompileMode", "expression");
			}
		}

		public IConfigurableRegistration SetLifetime(ILifetime lifetime)
		{
			// Currently allows changing lifetime no matter if compiled or not and/or used in other compiled registrations
			// The change wont be reflected if changed after it have been compiled (and not re-compiled) which is a problem.
			// Throw an exception ? or just  dont care ? or recompile (if already compiled - requires all registrations to be recompiled as they might refer to this registration)
			
			if (lifetime == null)
				throw new ArgumentNullException("lifetime");

			// Init new lifetime
			lifetime.Init(this);

			// If _lifetime is already set dispose it first ? or call Clear/Reset() if supported ?
			//if (_lifetime != null)
			//   _lifetime.Dispose();

			_lifetime = lifetime;

			return this;
		}

		public void Compile(IIndexAccessor index)
		{
			var compiler = new ExpressionCompiler(index);

			Expression<Func<IResolver, object>> compiledExpression; 
			if (compiler.TryCompile(_expression, out compiledExpression))
			{
				SetFactory(compiledExpression);

				// Store Compiled Expression so it can be reused later if referenced from another registration instead of compiling it again ?
				// _compiledExpression = compiledExpression;
			}
		}

		public object GetInstance(IResolver resolver)
		{
			return _lifetime.GetInstance(this, resolver);
		}

		public object CreateInstance(IResolver resolver)
		{
			return _factory(resolver);
		}

		public void Dispose()
		{
			// Remove ? 
			// Else try to dispose the Lifetime which could then handle the clean up
		}
		#endregion
	}
}