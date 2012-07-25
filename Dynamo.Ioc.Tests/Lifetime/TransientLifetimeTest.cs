using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Lifetime
{
	[TestClass]
	public class TransientLifetimeTest
	{
		[TestMethod]
		public void TransientLifetimeAlwaysReturnsANewInstance()
		{
			// Arrange 
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1()).WithTransientLifetime();

				// Act
				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();
				var result3 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreNotSame(result1, result2);
				Assert.AreNotSame(result2, result3);
				Assert.AreNotSame(result1, result3);

				Assert.IsInstanceOfType(result1, typeof(Foo1));
				Assert.IsInstanceOfType(result2, typeof(Foo1));
				Assert.IsInstanceOfType(result3, typeof(Foo1));
			}
		}
	}
}