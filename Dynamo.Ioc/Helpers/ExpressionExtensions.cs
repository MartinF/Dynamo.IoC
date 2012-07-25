using System;
using System.Linq.Expressions;

namespace Dynamo.Ioc
{
	internal static class ExpressionExtensions
	{
		public static bool IsMethod<T, TResult>(this MethodCallExpression expression, Expression<Func<T, TResult>> method, bool replaceGenericReturnType = false)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (method == null)
				throw new ArgumentNullException("method");

			var compareMethodCallExpression = method.Body as MethodCallExpression;

			if (compareMethodCallExpression == null)
				return false;

			var compareMethodInfo = compareMethodCallExpression.Method;

			if (replaceGenericReturnType)
			{
				if (!expression.Method.IsGenericMethod)
					return false;

				if (!compareMethodCallExpression.Method.IsGenericMethod)
					return false;

				var methodCallExpressionReturnType = expression.Method.ReturnType;
				var methodDefinition = compareMethodCallExpression.Method.GetGenericMethodDefinition();
				var genericMethod = methodDefinition.MakeGenericMethod(methodCallExpressionReturnType);

				compareMethodInfo = genericMethod;
			}

			if (expression.Method != compareMethodInfo)
				return false;

			return true;
		}

		public static T GetMethodArgument<T>(this MethodCallExpression expression, int index)
		{
			var arguments = expression.Arguments;
			var typeArgExpression = arguments[index];

			if (typeArgExpression is ConstantExpression)
			{
				var constantExpression = (ConstantExpression)typeArgExpression;
				return (T)constantExpression.Value;
			}

			if (typeArgExpression is MemberExpression)
			{
				var memberExpression = (MemberExpression)typeArgExpression;
				return (T)ExpressionHelper.GetMemberValue(memberExpression);
			}

			throw new InvalidOperationException("Cannot get method argument");
		}
	}
}
