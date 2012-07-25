using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Registration
{
	[TestClass]
	public class InstanceRegistrationTest
	{
		[TestMethod]
		public void InstanceRegistrationWorksLikeExpected()
		{
			var container = new IocContainer();

			var instance = new Foo1();
			var reg = new InstanceRegistration<IFoo>(instance);

			Assert.AreEqual(reg.ReturnType, typeof(IFoo));

			var out1 = reg.GetInstance();

			Assert.AreSame(instance, out1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InstanceRegistrationThrowsExceptionIfInstanceIsNull()
		{
			var reg = new InstanceRegistration<IFoo>(null);
		}
	}
}
