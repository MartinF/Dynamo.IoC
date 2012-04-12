using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class GetAllTest
	{
		[TestMethod]
		public void GetAllReturnsExpectedRegistrations()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1());
			var reg3 = new InstanceRegistration<IFoo>(new Foo1());
			var reg4 = new InstanceRegistration<IBar>(new Bar1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2, "Key1");
				index.Add(reg3, "Key2");
				index.Add(reg4, "Key1");

				var all = index.GetAll(typeof(IFoo));

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
		public void GetAllGenericReturnsExpectedRegistrations()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1());
			var reg3 = new InstanceRegistration<IFoo>(new Foo1());
			var reg4 = new InstanceRegistration<IBar>(new Bar1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2, "Key1");
				index.Add(reg3, "Key2");
				index.Add(reg4, "Key1");

				var all = index.GetAll<IFoo>();

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
		public void GetAllThrowsExceptionIfTypeIsNotRegistered()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2, "Key");

				try
				{
					var result = index.GetAll(typeof(IBar));

					// Doesnt throw exception before it is enumerated because it uses yield return - OK ?
					var test = result.Count();

					Assert.IsTrue(false);
				}
				catch (KeyNotFoundException)
				{
				}
			}
		}

		[TestMethod]
		public void GetAllGenericThrowsExceptionIfTypeIsNotRegistered()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2, "Key");

				try
				{
					var result = index.GetAll<IBar>();

					// Doesnt throw exception before it is enumerated because it uses yield return - OK ?
					var test = result.Count();

					Assert.IsTrue(false);
				}
				catch (KeyNotFoundException)
				{
				}
			}
		}
	}
}
