using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class TryGetAllTest
	{
		[TestMethod]
		public void TryGetAllReturnsAnEmptyEnumerableIfTypeIsNotRegistered()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var foo1 = container.Register<IFoo>(c => new Foo1());
				var foo2 = container.Register<IFoo>("Foo", c => new Foo2());

				// Act
				var result = container.Index.TryGetAll(typeof(IBar));

				// Assert
				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(result.Count() == 0);
			}
		}



		#region TryGetAll Generic - IIndexAccessorExtensions
		[TestMethod]
		public void TryGetAllGenericReturnsAnEmptyEnumerableIfTypeIsNotRegistered()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var foo1 = container.Register<IFoo>(c => new Foo1());
				var foo2 = container.Register<IFoo>("Foo", c => new Foo2());

				// Act
				var result = container.Index.TryGetAll<IBar>();

				// Assert
				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(IEnumerable<IRegistration>));
				Assert.IsTrue(result.Count() == 0);
			}
		}
		#endregion
	}
}
