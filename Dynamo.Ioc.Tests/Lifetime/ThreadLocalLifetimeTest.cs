using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Lifetime
{
	[TestClass]
	public class ThreadLocalLifetimeTest
	{
		[TestMethod]
		public void CanSetDefaultLifetimeToThreadLocalStorageLifetime()
		{
			using (var container = new IocContainer(() => new ThreadLocalLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(ThreadLocalLifetime));
			}
		}

		[TestMethod]
		public void ThreadLocalStorageLifetimeReturnsSameInstanceForSameThread()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1()).SetLifetime(new ThreadLocalLifetime());

				IFoo result1 = container.Resolve<IFoo>();
				IFoo result2 = container.Resolve<IFoo>();

				IFoo result3 = null;
				IFoo result4 = null;

				// Resolve on a different thread
				var task = Task.Factory.StartNew(() =>
				{
					result3 = container.Resolve<IFoo>();
					result4 = container.Resolve<IFoo>();
				});

				task.Wait();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);
				Assert.IsNotNull(result4);

				Assert.AreSame(result1, result2);
				Assert.AreSame(result3, result4);

				Assert.AreNotSame(result1, result3);
			}
		}
	}
}