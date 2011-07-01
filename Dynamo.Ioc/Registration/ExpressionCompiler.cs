using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dynamo.Ioc.Index;

// Ohhhh God ! This need cleaning up !
// Create methods / Helpers for what is going on in the VisitMethodCall() to make it easier to work with.

// Add Support for TryResolve (Type/Key, and <T>) - if not found just inline null, or keep TryResolve expression.
// Turn Generic method calls into the equvilant using type directly ? - very small improvement

// Add support for using cached compiled registration to speed it up instead of compiling the whole "tree" for each registration ?

// namespace ?

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

		private bool TryGetRegistration(Type type, object key, out IRegistration registration)
		{
			return key == null ? _index.TryGet(type, out registration) : _index.TryGet(type, key, out registration);		
		}

		private Expression HandleMethodCall(Type type, object key = null)
		{
			// Split up this method ?

			IRegistration registration;

			if (!TryGetRegistration(type, key, out registration))
			{
				// Not able to find registration referenced.
				// Could choose to ignore it and let it fail later when trying to Resolve if the type/key havent been added to the index inbetween then.

				var exceptionDescription = "Cannot Compile Registration. The registration have a reference to Type: " + type.Name;
				if (key != null)
					exceptionDescription += " with Key: " + key;
				exceptionDescription += " which isn't registered.";

				// TODO: Throw CompileException or something ?
				throw new RegistrationException(_currentRegistration, exceptionDescription);
			}

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

		protected override Expression VisitMethodCall(MethodCallExpression methodExpression)
		{
			// TODO: Someday this nasty nesting needs to be fixed ... zzZzzzZzz
			// Create helper methods for testing if it is a static extension method, or a instance method on some interface ?
			// Support TryResolve (Type/Key, <T>/Key)

			// If method is called on an object
			if (methodExpression.Object != null)
			{
				// Check it is a parameter and of type IResolver
				if (methodExpression.Object.NodeType == ExpressionType.Parameter && methodExpression.Object.Type == typeof(IResolver))
				{
					if (methodExpression.Method.Name == "Resolve")
					{
						if (!methodExpression.Method.IsGenericMethod)	// Return parameter is object ! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
						{
							var arguments = methodExpression.Arguments;		// Arguments for the call

							// Check that arguments was supplied and first one is of type Type
							if (arguments.Count > 0 && arguments[0] is ConstantExpression && arguments[0].Type == typeof(Type))
							{
								var typeArgument = (ConstantExpression)arguments[0];
								var type = (Type)typeArgument.Value;

								if (arguments.Count == 1)
								{
									// object Resolve(Type type)

									return HandleMethodCall(type) ?? base.VisitMethodCall(methodExpression);
								}
								else if (arguments.Count == 2)	// && arguments[1] is ConstantExpression && arguments[1].Type == typeof(Object))		// TODO PROBLEM HERE IF STRING !?!?!? it contains the actuall type us is Object?
								{
									// object Resolve(Type type, object key)

									var keyArgument = (ConstantExpression)arguments[1];
									var key = (Object)keyArgument.Value;

									return HandleMethodCall(type, key) ?? base.VisitMethodCall(methodExpression);
								}
							}

						}

						throw new Exception("Something on the interface IResolver have changed - Resolve method is not recognized");
					}

					// More instance methods declared on IResolver here...
				}
			}
			else
			{
				// Method not called on an object - static

				var methodInfo = methodExpression.Method;

				// Is a Call on Type ResolverExtensions
				if (methodExpression.NodeType == ExpressionType.Call && methodInfo.DeclaringType == typeof(ResolverExtensions))
				{
					if (methodInfo.Name == "Resolve")
					{
						// Is Generic
						if (methodInfo.IsGenericMethod)
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

									if (arguments.Count == 1)
									{
										// T Resolve<T>(this IResolver resolver)

										return HandleMethodCall(genericType) ?? base.VisitMethodCall(methodExpression);
									}
									else if (arguments.Count == 2) // && arguments[1] is ConstantExpression && arguments[1].Type == typeof(Object))		// TODO PROBLEM HERE ! contains the actual type and not object probably so never gets compiled !?
									{
										// T Resolve<T>(this IResolver resolver, object key)

										var keyArgument = (ConstantExpression)arguments[1];
										var key = (Object)keyArgument.Value;
										
										return HandleMethodCall(genericType, key) ?? base.VisitMethodCall(methodExpression);
									}
								}
							}
						}

						throw new Exception("Something on the interface IResolver have changed - Generic Resolve method is not recognized");
					}

					// More static extensions methods declared in ResolverExtensions here...
				}
			}

			return base.VisitMethodCall(methodExpression);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
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
	}
}