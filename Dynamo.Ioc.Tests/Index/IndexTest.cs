using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class IndexTest
	{
		[TestMethod]
		public void IndexReturnsEmptyEnumerableWhenEmpty()
		{
			using (var container = new IocContainer())
			{
				Assert.IsFalse(container.Index.Any());
			}
		}

		[TestMethod]
		public void IndexIsEnumerable()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register<IFoo>(c => new Foo1());
				var reg2 = container.Register<IBar>(c => new Bar1());
				var reg3 = container.Register<IBar>("Bar", c => new Bar1());

				var registrations = container.Index.ToList();

				Assert.IsTrue(container.Index.Count() == 3);

				CollectionAssert.AllItemsAreNotNull(registrations);
				CollectionAssert.AllItemsAreUnique(registrations);

				CollectionAssert.Contains(registrations, reg1);
				CollectionAssert.Contains(registrations, reg2);
				CollectionAssert.Contains(registrations, reg3);
			}
		}
	}
}
