using System;
using System.Linq;
using Dynamo.Ioc.Index;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// [ClassInitialize()] Use ClassInitialize to run code before running the first test in the class
// [ClassCleanup()] Use ClassCleanup to run code after all tests in a class have run
// [TestInitialize()] Use TestInitialize to run code before running each test 
// [TestCleanup()] Use TestCleanup to run code after each test has run

namespace Dynamo.Ioc.Tests
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
				Assert.IsInstanceOfType(container.Index, typeof(DirectIndex));
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

		[TestMethod]
		public void ContainerUsesTheDefaultLifetimeWhenRegistering()
		{
			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				var defaultLifetime = container.DefaultLifetimeFactory();

				// Default Lifetime
				Assert.IsInstanceOfType(defaultLifetime, typeof(ContainerLifetime));

				// Try registering with all Register methods using lifetime
				var result1 = container.Register<IFoo>(c => new Foo1());
				var result2 = container.Register<IFoo, Foo1>("Key1");
				var result3 = container.Register(typeof(IFoo), typeof(Foo1), "Key2");

				Assert.IsInstanceOfType(result1.Lifetime, typeof(ContainerLifetime));
				Assert.IsInstanceOfType(result2.Lifetime, typeof(ContainerLifetime));
				Assert.IsInstanceOfType(result3.Lifetime, typeof(ContainerLifetime));
			}
		}

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
	}
}
