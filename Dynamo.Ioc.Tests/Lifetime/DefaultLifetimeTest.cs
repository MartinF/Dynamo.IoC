using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.LifetimeTests
{
	[TestClass]
	public class DefaultLifetimeTest
	{
		/// <summary>
		/// Make sure the Default Lifetime is TransientLifetime
		///</summary>
		[TestMethod]
		public void DefaultLifetimeIsDefaultSetToTransient()
		{
			using (var container = new Container())
			{
				Assert.IsInstanceOfType(container.DefaultLifetime, typeof(TransientLifetime));
			}
		}

		/// <summary>
		/// Resolving with Default Lifetime (Transient) returns a new instance
		///</summary>
		[TestMethod]
		public void DefaultLifetimeAlwaysReturnsNewInstance()
		{
			// Arrange 
			using (var container = new Container())
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
			}
		}
	}
}
