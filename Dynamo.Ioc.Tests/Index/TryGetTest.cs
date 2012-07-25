using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class TryGetTest
	{
		[TestMethod]
		public void TryGetReturnTheExpectedRegistration()
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

				IRegistration out1;
				var result1 = index.TryGet(typeof(IFoo), out out1);

				IRegistration out2;
				var result2 = index.TryGet(typeof(IFoo), "Key2", out out2);

				Assert.IsTrue(result1);
				Assert.IsTrue(result2);

				Assert.IsInstanceOfType(out1, typeof(IRegistration));
				Assert.IsInstanceOfType(out2, typeof(IRegistration));

				Assert.AreSame(reg1, out1);
				Assert.AreSame(reg3, out2);
			}
		}

		[TestMethod]
		public void TryGetGenericReturnTheExpectedRegistration()
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

				IRegistration out1;
				var result1 = index.TryGet<IFoo>(out out1);

				IRegistration out2;
				var result2 = index.TryGet<IFoo>("Key2", out out2);

				Assert.IsTrue(result1);
				Assert.IsTrue(result2);

				Assert.IsNotNull(out1);
				Assert.IsNotNull(out2);

				Assert.AreSame(reg1, out1);
				Assert.AreSame(reg3, out2);
			}
		}

		[TestMethod]
		public void TryGetThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				IRegistration reg;
				var result = index.TryGet(typeof(IFoo), out reg);

				Assert.IsFalse(result);
				Assert.IsNull(reg);
			}
		}

		[TestMethod]
		public void TryGetUsingKeyThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				IRegistration reg;
				var result = index.TryGet(typeof(IFoo), "Key", out reg);

				Assert.IsFalse(result);
				Assert.IsNull(reg);
			}
		}

		[TestMethod]
		public void TryGetGenericThrowsExceptionIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				IRegistration reg;
				var result = index.TryGet<IFoo>(out reg);

				Assert.IsFalse(result);
				Assert.IsNull(reg);
			}
		}

		[TestMethod]
		public void TryGetGenericUsingKeyReturnsFalseAndNullIfRegistrationDoesntExist()
		{
			foreach (var index in Helper.GetIndexes())
			{
				IRegistration reg;
				var result = index.TryGet<IFoo>("Key", out reg);

				Assert.IsFalse(result);
				Assert.IsNull(reg);
			}
		}
	}
}
