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
			foreach (var index in Helper.GetIndexes())
			{
				Assert.IsFalse(index.Any());		
			}
		}

		[TestMethod]
		public void IndexIsEnumerable()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IBar>(new Bar1());
			var reg3 = new InstanceRegistration<IBar>(new Bar1(), "Key");
			
			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);
				index.Add(reg3);
				
				Assert.IsTrue(index.Count() == 3);

				var registrations = index.ToList();

				CollectionAssert.AllItemsAreNotNull(registrations);
				CollectionAssert.AllItemsAreUnique(registrations);

				CollectionAssert.Contains(registrations, reg1);
				CollectionAssert.Contains(registrations, reg2);
				CollectionAssert.Contains(registrations, reg3);
			}
		}
	}
}
