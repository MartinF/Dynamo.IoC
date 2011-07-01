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
		public static Expression<Func<IResolver, object>> Convert<TOutput>(this Expression<Func<IResolver, TOutput>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			Expression<Func<IResolver, object>> convertedExpression;
			Type outputType = typeof(TOutput);

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

		public static Func<IResolver, object> CompileDynamic(this Expression<Func<IResolver, object>> expression)
		{
			// Speed up the performance of the Func created from an Expression
			// Wasnt there a even faster way of doing this ?

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

		public static Expression<Func<IResolver, TType>> CreateRegistration<TType, TImpl>(bool includeInternal = true)
			where TType : class
			where TImpl : class, TType
		{
			// if (type.IsValueType) inline - FormatterServices.GetUninitializedObject(typeof(int)); to make it work with structs ?



			// Get Constructer Info for the TImpl type
			ConstructorInfo constructor = GetConstructorInfo<TImpl>(includeInternal);
			
			// Get each parameter accepted by the constructor
			ParameterInfo[] parameters = constructor.GetParameters();

			// Input parameter in Func<IResolver, TType>
			ParameterExpression container = Expression.Parameter(typeof(IResolver), "resolver");

			// Create List of arguments for the constructor
			// ((TYPE)c.Resolve(typeof(parameterType1), (TYPE)c.Resolve(typeof(parameterType2))) etc.
			List<Expression> arguments = new List<Expression>();
			foreach (var paramInfo in parameters)
			{
				// Not happening what i expect to happen - it is actually calling the generic method isnt it ? something is wrong.

				// Call Nongeneric method on input parameter - Best
				var resolve = Expression.Call(container, "Resolve", null, new Expression[] { Expression.Constant(paramInfo.ParameterType, typeof(Type)) });
				var call = Expression.Convert(resolve, paramInfo.ParameterType);

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

		private static ConstructorInfo GetConstructorInfo<T>(bool includeInternal = true)
			where T : class
		{
			var type = typeof(T);

			// Get all Public Constructors
			var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

			// Only use internal constructor if no public constructor is found
			// IsAssembly = internal, IsFamilyOrAssembly = protected internal
			if (includeInternal && constructors.Length == 0)
				constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.IsAssembly || x.IsFamilyOrAssembly).ToArray();

			// Find the Constructor with the most parameters
			var constructor = constructors.OrderBy(c => c.GetParameters().Length).LastOrDefault();
			
			if (constructor == null)
				throw new ArgumentException("Type: " + type.Name + " does not have a public" + (includeInternal ? " or internal" : "") + " constructor.");

			return constructor;
		}
	}
}