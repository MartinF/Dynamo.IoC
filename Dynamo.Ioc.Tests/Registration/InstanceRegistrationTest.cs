using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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

			Assert.AreEqual(reg.ImplementationType, typeof(Foo1));
			Assert.AreEqual(reg.ReturnType, typeof(IFoo));

			var out1 = reg.GetInstance(container);

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
