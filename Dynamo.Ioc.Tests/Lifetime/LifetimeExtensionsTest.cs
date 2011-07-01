using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Lifetime
{
	[TestClass]
	public class LifetimeExtensionsTest
	{
		[TestMethod]
		public void TransientLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new Container(() => new ContainerLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).TransientLifetime();

				Assert.IsTrue(registration.Lifetime is TransientLifetime);
			}
		}

		[TestMethod]
		public void ContainerLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new Container(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).ContainerLifetime();

				Assert.IsTrue(registration.Lifetime is ContainerLifetime);
			}
		}

		[TestMethod]
		public void ThreadLocalLifetimeExtensionMethodSetsLifetime()
		{
			using (var container = new Container(() => new TransientLifetime()))
			{
				var registration = container.Register<IFoo>(c => new Foo1()).ThreadLocalLifetime();

				Assert.IsTrue(registration.Lifetime is ThreadLocalLifetime);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TransientLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeExtensions.TransientLifetime(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ContainerLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeExtensions.ContainerLifetime(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThreadLocalLifetimeExtensionMethodThrowsExceptionIfParameterIsNull()
		{
			LifetimeExtensions.ThreadLocalLifetime(null);
		}
	}
}
