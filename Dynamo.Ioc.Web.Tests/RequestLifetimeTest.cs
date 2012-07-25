using System;
using System.Web;
using Dynamo.Ioc.Tests;
using Dynamo.Ioc.Web.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Web.Tests
{
	[TestClass]
	public class RequestLifetimeTest
	{
		/// <summary>
		/// Make sure Default Lifetime can be set to RequestLifetime
		///</summary>
		[TestMethod]
		public void CanSetDefaultLifetimeToRequestLifetime()
		{
			using (var container = new IocContainer(() => new RequestLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetimeFactory(), typeof(RequestLifetime));
			}
		}

		/// <summary>
		/// Make sure Request Lifetime returns the same instance for the same request, and different from different request
		/// </summary>
		[TestMethod]
		public void RequestLifetimeReturnsSameInstanceForSameRequest()
		{
			// Arrange
			using (var container = new IocContainer())
			{
				var context1 = new FakeHttpContext("Http://fakeUrl1.com");
				var context2 = new FakeHttpContext("Http://fakeUrl2.com");

				HttpContextBase currentContext = null;

				var lifetime = new RequestLifetime(() => currentContext);		// better solution for injecting HttpContext ?

				container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

				// Act
				currentContext = context1;

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IFoo>();

				currentContext = context2;

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
		[ExpectedException(typeof(InvalidCastException))]
		public void RequestLifetimeWithDisposeOnEndThrowsExceptionIfInstanceIsNotIDisposable()
		{
			// What exception should it throw?
			// Type Exception of some sort ?

			using (var container = new IocContainer())
			{
				// Arrange
				var context = new FakeHttpContext("Http://fakeUrl1.com");
				var lifetime = new RequestLifetime(() => context, disposeOnEnd: true);

				// Should throw exception here
				container.Register<IFoo>(c => new Foo1()).SetLifetime(lifetime);

				// But currently first throws exception here
				var instance = container.Resolve<IFoo>();
			}
		}

		[TestMethod]
		public void RequestLifetimeWorksWithCompile()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var context = new FakeHttpContext("Http://fakeUrl1.com");
				var requestLifetime = new RequestLifetime(() => context, disposeOnEnd: false);	// Do not test disposeOnEnd here

				container.Register<IFoo, Foo1>();
				container.Register<IBar, Bar1>().SetLifetime(requestLifetime);
				container.Register<IFooBar, FooBar>();

				// Act
				container.Compile();

				var fooBar = container.Resolve<IFooBar>();

				// Assert
				Assert.IsNotNull(fooBar);
				Assert.IsNotNull(fooBar.Foo);
				Assert.IsNotNull(fooBar.Bar);
			}
		}

		[TestMethod]
		public void RequestLifetimeNestedWorksWithCompile()
		{
			// This doesnt work - will affect other Lifetimes that just nests the expression 
			// Need to fix the scope of the input parameter IResolver

			using (var container = new IocContainer())
			{
				// Arrange
				var context = new FakeHttpContext("Http://fakeUrl1.com");
				var requestLifetime = new RequestLifetime(() => context, disposeOnEnd: false);	// Do not test disposeOnEnd here

				container.Register<IFoo, Foo1>();
				container.Register<IBar, Bar1>().SetLifetime(requestLifetime);
				container.Register<IFooBar, FooBar>();
				container.Register<IFooBarContainer, FooBarContainer>();

				// Act
				container.Compile();

				var fooBarContainer = container.Resolve<IFooBarContainer>();

				// Assert
				Assert.IsNotNull(fooBarContainer);
				Assert.IsNotNull(fooBarContainer.FooBar);
				Assert.IsNotNull(fooBarContainer.FooBar.Foo);
				Assert.IsNotNull(fooBarContainer.FooBar.Bar);
			}
		}
	}
}