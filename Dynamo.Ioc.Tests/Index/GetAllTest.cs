using System;
using System.Text;
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
			using (var container = new IocContainer())
			{
				var foo1 = container.Register(typeof(IFoo), c => new Foo1());
				var foo2 = container.Register(typeof(IFoo), "Bob", c => new Foo2());
				var foo3 = container.Register(typeof(IFoo), "Bill", c => new Foo2());
				var bar1 = container.Register(typeof(IBar), "Jane", c => new Bar1());

				var result = container.Index.GetAll(typeof(IFoo));

				Assert.IsInstanceOfType(result, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(result.Count() == 3);

				var resultList = result.ToList();

				CollectionAssert.AllItemsAreNotNull(resultList);
				CollectionAssert.AllItemsAreInstancesOfType(resultList, typeof(IRegistration));
				CollectionAssert.Contains(resultList, foo1);
				CollectionAssert.Contains(resultList, foo2);
				CollectionAssert.Contains(resultList, foo3);
				CollectionAssert.DoesNotContain(resultList, bar1);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetAllThrowsExceptionIfTypeIsNotRegistered()
		{
			using (var container = new IocContainer())
			{
				var foo1 = container.Register<IFoo>(c => new Foo1());
				var foo2 = container.Register<IFoo>("Foo", c => new Foo2());

				var result = container.Index.GetAll(typeof(IBar));

				// Doesnt throw exception before it is enumerated because it uses yield return - OK ?
				var test = result.Count();
			}
		}



		#region GetAll Generic - IndexAccessorExtensions
		[TestMethod]
		public void GetAllGenericReturnsExpectedRegistrations()
		{
			using (var container = new IocContainer())
			{
				var foo1 = container.Register<IFoo>(c => new Foo1());
				var foo2 = container.Register<IFoo>("Bob", c => new Foo2());
				var foo3 = container.Register<IFoo>("Bill", c => new Foo2());
				var bar1 = container.Register<IBar>("Jane", c => new Bar1());

				var result = container.Index.GetAll<IFoo>();

				Assert.IsInstanceOfType(result, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(result.Count() == 3);

				var resultList = result.ToList();

				CollectionAssert.AllItemsAreNotNull(resultList);
				CollectionAssert.AllItemsAreInstancesOfType(resultList, typeof(IRegistration));
				CollectionAssert.Contains(resultList, foo1);
				CollectionAssert.Contains(resultList, foo2);
				CollectionAssert.Contains(resultList, foo3);
				CollectionAssert.DoesNotContain(resultList, bar1);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetAllGenericThrowsExceptionIfTypeIsNotRegistered()
		{
			using (var container = new IocContainer())
			{
				var foo1 = container.Register<IFoo>(c => new Foo1());
				var foo2 = container.Register<IFoo>("Foo", c => new Foo2());

				var result = container.Index.GetAll<IBar>();

				// Doesnt throw exception before it is enumerated because it uses yield return - OK ?
				var test = result.Count();
			}
		}
		#endregion
	}
}
