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
				var registration = container.Register<IFoo>(c => new Foo1()).WithRequestLifetime();

				Assert.IsTrue(registration.Lifetime is RequestLifetime);
			}
		}

		[TestMethod]
		public void SessionLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).WithSessionLifetime();

				Assert.IsTrue(registration.Lifetime is SessionLifetime);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RequestLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeRegistrationExtensions.WithRequestLifetime<ILifetimeRegistration>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SessionLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeRegistrationExtensions.WithSessionLifetime<ILifetimeRegistration>(null);
		}
	}
}
