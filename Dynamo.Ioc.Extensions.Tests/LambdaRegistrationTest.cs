using System;
using Dynamo.Ioc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Extensions.Tests
{
	[TestClass]
	public class LambdaRegistrationTest
	{
		[TestMethod]
		public void LambdaRegistrationWorksLikeExpected()
		{
			var container = new IocContainer();
			var instance = new Foo1();

			var reg = new LambdaRegistration<IFoo>(container, x => { return instance; }, new TransientLifetime());

			Assert.AreEqual(reg.ReturnType, typeof(IFoo));

			var out1 = reg.GetInstance();

			Assert.AreSame(instance, out1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void LambdaRegistrationThrowsExceptionIfLambdaIsNull()
		{
			var container = new IocContainer();
			var reg = new LambdaRegistration<IFoo>(container, null, new TransientLifetime());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void LambdaRegistrationThrowsExceptionIfResolverIsNull()
		{
			var reg = new LambdaRegistration<IFoo>(null, (IResolver) =>  new Foo1(), new TransientLifetime());
		}
	}
}
