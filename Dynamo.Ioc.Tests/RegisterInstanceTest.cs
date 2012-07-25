using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testing the RegisterInstance() and RegisterInstance<T> methods on the Container

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class RegisterInstanceTest
	{
		[TestMethod]
		public void RegisterInstanceReturnsCorrectType()
		{
			using (var container = new IocContainer())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IFoo), fooInstance);

				Assert.IsInstanceOfType(result, typeof(InstanceRegistration<IFoo>));

				Assert.AreSame(result.ReturnType, typeof(IFoo));

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo)));
			}
		}

		[TestMethod]
		public void RegisterInstanceUsingKeyReturnsCorrectType()
		{
			using (var container = new IocContainer())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IFoo), fooInstance, "Bar");

				Assert.IsInstanceOfType(result, typeof(InstanceRegistration<IFoo>));

				Assert.AreSame(result.ReturnType, typeof(IFoo));

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo), "Bar"));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterInstanceThrowsExceptionIfInstanceNotOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				var fooInstance = new Foo1();
				var reg = container.RegisterInstance(typeof(IBar), fooInstance);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterInstanceUsingKeyThrowsExceptionIfInstanceNotOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IBar), fooInstance, "Foo");
			}
		}

		[TestMethod]
		public void RegisterInstanceCanRegisterAStruct()
		{
			// Struct / ValueType ?

			using (var container = new IocContainer())
			{
				int number = 32;

				var reg1 = container.RegisterInstance(typeof(int), number);
				var reg2 = container.RegisterInstance(number, "key");

				// Check registrations
				Assert.IsInstanceOfType(reg1, typeof(InstanceRegistration<int>));
				Assert.IsInstanceOfType(reg2, typeof(InstanceRegistration<int>));

				Assert.AreSame(reg1.ReturnType, typeof(int));
				Assert.AreSame(reg2.ReturnType, typeof(int));

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(int)));
				Assert.IsTrue(container.Index.Contains(typeof(int), "key"));

				// Try to resolve
				var result1 = (int)container.Resolve(typeof(int));
				var result2 = container.Resolve<int>("key");

				Assert.AreEqual(number, result1);
				Assert.AreEqual(number, result2);
				Assert.AreEqual(result1, result2);
			}
		}

		[TestMethod]
		public void RegisterInstanceGenericReturnsCorrectType()
		{
			using (var container = new IocContainer())
			{
				var fooInstance = new Foo1();
				var reg = container.RegisterInstance<IFoo>(fooInstance);

				Assert.IsInstanceOfType(reg, typeof(InstanceRegistration<IFoo>));

				Assert.AreSame(reg.ReturnType, typeof(IFoo));

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo)));
			}
		}

		[TestMethod]
		public void RegisterInstanceGenericUsingKeyReturnsCorrectType()
		{
			using (var container = new IocContainer())
			{
				var fooInstance = new Foo1();
				var reg = container.RegisterInstance<IFoo>(fooInstance, "Bar");

				Assert.IsInstanceOfType(reg, typeof(InstanceRegistration<IFoo>));

				Assert.AreSame(reg.ReturnType, typeof(IFoo));

				// Check index
				Assert.IsTrue(container.Index.Contains(typeof(IFoo), "Bar"));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterInstanceGenericUsingTypeThatAlreadyExistsThrowsException()
		{
			using (var container = new IocContainer())
			{
				container.RegisterInstance<IFoo>(new Foo1());
				container.RegisterInstance<IFoo>(new Foo2());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterInstanceGenericUsingKeyThatAlreadyExistsThrowsException()
		{
			using (var container = new IocContainer())
			{
				container.RegisterInstance<IFoo>(new Foo1(), "Bar");
				container.RegisterInstance<IFoo>(new Foo2(), "Bar");
			}
		}
	}
}
