using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Create default configuration and test it instead of configuring every time ?

namespace Dynamo.Ioc.Tests.Container
{
	[TestClass]
	public class ResolveTest
	{
		[TestMethod]
		public void ResolveGenericReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				var result = container.Resolve<IFoo>();

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void ResolveGenericUsingKeyReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "TheKey");
				var result = container.Resolve<IFoo>("TheKey");

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void ResolveReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				var result = container.Resolve(typeof(IFoo));

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void ResolveUsingKeyReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "TheKey");
				var result = container.Resolve(typeof(IFoo), "TheKey");

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}
	
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void ResolveGenericThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "Key");
				container.Resolve<IFoo>();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void ResolveGenericUsingKeyThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Resolve<IFoo>("Key");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void ResolveThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "Key");
				container.Resolve(typeof(IFoo));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void ResolveUsingKeyThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Resolve(typeof(IFoo), "Key");
			}
		}





		// Should be in register - tests if registering with or without key works like it should ?

		// Or keep here and only test Register method and exceptions in Register ?

		[TestMethod]
		public void RegistrationsWithDifferentNameResolveToDifferentTypes()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1(), "Foo1");
				container.Register<IFoo>(c => new Foo2(), "Foo2");

				var result1 = container.Resolve<IFoo>("Foo1");
				var result2 = container.Resolve<IFoo>("Foo2");

				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);

				Assert.IsInstanceOfType(result1, typeof(Foo1));
				Assert.IsInstanceOfType(result2, typeof(Foo2));

				Assert.AreNotSame(result1, result2);
			}
		}

		[TestMethod]
		public void RegistrationsRegisteredByTypeOrNameResolvesToDifferentTypes()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFoo>(c => new Foo2(), "Foo2");

				// Act
				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>("Foo2");

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);

				Assert.IsInstanceOfType(result1, typeof(Foo1));
				Assert.IsInstanceOfType(result2, typeof(Foo2));

				Assert.AreNotSame(result1, result2);
			}
		}

		[TestMethod]
		public void MultipleResolvesReturnDifferentInstances()
		{
			// Belongs to the lifetime?

			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);

				Assert.AreNotSame(result1, result2);
			}
		}



		// Should be part of Register tests instead ?

		[TestMethod]
		public void ResolveGenericCanUseFunc()
		{
			using (var container = new IocContainer(defaultCompileMode: CompileMode.Delegate))
			{
				Func<IResolver, IFoo> func = x =>
				{
					string what = "";
					return new Foo1();
				};

				container.Register<IFoo>(x => func(x));
				var result = container.Resolve<IFoo>();

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void ResolveGenericCanUseMethod()
		{
			using (var container = new IocContainer(defaultCompileMode: CompileMode.Delegate))
			{
				container.Register<IFoo>(x => Fixed());
				var result = container.Resolve<IFoo>();

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		private IFoo Fixed()
		{
			var what = "";
			return new Foo1();
		}
	}
}
