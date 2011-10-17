using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dynamo.Ioc.Index;

// Add Support for TryResolve (Type/Key, and <T>) - if not found just inline null, or keep TryResolve expression.
// Turn Generic method calls into the equvilant using type directly ? - very small improvement

// Add support for using cached compiled registration to speed it up instead of compiling the whole "tree" for each registration ?

namespace Dynamo.Ioc.Registration
{
	public class ExpressionCompiler : ExpressionVisitor
	{
		#region Fields
		private readonly IIndexAccessor _index;
		private ExpressionRegistration _currentRegistration;
		#endregion

		#region Constructors
		public ExpressionCompiler(IIndexAccessor index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			_index = index;
		}
		#endregion

		#region Methods
		public bool TryCompile(Expression<Func<IResolver, object>> expression, out Expression<Func<IResolver, object>> compiledExpression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			var newExpression = (Expression<Func<IResolver, object>>)this.Visit(expression);

			if (newExpression != expression)
			{
				compiledExpression = newExpression;
				return true;
			}

			compiledExpression = null;
			return false;
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

		private IRegistration GetRegistration(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			IRegistration registration;

			if (!(key == null ? _index.TryGet(type, out registration) : _index.TryGet(type, key, out registration)))
			{
				// Not able to find registration referenced.
				// Could choose to ignore it and let it fail later when trying to Resolve if the type/key havent been added to the index inbetween then.

				var exceptionDescription = "Cannot Compile Registration. The registration have a reference to Type: " + type.Name;
				if (key != null)
					exceptionDescription += " with Key: " + key;	// only if string it makes sense to use the ToString on the object ? else get the type ?
				exceptionDescription += " which isn't registered.";

				// TODO: Throw CompileException or something ?
				throw new RegistrationException(_currentRegistration, exceptionDescription);
			}

			return registration;
		}

		private Expression HandleMethodCall(Type type, object key = null)
		{
			IRegistration registration = GetRegistration(type, key);

			// Only possible to compile ExpressionRegistration's
			var expressionRegistration = registration as ExpressionRegistration;
			if (expressionRegistration != null)
			{
				// Currently the Compiler only support compiling TransientLifetime's

				if (expressionRegistration.Lifetime.GetType() == typeof(TransientLifetime))		// Check specific type incase something changes in the future and TransientLifetime is no longer sealed? in stead of using "is TransientLifetime"
				{
					// Keep visiting and try to inline the expression
					// Recursive call until deepest nested ExpressionRegistration with TransientLifetime is found

					// Change current registration to new registration which should be visited and save current for later
					var current = _currentRegistration;
					_currentRegistration = expressionRegistration;

					// Visit
					var expression = this.Visit(expressionRegistration.Expression.Body);

					// Change it back again
					_currentRegistration = current;

					return expression;
				}
			}

			return null;
		}

		private bool IsMethodOnParameter(MethodCallExpression expression, Type instanceType)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (instanceType == null)
				throw new ArgumentNullException("instanceType");

			// If method is not called on an object
			if (expression.Object == null)
				return false;

			// If not a parameter
			if (expression.Object.NodeType != ExpressionType.Parameter)
				return false;

			// If not of correct type
			if (expression.Object.Type != instanceType)
				return false;

			return true;
		}

		private bool IsMethod(MethodCallExpression expression, string methodName, bool isGeneric)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (methodName == null)
				throw new ArgumentNullException("methodName");

			// If not correct name
			if (expression.Method.Name != methodName)
				return false;

			// if not of correct type (generic or not)
			if (expression.Method.IsGenericMethod != isGeneric)
				return false;

			return true;
		}

		protected override Expression VisitMethodCall(MethodCallExpression methodExpression)
		{
			// Cleanup handling the parameters
			// Support TryResolve (Type/Key, <T>/Key)

			if (IsMethodOnParameter(methodExpression, typeof(IResolver)))
			{
				// Resolve(Type) & Resovle(Type, Key)
				if (IsMethod(methodExpression, "Resolve", false))
				{
					var arguments = methodExpression.Arguments;

					// Check that arguments was supplied and first one is of type Type
					if (arguments.Count > 0 && arguments[0] is ConstantExpression && arguments[0].Type == typeof(Type))
					{
						var typeArgument = (ConstantExpression)arguments[0];
						var type = (Type)typeArgument.Value;

						if (arguments.Count == 1) // object Resolve(Type type)
						{
							return HandleMethodCall(type) ?? base.VisitMethodCall(methodExpression);
						}

						if (arguments.Count == 2) // object Resolve(Type type, object key)	
						{
							// Also check - && arguments[1] is ConstantExpression && arguments[1].Type == typeof(Object)) ? Last check wont work if etc string is supplied, and checking if it is an object doesnt make sense as everything is an object?
							
							var keyArgument = (ConstantExpression)arguments[1];
							var key = keyArgument.Value;

							return HandleMethodCall(type, key) ?? base.VisitMethodCall(methodExpression);
						}
					}

					// Throw exception ?
				}

				// Resolve<>() & Resolve<>(key)
				if (IsMethod(methodExpression, "Resolve", true))
				{
					var genericMethodDef = methodExpression.Method.GetGenericMethodDefinition();
					var genericMethodDefArgs = genericMethodDef.GetGenericArguments();

					// Have same Generic definition (1 generic parameter and return type is the same a generic parameter)
					if (genericMethodDefArgs.Length == 1 && genericMethodDefArgs[0] == genericMethodDef.ReturnType)
					{
						// Get the generic argument type
						//var genericType = methodExpression.Method.GetGenericArguments()[0];
						var genericType = methodExpression.Type;

						// Get arguments used in the method call
						var arguments = methodExpression.Arguments;

						// Check that arguments was supplied and first one is a ParameterExpression and is of type IResolver
						if (arguments.Count > 0 && arguments[0] is ParameterExpression && arguments[0].Type == typeof(IResolver))
						{
							// Check somehow that the argument[0] (as ParameterExpression) is the same as the input parameter for the expression !
							// Need to have the expression which is being visited stored somewhere !?
							//var methodArgument1 = (ParameterExpression)arguments[0];

							if (arguments.Count == 0) // T Resolve<T>()
							{
								return HandleMethodCall(genericType) ?? base.VisitMethodCall(methodExpression);
							}

							if (arguments.Count == 1) // T Resolve<T>(object key)
							{
								// && arguments[1] is ConstantExpression && arguments[1].Type == typeof(Object))		// TODO PROBLEM HERE ! contains the actual type and not object probably so never gets compiled !?

								var keyArgument = (ConstantExpression)arguments[0];
								var key = keyArgument.Value;

								return HandleMethodCall(genericType, key) ?? base.VisitMethodCall(methodExpression);
							}
						}
					}

					// Throw Exception ?
				}
			}



			return base.VisitMethodCall(methodExpression);
		}
		#endregion
	}
}
