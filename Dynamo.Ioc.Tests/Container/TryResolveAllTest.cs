using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class TryResolveAllTest
	{
		[TestMethod]
		public void TryResolveAllReturnsExpectedInstances()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var foo1 = new Foo1();
				var foo2 = new Foo2();
				var foo3 = new Foo2();
				var bar1 = new Bar1();

				container.RegisterInstance<IFoo>(foo1);
				container.RegisterInstance<IFoo>("Foo1", foo2);
				container.RegisterInstance<IFoo>("Foo2", foo3);
				container.RegisterInstance<IBar>(bar1);

				// Act
				var results = container.TryResolveAll<IFoo>();

				var resultList = results.ToList();

				// Assert
				Assert.IsTrue(results.Count() == 3);

				CollectionAssert.Contains(resultList, foo1);
				CollectionAssert.Contains(resultList, foo2);
				CollectionAssert.Contains(resultList, foo3);
			}
		}

		[TestMethod]
		public void TryResolveAllByTypeNotRegisteredReturnsEmptyEnumerable()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var foo1 = new Foo1();
				var foo2 = new Foo2();
				var bar1 = new Bar1();

				container.RegisterInstance<IFoo>(foo1);
				container.RegisterInstance<IFoo>("Foo", foo2);
				container.RegisterInstance<IBar>(bar1);

				// Act
				var results = container.TryResolveAll<IFooBar>();

				// Assert
				Assert.IsTrue(results.Count() == 0);
			}
		}
	}
}
