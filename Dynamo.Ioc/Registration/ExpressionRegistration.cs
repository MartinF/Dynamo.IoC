using System;
using System.Linq.Expressions;

// Doesnt need to be generic - only there to enforce the constraints

namespace Dynamo.Ioc
{
	public class ExpressionRegistration<T> : LifetimeRegistrationBase<T>, IExpressionRegistration
	{
		#region Fields
		private Expression<Func<IResolver, object>> _expression;
		private Func<IResolver, object> _factory;
		private CompileMode _compileMode;
		private readonly IResolver _resolver;
		#endregion

		#region Constructors	
		public ExpressionRegistration(IResolver resolver, Expression<Func<IResolver, T>> expression, ILifetime lifetime, CompileMode compileMode = CompileMode.Delegate, object key = null)
			: base(lifetime: lifetime, key: key)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			if (expression == null)
				throw new ArgumentNullException("expression");

			_resolver = resolver;
			CompileMode = compileMode;
			Expression = expression.Convert();
		}
		#endregion

		#region Properties
		public CompileMode CompileMode
		{
			get { return _compileMode; }
			set
			{
				if (!Enum.IsDefined(typeof(CompileMode), value))
					throw new ArgumentException("Invalid CompileMode value");

				_compileMode = value;
			}
		}

		public Expression<Func<IResolver, object>> Expression
		{
			get { return _expression; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("expression");
				
				switch (_compileMode)
				{
					case CompileMode.Dynamic:
						_factory = value.CompileDynamic();
						break;
					case CompileMode.Delegate:
						_factory = value.Compile();
						break;
					default:
						throw new NotSupportedException("CompileMode: " + _compileMode + " is not supported");
				}

				_expression = value;
			}
		}

		public IResolver Resolver
		{
			get { return _resolver; }
		}
		#endregion

		#region Methods
		public override object GetInstance()
		{
			return _lifetime.GetInstance(this);
		}

		public override object CreateInstance()
		{
			return _factory(_resolver);
		}
		#endregion
	}
}