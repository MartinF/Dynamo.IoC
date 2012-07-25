using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class GetTest
	{
		[TestMethod]
		public void GetReturnTheExpectedRegistration()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1(), "Key1");
			var reg3 = new InstanceRegistration<IFoo>(new Foo1(), "Key2");
			var reg4 = new InstanceRegistration<IBar>(new Bar1(), "Key1");

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);
				index.Add(reg3);
				index.Add(reg4);

				var out1 = index.Get(typeof(IFoo));
				var out2 = index.Get(typeof(IFoo), "Key2");

				Assert.IsInstanceOfType(out1, typeof(IRegistration));
				Assert.IsInstanceOfType(out2, typeof(IRegistration));

				Assert.AreSame(reg1, out1);
				Assert.AreSame(reg3, out2);
			}
		}

		[TestMethod]
		public void GetGenericReturnTheExpectedRegistration()
		{
			var reg1 = new InstanceRegistration<IFoo>(new Foo1());
			var reg2 = new InstanceRegistration<IFoo>(new Foo1(), "Key1");
			var reg3 = new InstanceRegistration<IFoo>(new Foo1(), "Key2");
			var reg4 = new InstanceRegistration<IBar>(new Bar1(), "Key1");

			foreach (var index in Helper.GetIndexes())
			{
				index.Add(reg1);
				index.Add(reg2);
				index.Add(reg3);
				index.Add(reg4);

				var out1 = index.Get<IFoo>();
				var out2 = index.Get<IFoo>("Key2");

				Assert.IsInstanceOfType(out1, typeof(IRegistration));
				Assert.IsInstanceOfType(out2, typeof(IRegistration));

				Assert.AreSame(reg1, out1);
				Assert.AreSame(reg3, out2);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				index.Get(typeof(IFoo));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetUsingKeyThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				index.Get(typeof(IFoo), "Key");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetGenericThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				index.Get<IFoo>();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetGenericUsingKeyThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				index.Get<IFoo>("Key");
			}
		}
	}
}
