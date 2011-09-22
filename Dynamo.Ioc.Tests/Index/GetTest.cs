using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Tests.Index
{
	[TestClass]
	public class GetTest
	{
		[TestMethod]
		public void GetRegistrationReturnTheExpectedRegistration()
		{
			using (var container = new IocContainer())
			{
				var registration = container.Register(typeof(IFoo), c => new Foo1());

				var result = container.Index.Get(typeof(IFoo));

				Assert.IsNotNull(result);
				Assert.AreSame(result, registration);
			}
		}

		[TestMethod]
		public void GetRegistrationUsingKeyReturnTheExpectedRegistration()
		{
			using (var container = new IocContainer())
			{
				var registration = container.Register(typeof(IFoo), "Foo", c => new Foo1());

				var result = container.Index.Get(typeof(IFoo), "Foo");

				Assert.IsNotNull(result);
				Assert.AreSame(result, registration);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetRegistrationsThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Index.Get(typeof(IFoo));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetRegistrationsByNameThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Index.Get(typeof(IFoo), "Foo");
			}
		}



		#region Get Generic - IIndexAccessorExtensions
		[TestMethod]
		public void GetGenericReturnTheExpectedRegistration()
		{
			using (var container = new IocContainer())
			{
				var registration = container.Register<IFoo>(c => new Foo1());

				var result = container.Index.Get<IFoo>();

				Assert.IsNotNull(result);
				Assert.AreSame(result, registration);
			}
		}

		[TestMethod]
		public void GetGenericUsingKeyReturnTheExpectedRegistration()
		{
			using (var container = new IocContainer())
			{
				var registration = container.Register<IFoo>("Foo", c => new Foo1());

				var result = container.Index.Get<IFoo>("Foo");

				Assert.IsNotNull(result);
				Assert.AreSame(result, registration);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetGenericThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Index.Get<IFoo>();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetGenericUsingKeyThrowsExceptionIfRegistrationDoesntExist()
		{
			using (var container = new IocContainer())
			{
				container.Index.Get<IFoo>("Foo");
			}
		}
		#endregion
	}
}
