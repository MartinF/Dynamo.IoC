using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testing the RegisterInstance() method on the Container and the Generic RegisterInstance method in the IResolverExtensions

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class RegisterInstanceTest
	{
		[TestMethod]
		public void RegisterInstanceReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IFoo), fooInstance);

				Assert.IsInstanceOfType(result, typeof(IRegistration));
				Assert.IsInstanceOfType(result, typeof(InstanceRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, null);
			}
		}

		[TestMethod]
		public void RegisterInstanceUsingKeyReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IFoo), "Bar", fooInstance);

				Assert.IsInstanceOfType(result, typeof(IRegistration));
				Assert.IsInstanceOfType(result, typeof(InstanceRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, "Bar");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void RegisterInstanceByTypeThrowsExceptionIfInstanceNotOfExpectedType()
		{
			using (var container = new Container())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IBar), fooInstance);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void RegisterInstanceUsingKeyThrowsExceptionIfInstanceNotOfExpectedType()
		{
			using (var container = new Container())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance(typeof(IBar), "Foo", fooInstance);
			}
		}

		[TestMethod]
		public void RegisterInstanceCanRegisterAStruct()
		{
			using (var container = new Container())
			{
				int number = 32;
				var registration = container.RegisterInstance(typeof(int), number);

				// Check registration
				Assert.IsInstanceOfType(registration, typeof(IRegistration));
				Assert.AreSame(registration.Type, typeof(int));
				Assert.AreEqual(registration.Key, null);

				// Try to resolve
				var result = container.Resolve(typeof(int));
				Assert.AreEqual(number, result);
			}
		}

		#region RegisterInstance Generic - IResolverExtensions
		[TestMethod]
		public void RegisterInstanceGenericReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance<IFoo>(fooInstance);

				Assert.IsInstanceOfType(result, typeof(IRegistration));
				Assert.IsInstanceOfType(result, typeof(InstanceRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, null);
			}
		}

		[TestMethod]
		public void RegisterInstanceGenericUsingKeyReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var fooInstance = new Foo1();
				var result = container.RegisterInstance<IFoo>("Bar", fooInstance);

				Assert.IsInstanceOfType(result, typeof(IRegistration));
				Assert.IsInstanceOfType(result, typeof(InstanceRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, "Bar");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterInstanceGenericUsingTypeThatAlreadyExistsThrowsException()
		{
			using (var container = new Container())
			{
				container.RegisterInstance<IFoo>(new Foo1());
				container.RegisterInstance<IFoo>(new Foo2());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterInstanceGenericUsingKeyThatAlreadyExistsThrowsException()
		{
			using (var container = new Container())
			{
				container.RegisterInstance<IFoo>("Bar", new Foo1());
				container.RegisterInstance<IFoo>("Bar", new Foo2());
			}
		}
		#endregion
	}
}
