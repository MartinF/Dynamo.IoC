using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Create default configuration and test it instead of configuring every time - DRY

namespace Dynamo.Ioc.Tests
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
		public void ResolveCanResolveCollection()
		{
			var foo1 = new Foo1();
			var foo2 = new Foo2();
			var foo3 = new Foo1();
			IList<IFoo> list = new List<IFoo>(new IFoo[] { foo1, foo2, foo3 });
			
			// Register Instance
			using (var container = new IocContainer())
			{
				container.RegisterInstance(typeof(IEnumerable<IFoo>), list);
				container.RegisterInstance(typeof(IEnumerable<IFoo>), list, "TheKey");

				AssertResolveCanResolveIEnumerableType(container);
				AssertResolveCanResolveIEnumerableType(container, "TheKey");
			}

			// RegisterInstance - Generic
			using (var container = new IocContainer())
			{
				container.RegisterInstance<IEnumerable<IFoo>>(list);
				container.RegisterInstance<IEnumerable<IFoo>>(list, "TheKey");

				AssertResolveCanResolveIEnumerableType(container);
				AssertResolveCanResolveIEnumerableType(container, "TheKey");
			}

			// Register
			using (var container = new IocContainer())
			{
				container.Register<IEnumerable<IFoo>>(x => list);
				container.Register<IEnumerable<IFoo>>(x => list, "TheKey");

				AssertResolveCanResolveIEnumerableType(container);
				AssertResolveCanResolveIEnumerableType(container, "TheKey");
			}
		}
		
		private void AssertResolveCanResolveIEnumerableType(IResolver resolver, string key = null)
		{
			// Test both generic and non - generic ? 

			var result = key == null ? resolver.Resolve<IEnumerable<IFoo>>() : resolver.Resolve<IEnumerable<IFoo>>(key);

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(IEnumerable<IFoo>));

			var collection = result.ToArray();

			CollectionAssert.AllItemsAreNotNull(collection);
			CollectionAssert.AllItemsAreUnique(collection);
			CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(IFoo));
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

		[TestMethod]
		public void ResolveByTypeOrKeyResolvesToDifferentTypes()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFoo>(c => new Foo2(), "Foo1");
				container.Register<IFoo>(c => new Foo2(), "Foo2");
				container.Register<IBar>(c => new Bar1(), "Foo1");	// same key but different type

				// Act
				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>("Foo1");
				var result3 = container.Resolve<IFoo>("Foo2");
				var result4 = container.Resolve<IBar>("Foo1");

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);
				Assert.IsNotNull(result4);

				Assert.IsInstanceOfType(result1, typeof(Foo1));
				Assert.IsInstanceOfType(result2, typeof(Foo2));
				Assert.IsInstanceOfType(result3, typeof(Foo2));
				Assert.IsInstanceOfType(result4, typeof(Bar1));

				Assert.AreNotSame(result1, result2);
				Assert.AreNotSame(result1, result3);
				Assert.AreNotSame(result1, result4);
				
				Assert.AreNotSame(result2, result3);
				Assert.AreNotSame(result2, result4);

				Assert.AreNotSame(result3, result4);
			}
		}

		[TestMethod]
		public void ResolveGenericCanUseFunc()
		{
			using (var container = new IocContainer(defaultCompileMode: CompileMode.Delegate))
			{
				Func<IResolver, IFoo> func = x =>
				{
					string what = ""; // do something
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
			return new Foo1();
		}
	}
}
