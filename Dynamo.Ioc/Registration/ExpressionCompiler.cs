using System;
using System.Linq.Expressions;

// Add Support for TryResolve (Type/Key, and <T>) - if not found just inline null, or keep TryResolve expression.
// + Resolve(Registration) and Resolve(Registration<T>) ?

// Turn Generic method calls into the equvilant using type directly ? - very small improvement
// Add support for using cached compiled registration to speed it up instead of compiling the whole "tree" for each registration ?
// TryCompile - which doesnt throw exception if not able to compile / find a reference to a registration needed ?

// Better protection against infinite loop ?

// Wrap in Compiler which could take care of caching the Compiled Expressions to speed it up instead of compiling the full tree every single time.
// Just let the Compiler provide methods for getting the Expression from an registration - check if has already been compiled and return it instead etc.

// Fix Error message in GetRegistration

namespace Dynamo.Ioc
{
	public class ExpressionCompiler : ExpressionVisitor
	{
		#region Fields
		private int _visitCount;
		private readonly IIocContainer _container;
		private IExpressionRegistration _target;
		private IExpressionRegistration _current;
		#endregion

		#region Constructors
		public ExpressionCompiler(IIocContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			_container = container;
		}
		#endregion

		#region Methods
		public Expression<Func<IResolver, object>> Compile<T>(IExpressionRegistration<T> registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			_visitCount = 0;
			_target = _current = registration;

			// Start visiting expression to get "compiled" expression
			var expression = (Expression<Func<IResolver, object>>)Visit(registration.Expression);
			
			return expression;
		}

		// Full generic version
		//public Expression<Func<IResolver, T>> Compile<T>(ExpressionRegistration<T> registration)
		//{
		//    if (registration == null)
		//        throw new ArgumentNullException("registration");

		//    _visitCount = 0;
		//    _target = _current = registration;

		//    // Start visiting expression to get "compiled" expression
		//    var expression = (Expression<Func<IResolver, T>>)Visit(registration.Expression);

		//    return expression;
		//}
		
		protected override Expression VisitUnary(UnaryExpression node)
		{
			// Cleanup unessecary cast

			// TODO: WHAT IF THERE IS A CAST SOMEWHERE ELSE INSIDE THE STATEMENT !? - Dont do this ! 

			var exp = base.VisitUnary(node);

			// if it has been changed
			if (exp != node && exp.NodeType == ExpressionType.Convert)
			{
				var convertExp = (UnaryExpression)exp;

				// Check that what is inside the operand is of the type expected
				if (convertExp.Type.IsAssignableFrom(convertExp.Operand.Type))
				{
					// Remove the convert by returning the operand
					return convertExp.Operand;
				}
			}

			return exp;
		}

		private IRegistration GetRegistration(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			IRegistration registration;

			if (!(key == null ? _container.Index.TryGet(type, out registration) : _container.Index.TryGet(type, key, out registration)))
			{
				// Not able to find registration
				// If using _index.Get() and not found it will throw KeyNotFoundException
				// Could catch the KeyNotFoundException and wrap it in the InvalidOperationException instead ?			

				//var msg = "Error occured when trying to compile registration for " + FormatTypeKeyMessage(_target.Type, _target.Key) + ". " +
				//          "Cannot find the registration for " + FormatTypeKeyMessage(type, key);
				//if (_current != _target)
				//    msg += ", referenced in the registration for " + FormatTypeKeyMessage(_current.Type, _current.Key);
				var msg = "Error occured when trying to compile registration";

				throw new InvalidOperationException(msg);
			}

			return registration;
		}

		private string FormatTypeKeyMessage(Type type, Object key)
		{
			var str = "type: " + type.Name;

			if (key != null)
				str += " with key: " + key;

			return str;
		}

		private Expression HandleMethodCall(Type type, object key = null)
		{
			// Simple protection against infinite loop
			// Assumes infinit loop if more than 100 calls 
			if (_visitCount >= 100)
				throw new InvalidOperationException("Registration for/including " + FormatTypeKeyMessage(type, key) + " caused an infinite loop.");
			_visitCount++;



			IRegistration registration = GetRegistration(type, key);

			// Check if it is an InstanceRegistration<>
			if (ReflectionHelper.IsInstanceOfGenericType(typeof(InstanceRegistration<>), registration) && _target.CompileMode == CompileMode.Delegate)
			{
				var instance = registration.GetInstance(_container);
				return ConstantExpression.Constant(instance);
			}

			// Check if it is an IExpressionRegistration
			var confReg = registration as IExpressionRegistration;
			if (confReg != null)
			{
				// Check if the lifetime can be compiled

				if (confReg.Lifetime is TransientLifetime)
				{
					// Keep visiting and try to inline the expression of the Registration found
					// Recursive call until deepest nested ExpressionRegistration with TransientLifetime is found

					return HandleTransientLifetimeRegistration(confReg);
				}

				if (confReg.Lifetime is ContainerLifetime && _target.CompileMode == CompileMode.Delegate)
				{
					var instance = confReg.GetInstance(_container);
					return ConstantExpression.Constant(instance);
				}
			}



			return null;
		}

		private Expression HandleTransientLifetimeRegistration(IExpressionRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			// About to visit a child to the current - current becomes parent and registration/child to visit becomes current
			var parent = _current;
			_current = registration;

			// Visit - visit the body and return it so it is inlined within the original Expression<Func<IResolver, object>>
			var expression = Visit(registration.Expression.Body);

			// Returned from visiting child - change it back again
			_current = parent;

			return expression;
		}

		private bool IsMethodOnParameter(MethodCallExpression expression, Type instanceType, string methodName)
		{
			// If method is not called on an object
			if (expression.Object == null)
				return false;

			// If not a parameter
			if (expression.Object.NodeType != ExpressionType.Parameter)
				return false;

			// If not of correct type
			if (expression.Object.Type != instanceType)
				return false;

			// If not correct name
			if (expression.Method.Name != methodName)
				return false;

			return true;
		}

		private bool TryGetValue<T>(Expression expression, out T value)
		{
			if (expression is ConstantExpression)
			{
				var constantExpression = (ConstantExpression)expression;
				value = (T)constantExpression.Value;
				return true;
			}
			
			if (expression is MemberExpression)
			{
				var memberExpression = (MemberExpression)expression;
				value = (T)ExpressionHelper.GetMemberValue(memberExpression);
				return true;
			}

			value = default(T);
			return false;
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			// Intercept calls to Resolve() and Resolve<>() on the input parameter and try to handle the method call by replacing it with something "faster"

			// TODO: Support TryResolve (Type/Key, <T>/Key), GetAll(), and TryGetAll()

			if (IsMethodOnParameter(expression, typeof(IResolver), "Resolve"))
			{
				// Not Generic - Resolve(Type) & Resovle(Type, Key)
				if (!expression.Method.IsGenericMethod)
				{
					var arguments = expression.Arguments;
					var typeArgExpression = arguments[0];

					// Check that arguments was supplied and first one is of type Type
					if (arguments.Count > 0 && typeArgExpression.Type == typeof(Type))
					{
						Type type;
						if (TryGetValue(typeArgExpression, out type))
						{
							if (arguments.Count == 1) // object Resolve(Type type)
							{
								return HandleMethodCall(type) ?? base.VisitMethodCall(expression);
							}

							if (arguments.Count == 2) // object Resolve(Type type, object key)	
							{
								object key;
								if (TryGetValue(arguments[1], out key))
								{
									return HandleMethodCall(type, key) ?? base.VisitMethodCall(expression);								
								}
							}
						}
					}

					// Throw exception ?
				}
				else // Generic - Resolve<>() & Resolve<>(key)
				{
					var genericMethodDef = expression.Method.GetGenericMethodDefinition();
					var genericMethodDefArgs = genericMethodDef.GetGenericArguments();

					// Have same Generic definition (1 generic parameter and return type is the same a generic parameter)
					if (genericMethodDefArgs.Length == 1 && genericMethodDefArgs[0] == genericMethodDef.ReturnType)
					{
						// Get the generic argument type
						//var genericType = methodExpression.Method.GetGenericArguments()[0];
						var genericType = expression.Type;

						// Get arguments used in the method call
						var arguments = expression.Arguments;

						if (arguments.Count == 0) // T Resolve<T>()
						{
							return HandleMethodCall(genericType) ?? base.VisitMethodCall(expression);
						}

						if (arguments.Count == 1 && (!arguments[0].Type.IsImplementationOfGenericInterface(typeof(IRegistration<>)))) // T Resolve<T>(object key)
						{
							// Need to check if it hits the T Resolve<T>(IRegistration<T>) overload method
							// Should only handle if T Resolve<T>(object key)

							var keyArgExpression = arguments[0];

							object key;
							if (TryGetValue(keyArgExpression, out key))
							{
								return HandleMethodCall(genericType, key) ?? base.VisitMethodCall(expression);
							}
						}
					}

					// Throw Exception ?
				}
			}

			// Add support for TryResolve here ?
			//if (IsMethodOnParameter(expression, typeof(IResolver), "TryResolve"))
			//{
			//}

			return base.VisitMethodCall(expression);
		}
		#endregion
	}
}
