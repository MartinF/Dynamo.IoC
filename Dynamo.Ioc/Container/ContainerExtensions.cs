using System;
using System.Linq.Expressions;

namespace Dynamo.Ioc
{
	public static class ContainerExtensions
	{
		#region Register
		public static IConfigurableRegistration Register<T>(this IContainer container, Expression<Func<IResolver, T>> expression)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.Register(typeof(T), expression.Convert());
		}
		public static IConfigurableRegistration Register<T>(this IContainer container, object key, Expression<Func<IResolver, T>> expression)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.Register(typeof(T), key, expression.Convert());
		}
		#endregion

		#region Register <TType, TImpl> - Automatic
		public static IConfigurableRegistration Register<TType, TImpl>(this IContainer container)
			where TType : class
			where TImpl : class, TType
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.Register(typeof(TType), ExpressionHelper.CreateRegistration<object, TImpl>());
		}
		public static IConfigurableRegistration Register<TType, TImpl>(this IContainer container, object key)
			where TType : class
			where TImpl : class, TType
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.Register(typeof(TType), key, ExpressionHelper.CreateRegistration<object, TImpl>());
		}
		#endregion

		#region Register Instance
		public static IRegistrationInfo RegisterInstance<T>(this IContainer container, T instance)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.RegisterInstance(typeof(T), instance);
		}
		public static IRegistrationInfo RegisterInstance<T>(this IContainer container, object key, T instance)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			return container.RegisterInstance(typeof(T), key, instance);
		}
		#endregion
	}
}