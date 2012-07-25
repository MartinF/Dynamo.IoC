using System;
using System.Linq.Expressions;

// Adds support for fluent like syntax

// SetCompileMode vs WithCompileMode ?
// SetExpression vs WithExpression ?

namespace Dynamo.Ioc
{
	public static class ExpressionRegistrationExtensions
	{
		public static T SetCompileMode<T>(this T registration, CompileMode compileMode)
			where T : IExpressionRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.CompileMode = compileMode;

			return registration;
		}

		public static T SetExpression<T>(this T registration, Expression<Func<IResolver, object>> expression)
			where T : IExpressionRegistration
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			registration.Expression = expression;

			return registration;
		}
	}
}
