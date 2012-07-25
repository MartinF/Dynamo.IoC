using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;

namespace Dynamo.Ioc.Tests.Lifetime
{
	[TestClass]
	public class CachedLifetimeTest
	{
		[TestMethod]
		public void CachedLifetimeCanBeSetAsDefaultLifetime()
		{
			var policy = new CacheItemPolicy();

			using (var container = new IocContainer(() => new CachedLifetime(policy)))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(CachedLifetime));
			}
		}

		[TestMethod]
		public void CachedLifetimeReturnsSameInstanceIfCacheNotExpired()
		{
			using (var container = new IocContainer())
			{
				var policy = new CacheItemPolicy()
				             {
								 SlidingExpiration = new TimeSpan(0, 0, 3)
				             };

				container.Register<IFoo>(c => new Foo1()).WithCachedLifetime(policy);

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				Thread.Sleep(1000);

				var result3 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreSame(result1, result2);
				Assert.AreSame(result1, result3);
			}
		}

		[TestMethod]
		public void CachedLifetimeReturnsDifferentInstanceIfCacheExpired()
		{
			using (var container = new IocContainer())
			{
				var policy = new CacheItemPolicy()
				{
					SlidingExpiration = new TimeSpan(0, 0, 1)
				};

				container.Register<IFoo>(c => new Foo1()).WithCachedLifetime(policy);

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				Thread.Sleep(1500);

				var result3 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreSame(result1, result2);
				Assert.AreNotSame(result1, result3);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CachedLifetimeThrowsExceptionIfPolicyParameterIsNull()
		{
			CacheItemPolicy policy = null;
			new CachedLifetime(policy);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CachedLifetimeThrowsExceptionIfCacheParameterIsNull()
		{
			ObjectCache cache = null;
			CacheItemPolicy policy = new CacheItemPolicy();
			new CachedLifetime(cache, policy);
		}
	}
}
