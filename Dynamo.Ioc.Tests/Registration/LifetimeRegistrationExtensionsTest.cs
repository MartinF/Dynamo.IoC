using System;
using System.Runtime.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Registration
{
	[TestClass]
	public class LifetimeRegistrationExtensionsTest
	{
		[TestMethod]
		public void SetLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).SetLifetime(new ThreadLocalLifetime());

				Assert.IsTrue(registration.Lifetime is ThreadLocalLifetime);
			}
		}

		[TestMethod]
		public void TransientLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).WithTransientLifetime();

				Assert.IsTrue(registration.Lifetime is TransientLifetime);
			}
		}

		[TestMethod]
		public void ContainerLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).WithContainerLifetime();

				Assert.IsTrue(registration.Lifetime is ContainerLifetime);
			}
		}

		[TestMethod]
		public void ThreadLocalLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).WithThreadLocalLifetime();

				Assert.IsTrue(registration.Lifetime is ThreadLocalLifetime);
			}
		}

		[TestMethod]
		public void CachedLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).WithCachedLifetime(new CacheItemPolicy());

				Assert.IsTrue(registration.Lifetime is CachedLifetime);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeRegistrationExtensions.SetLifetime<ILifetimeRegistration>(null, new TransientLifetime());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TransientLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeRegistrationExtensions.WithTransientLifetime<ILifetimeRegistration>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContainerLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeRegistrationExtensions.WithContainerLifetime<ILifetimeRegistration>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThreadLocalLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeRegistrationExtensions.WithThreadLocalLifetime<ILifetimeRegistration>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CachedLifetimeExtensionMethodThrowsExceptionIfRegistrationParameterIsNull()
		{
			var policy = new CacheItemPolicy();
			LifetimeRegistrationExtensions.WithCachedLifetime<ILifetimeRegistration>(null, policy);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CachedLifetimeExtensionMethodThrowsExceptionIfPolicyParameterIsNull()
		{
			var container = new IocContainer();
			var reg = new ExpressionRegistration<IFoo>(container, resolver => new Foo1(), new TransientLifetime());
			LifetimeRegistrationExtensions.WithCachedLifetime(reg, null);
		}
	}
}
