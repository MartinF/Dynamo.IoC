using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testing the Register methods on the Container
	
namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class RegisterTest
	{
		[TestMethod]
		public void RegisterReturnsCorrectType()
		{
			using (var container = new IocContainer())
			{
				var reg = container.Register<IFoo>(c => new Foo1());

				// Correct return type
				Assert.IsInstanceOfType(reg, typeof(IExpressionRegistration));
				Assert.IsTrue(reg.ReturnType == typeof(IFoo));
				
				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo)));
			}
		}

		[TestMethod]
		public void RegisterUsingKeyReturnsCorrectType()
		{
			using (var container = new IocContainer())
			{
				var reg = container.Register<IFoo>(c => new Foo1(), "Bar");
				
				// Correct return type
				Assert.IsInstanceOfType(reg, typeof(IExpressionRegistration));
				Assert.IsTrue(reg.ReturnType == typeof(IFoo));

				// Test index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo), "Bar"));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterThrowsExceptionIfRegistrationAlreadyExists()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFoo>(c => new Foo2());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterUsingKeyThrowsExceptionIfRegistrationAlreadyExists()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "Bar");
				container.Register<IFoo>(c => new Foo2(), "Bar");
			}
		}

		[TestMethod]
		public void RegisterWithPropertyInjectionResolvesToCorrectType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(c => new Bar1());
				container.Register<IFooBar>(c => new FooBar() { Foo = c.Resolve<IFoo>(), Bar = c.Resolve<IBar>() });

				var instance = container.Resolve<IFooBar>();

				Assert.IsNotNull(instance);
				Assert.IsNotNull(instance.Foo);
				Assert.IsNotNull(instance.Bar);

				Assert.IsInstanceOfType(instance, typeof(FooBar));
				Assert.IsInstanceOfType(instance.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(instance.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void RegisterUsingKeyWithPropertyInjectionResolvesToCorrectType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "Foo");
				container.Register<IBar>(c => new Bar1(), "Bar");
				container.Register<IFooBar>(c => new FooBar() { Foo = c.Resolve<IFoo>("Foo"), Bar = c.Resolve<IBar>("Bar") }, "PropInjection");

				var instance = container.Resolve<IFooBar>("PropInjection");

				Assert.IsNotNull(instance);
				Assert.IsNotNull(instance.Foo);
				Assert.IsNotNull(instance.Bar);

				Assert.IsInstanceOfType(instance, typeof(FooBar));
				Assert.IsInstanceOfType(instance.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(instance.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void RegisterCanRegisterAStruct()
		{
			// Works with CompileMode.Delegate
			// Also works with CompileMode.Dynamic - but only if value is included directly - cannot reference a local variable

			// In this case it would make more sense to use RegisterInstance instead of Register - set constraint on Register so it cant register a struct?

			using (var container = new IocContainer())
			{
				var registration = container.Register<int>(x => 32);		// Should use RegisterInstance ? will it work ?

				// Check registration
				Assert.IsInstanceOfType(registration, typeof(IExpressionRegistration));
				Assert.AreSame(registration.ReturnType, typeof(int));

				// Try to resolve
				var result = container.Resolve<int>();
				Assert.AreEqual(32, result);

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(int)));
			}
		}

		[TestMethod]
		public void RegisterCanPointToMethod()
		{
			using (var container = new IocContainer())
			{
				var reg = container.Register<IFoo>(c => ContainLogic(true));

				// Correct return type
				Assert.IsInstanceOfType(reg, typeof(IExpressionRegistration));
				Assert.IsTrue(reg.ReturnType == typeof(IFoo));

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo)));

				// Resolve
				var instance = container.Resolve<IFoo>();
				Assert.IsNotNull(instance);
			}
		}

		public IFoo ContainLogic(bool def)
		{
			if (def)
				return new Foo1();

			return new Foo2();
		}
	}
}