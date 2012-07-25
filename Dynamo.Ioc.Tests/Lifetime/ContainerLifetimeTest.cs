using Microsoft.VisualStudio.TestTools.UnitTesting;
	
namespace Dynamo.Ioc.Tests.Lifetime
{
	[TestClass]
	public class ContainerLifetimeTest
	{
		[TestMethod]
		public void CanSetDefaultLifetimeToContainerLifetime()
		{
			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(ContainerLifetime));
			}
		}

		[TestMethod]
		public void ContainerLifetimeAlwaysReturnsSameInstance()
		{
			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				container.Register<IFoo>(c => new Foo1());

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();
				var result3 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreSame(result1, result2);
				Assert.AreSame(result2, result3);
				Assert.AreSame(result1, result3);		
			}
		}
	}
}
