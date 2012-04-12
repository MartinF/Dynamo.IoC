using System;
using Dynamo.Ioc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Web.Tests
{
	[TestClass]
	public class LifetimeExtensionsTest
	{
		[TestMethod]
		public void RequestLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).RequestLifetime();

				Assert.IsTrue(registration.Lifetime is RequestLifetime);
			}
		}

		[TestMethod]
		public void SessionLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).SessionLifetime();

				Assert.IsTrue(registration.Lifetime is SessionLifetime);
			}
		}

		[TestMethod]
		public void CachedLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).CachedLifetime();

				Assert.IsTrue(registration.Lifetime is CachedLifetime);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RequestLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeExtensions.RequestLifetime(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SessionLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeExtensions.SessionLifetime(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CachedLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeExtensions.CachedLifetime(null);
		}
	}
}
