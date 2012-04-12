using System;
using System.IO;
using System.Threading;
using System.Web.Caching;
using Dynamo.Ioc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Web.Tests
{
	[TestClass]
	public class CachedLifetimeTest
	{
		[TestMethod]
		public void CachedLifetimeCanBeSetAsDefaultLifetime()
		{
			using (var container = new IocContainer(() => new CachedLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(CachedLifetime));
			}
		}

		[TestMethod]
		public void CachedLifetimeReturnsSameInstanceIfCacheNotExpired()
		{
			using (var container = new IocContainer())
			{
				var lifetime = new CachedLifetime(new TimeSpan(0, 0, 3));

				container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

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
				var reg = container.Register<IFoo>(c => new Foo1()).SetLifetime(new CachedLifetime());

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				// Simulate expiry - how ??????
				// Create Reset/Clear method on Lifetime ?

				var result3 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreSame(result1, result2);
				Assert.AreNotSame(result1, result3);
			}
		}

		//[TestMethod]
		//public void CachedLifetimeReturnsDifferentInstanceIfAbsoluteTimeoutHaveExpired()
		//{
		//    using (var container = new DynamoContainer())
		//    {
		//        var lifetime = new CachedLifetime().ExpiresOn(DateTime.UtcNow.AddSeconds(1));

		//        container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

		//        var result1 = container.Resolve<IFoo>();
		//        var result2 = container.Resolve<IFoo>();

		//        // Simulate expiry
		//        Thread.Sleep(3000);

		//        var result3 = container.Resolve<IFoo>();

		//        // Assert
		//        Assert.IsNotNull(result1);
		//        Assert.IsNotNull(result2);
		//        Assert.IsNotNull(result3);

		//        Assert.AreSame(result1, result2);
		//        Assert.AreNotSame(result1, result3);
		//    }
		//}

		[TestMethod]
		public void CachedLifetimeReturnsDifferentInstanceIfSlidingTimeoutHaveExpired()
		{
			using (var container = new IocContainer())
			{
				// Expires When not Accessed For More than a specific time periode.
				var lifetime = new CachedLifetime(new TimeSpan(0, 0, 1));

				container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				// Simulate expiry
				Thread.Sleep(2000);

				var result3 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreSame(result1, result2);
				Assert.AreNotSame(result1, result3);
			}
		}

		bool _itemRemoved;
		CacheItemRemovedReason _reason;
		public void RemovedCallback(String someString, Object someObject, CacheItemRemovedReason removeReason)
		{
			_itemRemoved = true;
			_reason = removeReason;
		}

		[TestMethod]
		public void CachedLifetimeCallbackIsCalledWhenItemRemovedFromCache()
		{
			using (var container = new IocContainer())
			{
				var lifetime = new CachedLifetime(new TimeSpan(0, 0, 1), itemRemovedCallback: RemovedCallback);

				container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

				_itemRemoved = false;
				var result1 = container.Resolve<IFoo>();

				// Simulate expiry
				Thread.Sleep(2000);
				var result2 = container.Resolve<IFoo>();

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
			
				Assert.AreNotSame(result1, result2);
				Assert.IsTrue(_itemRemoved);
				Assert.IsTrue(_reason == CacheItemRemovedReason.Expired);
			}
		}

		[TestMethod]
		public void CachedLifetimeIsDependentOnTest()
		{
			using (var container = new IocContainer())
			{
				var executionDirectory = Environment.CurrentDirectory;
				
				// Create a file for the cached item to be dependent on
				var filePath = executionDirectory + "\\DependencyFile.txt";

				if (File.Exists(filePath))
					File.Delete(filePath);

				var dependencyFile = File.CreateText(filePath);

				dependencyFile.WriteLine("This is a file that the cache item is dependent on.");
				dependencyFile.Close();

				var cacheDependency = new CacheDependency(filePath);

				var lifetime = new CachedLifetime(cacheDependency);

				container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				// Change the dependency file
				dependencyFile = File.AppendText(filePath);
				dependencyFile.WriteLine("Modified dependecy file.");
				dependencyFile.Close();

				// Need to give the system time to detect the change.
				Thread.Sleep(500);

				var result3 = container.Resolve<IFoo>();

				// cleanup
				if (File.Exists(filePath))
					File.Delete(filePath);

				// Assert
				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);
				Assert.IsNotNull(result3);

				Assert.AreSame(result1, result2);
				Assert.AreNotSame(result1, result3);
			}	
		}

		[TestMethod]
		public void CachedLifetimeWithPriorityTest()
		{
			// Hmm...
			Assert.IsTrue(false);
		}
	}
}
