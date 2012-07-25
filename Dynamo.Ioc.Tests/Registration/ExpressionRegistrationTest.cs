using System;
using System.Linq.Expressions;
using Dynamo.Ioc.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Compile ?

namespace Dynamo.Ioc.Tests.Registration
{
	[TestClass]
	public class ExpressionRegistrationTest
	{
		[TestMethod]
		public void ExpressionRegistrationWorksLikeExpected()
		{
			Expression<Func<IResolver, IFoo>> expression = x => new Foo1();
			var container = new IocContainer();
			var lifetime = new TransientLifetime();
			var compileMode = CompileMode.Dynamic;

			var reg = new ExpressionRegistration<IFoo>(container, expression, lifetime, compileMode);

			Assert.AreEqual(reg.ReturnType, typeof(IFoo));

			Assert.AreEqual(reg.CompileMode, compileMode);
			//Assert.AreSame(reg.Expression, expression); // Is being converted to Expression<Func<IResolver, object>>

			Assert.AreSame(reg.Lifetime, lifetime);
			
			// Test GetInstance
			var instance = reg.GetInstance();
			Assert.IsInstanceOfType(instance, typeof(Foo1));
		}

		[TestMethod]
		public void ExpressionRegistrationCanBeChanged()
		{
			var container = new IocContainer();
			var lifetime = new TransientLifetime();
			var reg = new ExpressionRegistration<IFoo>(container, x => new Foo1(), lifetime, CompileMode.Delegate);

			Assert.AreSame(reg.Lifetime, lifetime);
			Assert.AreEqual(reg.CompileMode, CompileMode.Delegate);

			var get1 = reg.GetInstance();

			var newLifetime = new ContainerLifetime();
			var newCompileMode = CompileMode.Dynamic;

			// Set new lifetime
			reg.SetLifetime(newLifetime);
			Assert.AreSame(reg.Lifetime, newLifetime);

			// Set different compile mode
			reg.SetCompileMode(newCompileMode);
			Assert.AreEqual(reg.CompileMode, newCompileMode);

			var get2 = reg.GetInstance();
			var get3 = reg.GetInstance();

			// Check that the lifetime is also being used
			Assert.AreNotSame(get1, get2);
			Assert.AreSame(get2, get3);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationSetLifetimeThrowsExceptionIfInvalidLifetimeIsSupplied()
		{
			var container = new IocContainer();		
			var lifetime = new TransientLifetime();
			var reg = new ExpressionRegistration<IFoo>(container, x => new Foo1(), lifetime, CompileMode.Delegate);

			reg.SetLifetime((ILifetime)null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ExpressionRegistrationSetCompileModeThrowsExceptionIfInvalidCompileModeIsSupplied()
		{
			var container = new IocContainer();
			var lifetime = new TransientLifetime();
			var reg = new ExpressionRegistration<IFoo>(container, x => new Foo1(), lifetime, CompileMode.Delegate);

			reg.SetCompileMode((CompileMode)124);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationThrowsExceptionIfResolverIsNull()
		{
			var reg = new ExpressionRegistration<object>(null, x => new object(), new TransientLifetime());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationThrowsExceptionIfExpressionIsNull()
		{
			var container = new IocContainer();
			var reg = new ExpressionRegistration<object>(container, null, new TransientLifetime());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationThrowsExceptionIfLifetimeIsNull()
		{
			var container = new IocContainer();
			var reg = new ExpressionRegistration<object>(container, x => new object(), null);
		}
	}
}
