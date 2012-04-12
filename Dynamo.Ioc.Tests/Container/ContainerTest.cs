using System;
using System.Linq;
using Dynamo.Ioc.Index;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// You can use the following additional attributes as you write your tests:
//
// Use ClassInitialize to run code before running the first test in the class
// [ClassInitialize()]
// public static void MyClassInitialize(TestContext testContext) { }
//
// Use ClassCleanup to run code after all tests in a class have run
// [ClassCleanup()]
// public static void MyClassCleanup() { }
//
// Use TestInitialize to run code before running each test 
// [TestInitialize()]
// public void MyTestInitialize() { }
//
// Use TestCleanup to run code after each test has run
// [TestCleanup()]
// public void MyTestCleanup() { }
//

namespace Dynamo.Ioc.Tests.Container
{
	[TestClass]
	public class ContainerTest
	{
		[TestMethod]
		public void ContainerCanBeCreated()
		{
			using (var container = new IocContainer())
			{
				Assert.IsNotNull(container);
			}
		}

		[TestMethod]
		public void ContainerIsCreatedWithCorrectDefaults()
		{
			using (var container = new IocContainer())
			{
				// Default Lifetime
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(TransientLifetime));

				// Default CompileMode
				Assert.IsTrue(container.DefaultCompileMode == CompileMode.Delegate);

				// Default Index
				Assert.IsInstanceOfType(container.Index, typeof(GroupedIndex));
			}
		}

		[TestMethod]
		public void ContainerCanSetDefaultsUsingTheConstructor()
		{
			Func<ILifetime> lifetimeFactory = () => new ContainerLifetime();
			var compileMode = CompileMode.Dynamic;
			var index = new DirectIndex();
			
			using (var container = new IocContainer(lifetimeFactory, compileMode, index))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(ContainerLifetime));
				Assert.IsTrue(compileMode == container.DefaultCompileMode);
				Assert.AreSame(index, container.Index);
			}
		}

		[TestMethod]
		public void ContainerReturnsEmptyEnumerableWhenEmpty()
		{
			using (var container = new IocContainer())
			{
				Assert.IsFalse(container.Index.Any());
			}
		}

		[TestMethod]
		public void ContainerIsEnumerable()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register<IFoo>(c => new Foo1());
				var reg2 = container.Register<IBar>(c => new Bar1());
				var reg3 = container.Register<IBar>(c => new Bar1(), "Bar");

				var registrations = container.Index.ToList();

				Assert.IsTrue(container.Index.Count() == 3);

				CollectionAssert.AllItemsAreNotNull(registrations);
				CollectionAssert.AllItemsAreUnique(registrations);

				CollectionAssert.Contains(registrations, reg1);
				CollectionAssert.Contains(registrations, reg2);
				CollectionAssert.Contains(registrations, reg3);
			}
		}









		// Where to put this ? tests the same as what is tested in ExpressionRegistration - just through the container ?

		[TestMethod]
		public void CanChangeLifetimeUsingSetLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(TransientLifetime));

				var registration = container.Register<IFoo>(c => new Foo1()).SetLifetime(new ContainerLifetime());

				Assert.IsInstanceOfType(registration.Lifetime, typeof(ContainerLifetime));

				var foo1 = container.Resolve<IFoo>();
				var foo2 = container.Resolve<IFoo>();

				Assert.AreSame(foo1, foo2);
			}
		}



		// Isnt this in the RegisterTest already ? MOVE ?


		[TestMethod]
		public void CanRegisterAndResolveMultipleTypes()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(c => new Bar1());

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IBar>();

				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);

				Assert.IsInstanceOfType(result1, typeof(Foo1));
				Assert.IsInstanceOfType(result2, typeof(Bar1));
			}
		}

		[TestMethod]
		public void ContainerUsesTheDefaultLifetimeSetWhenRegistering()	// RegisterUsesTheDefaultLifetime ?
		{
			// Make sure the set lifetime is not the default - if changed in the future !?
			// Create a default IocContainer and check that the default lifetime is not ContainerLifetime?

			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				var defaultLifetime = container.DefaultLifetimeFactory();

				// Default Lifetime
				Assert.IsInstanceOfType(defaultLifetime, typeof(ContainerLifetime));

				var result1 = container.Register<IFoo>(c => new Foo1());
				var result2 = container.Register<IBar>(c => new Bar1());

				Assert.IsInstanceOfType(result1.Lifetime, typeof(ContainerLifetime));
				Assert.IsInstanceOfType(result2.Lifetime, typeof(ContainerLifetime));
			}
		}
	}
}
