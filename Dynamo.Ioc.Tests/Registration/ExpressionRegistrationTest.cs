using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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

			var reg = new ExpressionRegistration<IFoo>(expression, lifetime, compileMode);

			Assert.AreEqual(reg.ImplementationType, typeof(Foo1));
			Assert.AreEqual(reg.ReturnType, typeof(IFoo));
			Assert.AreEqual(reg.CompileMode, compileMode);

			Assert.AreSame(reg.Lifetime, lifetime);

			//Assert.AreSame(reg.Expression, expression);

			// Test GetInstance
			var instance = reg.GetInstance(container);
			Assert.IsInstanceOfType(instance, typeof(Foo1));
		}

		[TestMethod]
		public void ExpressionRegistrationCanBeChanged()
		{
			var lifetime = new TransientLifetime();
			var reg = new ExpressionRegistration<IFoo>(x => new Foo1(), lifetime, CompileMode.Delegate);

			Assert.AreSame(reg.Lifetime, lifetime);
			Assert.AreEqual(reg.CompileMode, CompileMode.Delegate);

			var get1 = reg.GetInstance(null);

			var newLifetime = new ContainerLifetime();
			var newCompileMode = CompileMode.Dynamic;

			// Set new lifetime
			reg.SetLifetime(newLifetime);
			Assert.AreSame(reg.Lifetime, newLifetime);

			// Set different compile mode
			reg.SetCompileMode(newCompileMode);
			Assert.AreEqual(reg.CompileMode, newCompileMode);

			var get2 = reg.GetInstance(null);
			var get3 = reg.GetInstance(null);

			// Check that the lifetime is also being used
			Assert.AreNotSame(get1, get2);
			Assert.AreSame(get2, get3);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationSetLifetimeThrowsExceptionIfInvalidLifetimeIsSupplied()
		{
			var lifetime = new TransientLifetime();
			var reg = new ExpressionRegistration<IFoo>(x => new Foo1(), lifetime, CompileMode.Delegate);

			reg.SetLifetime((ILifetime)null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ExpressionRegistrationSetCompileModeThrowsExceptionIfInvalidCompileModeIsSupplied()
		{
			var lifetime = new TransientLifetime();
			var reg = new ExpressionRegistration<IFoo>(x => new Foo1(), lifetime, CompileMode.Delegate);

			reg.SetCompileMode((CompileMode)124);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationThrowsExceptionIfExpressionIsNull()
		{
			var reg = new ExpressionRegistration<object>(null, new TransientLifetime());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionRegistrationThrowsExceptionIfLifetimeIsNull()
		{
			var reg = new ExpressionRegistration<object>(x => new object(), null);
		}
	}
}
