using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.LifetimeTests
{
	[TestClass]
	public class TransientLifetimeTest
	{
		/// <summary>
		/// Make sure it is possible to set the Default Lifetime to Transient
		///</summary>
		[TestMethod]
		public void CanSetDefaultLifetimeToTransient()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetime, typeof(TransientLifetime));
			}
		}

		/// <summary>
		/// Resolving with Transient Lifetime returns a new instance
		///</summary>
		[TestMethod]
		public void AlwayNewLifetimeReturnsANewInstance()
		{
			// Arrange 
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				container.Register<IFoo>(c => new Foo1());

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