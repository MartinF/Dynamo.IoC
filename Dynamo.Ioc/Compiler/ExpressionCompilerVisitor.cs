using System;
using System.Linq.Expressions;

// Add Support for TryResolve (Type/Key, and <T>) - if not found just inline null, or keep TryResolve expression.
// + Resolve(Registration) and Resolve(Registration<T>) ?

// Turn Generic method calls into the equvilant using type directly ? - very small improvement
// TryCompile - which doesnt throw exception if not able to compile / find a reference to a registration needed ?

// Better protection against infinite loop ?
	// Use stack etc and push and pop as tree is crawled/compiled and check for loop

namespace Dynamo.Ioc.Compiler
{
	public class ExpressionCompilerVisitor : ExpressionVisitor
	{
		#region Fields
		protected IExpressionRegistration _targetRegistration;
		protected IExpressionRegistration _currentVisitedRegistration;
		protected ParameterExpression _targetParameter;
		protected int _visitCount;
		#endregion

		#region Constructors
		#endregion

		#region Methods

		public Expression<Func<IResolver, object>> Compile(IExpressionRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			_targetRegistration = _currentVisitedRegistration = registration;
			_targetParameter = registration.Expression.Parameters[0];
			
			_visitCount = 0;

			// Start visiting expression to get compiled expression
			var compiledExpression = (Expression<Func<IResolver, object>>)Visit(registration.Expression);

			return compiledExpression;
		}
		
		private Expression HandleMethodCall(Type type, object key = null)
		{
			// Simple protection against infinite loop
			// Assumes infinit loop if more than 10000 visits 
			if (_visitCount >= 10000)
				throw new InvalidOperationException("Registration for/including " + FormatTypeKeyMessage(type, key) + " caused an infinite loop.");
			
			_visitCount++;

			// Lookup the registration
			IRegistration registration = GetRegistration(type, key);

			return CompileRegistrationIntoExpression(registration);
		}

		protected virtual Expression CompileRegistrationIntoExpression(IRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");
			
			// Check if it is an InstanceRegistration<>
			if (ReflectionHelper.IsInstanceOfGenericType(typeof(InstanceRegistration<>), registration) && _targetRegistration.CompileMode == CompileMode.Delegate)
			{
				var instance = registration.GetInstance();
				return ConstantExpression.Constant(instance);
			}

			// Check if it is an IExpressionRegistration
			var expReg = registration as IExpressionRegistration;
			if (expReg != null)
			{
				// Check if the lifetime can be compiled

				if (expReg.Lifetime is TransientLifetime)
				{
					// Keep visiting and try to inline the expression of the Registration found
					// Recursive call until deepest nested ExpressionRegistration with TransientLifetime is found

					// About to visit a child to the current - current becomes parent and registration/child to visit becomes current
					var parent = _currentVisitedRegistration;
					_currentVisitedRegistration = expReg;

					// Visit - visit the body and return it so it is inlined within the original Expression<Func<IResolver, object>>
					var expression = Visit(expReg.Expression.Body);

					// Returned from visiting child - change it back again
					_currentVisitedRegistration = parent;

					return expression;
				}

				if (expReg.Lifetime is ContainerLifetime && _targetRegistration.CompileMode == CompileMode.Delegate)
				{
					var instance = expReg.GetInstance();
					return ConstantExpression.Constant(instance);
				}
			}

			// Returning null will make it keep visiting
			return null;
		}

		#endregion

		#region Visitor Methods - overrides

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			// TODO: Support TryResolve (Type/Key, <T>/Key), GetAll(), and TryGetAll()

			// Intercept calls to Resolve() and Resolve<>() on the input parameter and try to handle the method call by replacing it with something "faster"

			if (expression.IsMethod<IResolver, object>(resolver => resolver.Resolve((Type)null)))
			{
				// Resolve(Type)

				var typeArg = expression.GetMethodArgument<Type>(0);

				return HandleMethodCall(typeArg) ?? base.VisitMethodCall(expression);
			}
			
			if (expression.IsMethod<IResolver, object>(resolver => resolver.Resolve((Type)null, (object)null)))
			{
				// Resolve(Type, Key)

				var typeArg = expression.GetMethodArgument<Type>(0);
				var keyArg = expression.GetMethodArgument<object>(1);

				return HandleMethodCall(typeArg, keyArg) ?? base.VisitMethodCall(expression);
			}
			
			if (expression.IsMethod<IResolver, object>(resolver => resolver.Resolve<object>(), replaceGenericReturnType: true))
			{
				// Resolve<T>()

				var genericType = expression.Type;

				return HandleMethodCall(genericType) ?? base.VisitMethodCall(expression);				
			}
			
			if (expression.IsMethod<IResolver, object>(resolver => resolver.Resolve<object>((object)null), replaceGenericReturnType: true))
			{
				// Resolve<T>(Key)

				var genericType = expression.Type;
				var keyArg = expression.GetMethodArgument<object>(0);

				return HandleMethodCall(genericType, keyArg) ?? base.VisitMethodCall(expression);			
			}
			
			return base.VisitMethodCall(expression);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			// Update all IResolver input parameters to reference the parameter of the target expression, to make sure all are in scope

			if (node.Type == typeof(IResolver))
			{
				// Replace parameter with targets parameter
				return _targetParameter;
			}

			return base.VisitParameter(node);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			// Cleanup unessecary cast
			
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

		#endregion

		#region Helpers
		private IRegistration GetRegistration(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			var resolver = _currentVisitedRegistration.Resolver;

			IRegistration registration;

			if (!(key == null ? resolver.Index.TryGet(type, out registration) : resolver.Index.TryGet(type, key, out registration)))
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
		#endregion
	}
}
