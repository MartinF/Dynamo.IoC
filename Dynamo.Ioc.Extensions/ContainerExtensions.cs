using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Ioc.Extensions
{
	public static class ContainerExtensions
	{
		//public static void RegisterInstanceForMultiple<TType1, TType2, TImpl>(this IIocContainer container, TImpl instance, object key = null)
		//	where TImpl : TType1, TType2
		//{
		//	// Need to return a IMultipleRegistration wrapper type that contains both IRegistrations returned by RegisterInstance?
			
		//	var reg1 = container.RegisterInstance<TType1>(instance, key);
		//	var reg2 = container.RegisterInstance<TType2>(instance, key);

		//	// Create a wrapper for the two registrations and return it
		//}

		//public static IExpressionRegistration RegisterForMultiple<TType1, TType2, TImpl>(object key = null, ILifetime lifetime = null, CompileMode? compileMode = null, bool includeInternalCtor = true, Func<ConstructorInfo[], ConstructorInfo> selector = null)
		//	where TType1 : class
		//	where TType2 : class
		//	where TImpl : class, TType1, TType2
		//{
		//}

		//public static IExpressionRegistration RegisterForMultiple<TType1, TType2, TImpl>(Expression<Func<IResolver, T>> expression, object key = null, ILifetime lifetime = null, CompileMode? compileMode = null)
		//	where TType1 : class
		//	where TType2 : class
		//	where TImpl : class, TType1, TType2
		//{		
		//}
	}
}
