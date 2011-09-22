using System.Web;
using System.Web.SessionState;
using Dynamo.Ioc.Tests;
using Dynamo.Testing.Mocks.Web2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Web.Tests
{
	[TestClass]
	public class SessionLifetimeTest
	{
		/// <summary>
		/// Make sure Default Lifetime can be set to SessionLifetime
		///</summary>
		[TestMethod]
		public void CanSetDefaultLifetimeToSessionLifetime()
		{
			using (var container = new IocContainer(() => new SessionLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetime, typeof(SessionLifetime));
			}
		}

		/// <summary>
		/// Make sure Session Lifetime returns the same instance for the same session, and different from different session
		/// </summary>
		[TestMethod]
		public void SessionLifetimeReturnsSameInstanceForSameSessionAndDifferentInstanceForDifferentSession()
		{
			// Arrange
			using (var container = new IocContainer())
			{
				var sessionItems1 = new SessionStateItemCollection();
				var sessionItems2 = new SessionStateItemCollection();
				var context1 = new FakeHttpContext("Http://fakeUrl1.com", null, null, null, null, sessionItems1);
				var context2 = new FakeHttpContext("Http://fakeUrl2.com", null, null, null, null, sessionItems2);

				HttpContextBase currentContext = null;

				var lifetime = new SessionLifetime(() => currentContext);		// better solution for injecting HttpContext ?

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
	}
}