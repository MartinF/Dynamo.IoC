using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class Add
	{
		[TestMethod]
		public void AddWorksLikeExpected()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IBar>(new Bar1());
			var reg3 = new InstanceRegistration<IBar>(new Bar1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);
				index.Add(reg3, "Key");

				Assert.IsTrue(index.Count() == 3);

				// Really dont need to include rest of the checks - covered by other tests - nothing to do with add

				var registrations = index.ToList();

				CollectionAssert.AllItemsAreNotNull(registrations);
				CollectionAssert.AllItemsAreUnique(registrations);

				CollectionAssert.Contains(registrations, reg1);
				CollectionAssert.Contains(registrations, reg2);
				CollectionAssert.Contains(registrations, reg3);

				var out1 = index.Get(typeof(IFoo));
				var out2 = index.Get(typeof(IBar));
				var out3 = index.Get(typeof(IBar), "Key");

				Assert.AreSame(out1, reg1);
				Assert.AreSame(out2, reg2);
				Assert.AreSame(out3, reg3);
			}
		}

		[TestMethod]
		public void AddThrowsExceptionWhenDuplicatesAreRegistered()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);

				try
				{
					index.Add(reg2);
					Assert.IsTrue(false);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		[TestMethod]
		public void AddUsingKeyThrowsExceptionWhenDuplicatesAreRegistered()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1());

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1, "Key");

				try
				{
					index.Add(reg2, "Key");
					Assert.IsTrue(false);
				}
				catch (ArgumentException)
				{
				}
			}
		}
	}
}
