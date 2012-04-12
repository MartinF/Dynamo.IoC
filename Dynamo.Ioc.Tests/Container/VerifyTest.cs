using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Container
{
	[TestClass]
	public class VerifyTest
	{
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void VerifyThrowsExceptionWhenMissingRegistration()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFooBar>(y => new FooBar((IFoo)y.Resolve(typeof(IFoo)), (IBar)y.Resolve(typeof(IBar))));

				//container.Verify();
			}
		}
	}
}
