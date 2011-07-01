using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class TryResolveTest
	{
		[TestMethod]
		public void TryResolveByTypeNotRegisteredReturnsNull()
		{
			using (var container = new Container())
			{
				// Act
				IFoo obj;
				bool result = container.TryResolve<IFoo>(out obj);

				// Assert
				Assert.IsFalse(result);
				Assert.IsNull(obj);
			}
		}

		[TestMethod]
		public void TryResolveByNameNotRegisteredReturnsNull()
		{
			using (var container = new Container())
			{
				// Arrange
				container.Register<IFoo>("Foo", c => new Foo1());

				// Act
				IFoo obj1;
				IFoo obj2;
				var result1 = container.TryResolve<IFoo>(out obj1);
				var result2 = container.TryResolve<IFoo>("Wrong", out obj2);

				// Assert
				Assert.IsFalse(result1);
				Assert.IsFalse(result2);
				Assert.IsNull(obj1);
				Assert.IsNull(obj2);
			}
		}

		[TestMethod]
		public void TryResolveByTypeReturnsExpectedInstance()
		{
			using (var container = new Container())
			{
				// Arrange
				container.Register<IFoo>(x => new Foo1());

				// Act
				IFoo obj;
				var result = container.TryResolve<IFoo>(out obj);

				// Assert
				Assert.IsInstanceOfType(obj, typeof(Foo1));
				Assert.IsTrue(result);
			}
		}

		[TestMethod]
		public void TryResolveByNameReturnsExpectedInstance()
		{
			using (var container = new Container())
			{
				// Arrange
				var foo = new Foo1();
				container.RegisterInstance<IFoo>("Foo", foo);

				// Act
				IFoo obj;
				var result = container.TryResolve<IFoo>("Foo", out obj);

				// Assert
				Assert.AreSame(obj, foo);
				Assert.IsTrue(result);
			}
		}
	}
}
