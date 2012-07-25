using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class TryGetAllTest
	{
		[TestMethod]
		public void TryGetAllReturnsExpectedRegistrations()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1(), "Key1");
			var reg3 = new InstanceRegistration<IFoo>(new Foo1(), "Key2");
			var reg4 = new InstanceRegistration<IBar>(new Bar1(), "Key1");

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);
				index.Add(reg3);
				index.Add(reg4);

				var all = index.TryGetAll(typeof(IFoo));

				Assert.IsInstanceOfType(all, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(all.Count() == 3);

				var allList = all.ToList();

				CollectionAssert.AllItemsAreNotNull(allList);
				CollectionAssert.AllItemsAreInstancesOfType(allList, typeof(InstanceRegistration<IFoo>));
				CollectionAssert.Contains(allList, reg1);
				CollectionAssert.Contains(allList, reg2);
				CollectionAssert.Contains(allList, reg3);
				CollectionAssert.DoesNotContain(allList, reg4);
			}
		}

		[TestMethod]
		public void TryGetAllGenericReturnsExpectedRegistrations()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1(), "Key1");
			var reg3 = new InstanceRegistration<IFoo>(new Foo1(), "Key2");
			var reg4 = new InstanceRegistration<IBar>(new Bar1(), "Key1");

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);
				index.Add(reg3);
				index.Add(reg4);

				var all = index.TryGetAll<IFoo>();

				Assert.IsInstanceOfType(all, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(all.Count() == 3);

				var allList = all.ToList();

				CollectionAssert.AllItemsAreNotNull(allList);
				CollectionAssert.AllItemsAreInstancesOfType(allList, typeof(InstanceRegistration<IFoo>));
				CollectionAssert.Contains(allList, reg1);
				CollectionAssert.Contains(allList, reg2);
				CollectionAssert.Contains(allList, reg3);
				CollectionAssert.DoesNotContain(allList, reg4);
			}
		}

		[TestMethod]
		public void TryGetAllThrowsExceptionIfTypeIsNotRegistered()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1(), "Key");

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);

				var result = index.TryGetAll(typeof(IBar));

				// Assert
				Assert.IsInstanceOfType(result, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(result.Count() == 0);
			}
		}

		[TestMethod]
		public void TryGetAllGenericThrowsExceptionIfTypeIsNotRegistered()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1(), "Key");

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);

				var result = index.TryGetAll<IBar>();

				// Assert
				Assert.IsInstanceOfType(result, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(result.Count() == 0);
			}
		}
	}
}
