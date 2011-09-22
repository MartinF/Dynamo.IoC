using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Problem !?
// Because they use yield they are lazy and first resolve when enumerated ? 
// So they could actually call ResolveAll<IFoo>() and not throw an exception before later.

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class ResolveAllTest
	{
		[TestMethod]
		public void ResolveAllByTypeReturnsInstancesOfExpectedType()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFoo>("Foo1", c => new Foo2());
				container.Register<IFoo>("Foo2", c => new Foo2());
				container.Register<IBar>(c => new Bar1());

				var results = container.ResolveAll<IFoo>();

				Assert.IsTrue(results.Count() == 3);

				var resultsList = results.ToList();

				CollectionAssert.AllItemsAreNotNull(resultsList);
				CollectionAssert.AllItemsAreInstancesOfType(resultsList, typeof(IFoo));
				CollectionAssert.AllItemsAreUnique(resultsList);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void ResolveAllByTypeNotRegisteredThrowsException()
		{
			using (var container = new IocContainer())
			{
				var results = container.ResolveAll<IFoo>();

				// Doesnt throw exception before it is enumerated because it uses yield return - OK ?
				var test = results.Count();
			}
		}
	}
}
