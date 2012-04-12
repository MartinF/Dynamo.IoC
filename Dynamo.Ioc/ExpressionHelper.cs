using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Dynamo.Ioc
{
	internal static class ExpressionHelper
	{
		public static object GetMemberValue(MemberExpression member)
		{
			var objectMember = Expression.Convert(member, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember);

			var getter = getterLambda.Compile();

			return getter();
		}

		public static Expression<Func<IResolver, object>> Convert<TOut>(this Expression<Func<IResolver, TOut>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			// If already of the right form - just return it - explicit cast wont compile ... !?
			if (typeof(TOut) == typeof(Object))
				return expression as Expression<Func<IResolver, object>>;

			Expression<Func<IResolver, object>> convertedExpression;
			Type outputType = typeof(TOut);

			if (outputType.IsValueType)
			{
				// Any way to get rid of the boxing ?
				Expression converted = Expression.Convert(expression.Body, typeof(object));
				convertedExpression = Expression.Lambda<Func<IResolver, object>>(converted, expression.Parameters);
			}
			else
			{
				convertedExpression = Expression.Lambda<Func<IResolver, object>>(expression.Body, expression.Parameters);
			}

			return convertedExpression;
		}

		// Remove ?
		public static Func<IResolver, object> CompileDynamic(this Expression<Func<IResolver, object>> expression)
		{
			// Speed up the performance of the Func created from an Expression
			// Wasnt there a even faster way of doing this - see question ?

			if (expression == null)
				throw new ArgumentNullException("expression");

			// Create assembly and construct method
			var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Dynamo.Ioc.Dynamic"), AssemblyBuilderAccess.Run);
			var mod = ab.DefineDynamicModule("DynamicModule");
			var tb = mod.DefineType("DynamicType", TypeAttributes.Public);
			var mb = tb.DefineMethod("FactoryDelegate", MethodAttributes.Public | MethodAttributes.Static);

			// Compile the Expression to a method definition
			expression.CompileToMethod(mb);

			var type = tb.CreateType();
			var newDelegate = (Func<IResolver, object>)Delegate.CreateDelegate(typeof(Func<IResolver, object>), type.GetMethod("FactoryDelegate"), true);

			return newDelegate;
		}

		public static Func<IResolver, T> CompileDynamic<T>(this Expression<Func<IResolver, T>> expression)
		{
			// Speed up the performance of the Func created from an Expression
			// Wasnt there a even faster way of doing this - see question ?

			if (expression == null)
				throw new ArgumentNullException("expression");

			// Create assembly and construct method
			var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Dynamo.Ioc.Dynamic"), AssemblyBuilderAccess.Run);
			var mod = ab.DefineDynamicModule("DynamicModule");
			var tb = mod.DefineType("DynamicType", TypeAttributes.Public);
			var mb = tb.DefineMethod("FactoryDelegate", MethodAttributes.Public | MethodAttributes.Static);

			// Compile the Expression to a method definition
			expression.CompileToMethod(mb);

			var type = tb.CreateType();
			var newDelegate = (Func<IResolver, T>)Delegate.CreateDelegate(typeof(Func<IResolver, T>), type.GetMethod("FactoryDelegate"), true);

			return newDelegate;
		}

		public static Expression<Func<IResolver, TType>> CreateExpression<TType, TImpl>(bool includeInternalCtor = false, Func<ConstructorInfo[], ConstructorInfo> selector = null)
			where TType : class
			where TImpl : class, TType
		{
			// if (type.IsValueType) inline - FormatterServices.GetUninitializedObject(typeof(int)); to make it work with structs ?
			// Dont care about structs, they shouldnt be used with the Register auto method any way, which is the only method needing and using this

			// Get Constructer Info for the TImpl type
			ConstructorInfo constructor = ReflectionHelper.GetConstructor(typeof(TImpl), includeInternalCtor);
			
			// Get each parameter accepted by the constructor
			ParameterInfo[] parameters = constructor.GetParameters();

			// Input parameter in Func<IResolver, TType>
			ParameterExpression container = Expression.Parameter(typeof(IResolver), "resolver");

			// Create List of arguments for the constructor
			// ((TYPE)c.Resolve(typeof(parameterType1), (TYPE)c.Resolve(typeof(parameterType2))) etc.
			List<Expression> arguments = new List<Expression>();
			foreach (var paramInfo in parameters)
			{
				// Call Nongeneric method on input parameter - Best
				var resolve = Expression.Call(container, "Resolve", null, new Expression[] { Expression.Constant(paramInfo.ParameterType, typeof(Type)) });
				var call = Expression.Convert(resolve, paramInfo.ParameterType);	// Is this cast really needed ?

				// Call Generic method on input parameter - Worst 
				//var call = Expression.Call(container, "Resolve", new Type[] { paramInfo.ParameterType }, new Expression[] { });

				// Call Static Generic method
				//var call = Expression.Call(typeof(ContainerExtensions), "Resolve", new Type[] { paramInfo.ParameterType }, new Expression[] { container });

				arguments.Add(call);
			}

			// new SomeType(argument1, argument2)
			NewExpression exp = Expression.New(constructor, arguments);

			// input => NewExpression
			var lambdaExpression = Expression.Lambda<Func<IResolver, TType>>(exp, new ParameterExpression[] { container });
			
			return lambdaExpression;
		}
	}
}