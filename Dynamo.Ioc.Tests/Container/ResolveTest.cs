using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class ResolveTest
	{
		[TestMethod]
		public void GenericResolveByTypeReturnsInstanceOfExpectedType()
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
		public void GenericResolveByNameReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>("Bob", c => new Foo1());
				var result = container.Resolve<IFoo>("Bob");

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void ResolveByTypeReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), c => new Foo1());
				var result = container.Resolve<IFoo>();

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void ResolveByNameReturnsInstanceOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), "Bob", c => new Foo1());
				var result = container.Resolve<IFoo>("Bob");

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IFoo));
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GenericResolveByTypeNotRegisteredThrowsException()
		{
			using (var container = new IocContainer())
			{
				container.Resolve<IFoo>();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GenericResolveByNameNotRegisteredThrowsException()
		{
			using (var container = new IocContainer())
			{
				container.Resolve<IFoo>("Foo");
			}
		}
	
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GenericResolveByTypeNotRegisteredThrowsException2()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>("Foo", c => new Foo1());
				container.Resolve<IFoo>();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GenericResolveByNameNotRegisteredThrowsException2()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Resolve<IFoo>("Foo");
			}
		}

		[TestMethod]
		public void MultipleResolvesReturnDifferentInstances()
		{
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

		[TestMethod]
		public void RegistrationsWithDifferentNameResolveToDifferentTypes()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>("Foo1", c => new Foo1());
				container.Register<IFoo>("Foo2", c => new Foo2());

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
				container.Register<IFoo>("Foo2", c => new Foo2());

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
	}
}
