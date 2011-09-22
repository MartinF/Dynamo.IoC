using Microsoft.VisualStudio.TestTools.UnitTesting;

// Clean up and move to where it belongs...

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class ContainerTest
	{
		[TestMethod]
		public void ContainerCanBeCreated()
		{
			using (var container = new IocContainer())
			{
				Assert.IsNotNull(container);
			}
		}

		[TestMethod]
		public void ContainerUsesTheDefaultLifetimeSetWhenRegistering()
		{
			// Make sure the set lifetime is not the default - if changed in the future !?

			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				var result1 = container.Register<IFoo>(c => new Foo1());
				var result2 = container.Register<IBar>(c => new Bar1());

				Assert.IsTrue(result1.Lifetime is ContainerLifetime);
				Assert.IsTrue(result2.Lifetime is ContainerLifetime);
			}
		}

		[TestMethod]
		public void CanRegisterAndResolveMultipleTypes()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(c => new Bar1());

				var result1 = container.Resolve<IFoo>();
				var result2 = container.Resolve<IBar>();

				Assert.IsNotNull(result1);
				Assert.IsNotNull(result2);

				Assert.IsInstanceOfType(result1, typeof(Foo1));
				Assert.IsInstanceOfType(result2, typeof(Bar1));
			}
		}

		[TestMethod]
		public void CanSetDefaultLifetimeUsingTheConstructor()
		{
			ILifetimeInfo defaultLifetime;

			// Make sure that the default lifetime is not the same as the one i am trying to change it to (if default is changed in the future)
			using (var container = new IocContainer())
			{
				defaultLifetime = container.DefaultLifetime;
				Assert.IsNotInstanceOfType(defaultLifetime, typeof(ContainerLifetime));
			}

			using (var container = new IocContainer(() => new ContainerLifetime()))
			{
				Assert.IsTrue(container.DefaultLifetime is ContainerLifetime);
			}
		}

		[TestMethod]
		public void CanChangeLifetimeUsingSetLifetime()
		{
			// Move to somewhere ...
			// Isnt it already in the Register Tests ?

			using (var container = new IocContainer(() => new TransientLifetime()))
			{
				Assert.IsInstanceOfType(container.DefaultLifetime, typeof(TransientLifetime));

				var registration = container.Register<IFoo>(c => new Foo1()).SetLifetime(new ContainerLifetime());

				Assert.IsInstanceOfType(registration.Lifetime, typeof(ContainerLifetime));

				var foo1 = container.Resolve<IFoo>();
				var foo2 = container.Resolve<IFoo>();
				
				Assert.AreSame(foo1, foo2);
			}
		}



		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void VerifyThrowsExceptionWhenRegistrationIsNotAssignableToType()
		{
			// What to do with this  ... ?

			// Should this be required ?
			// There is some problems with this because structs should be handled differently.
			// See the Register method in the Container

			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register(typeof(IBar), c => new Foo1());

				Assert.IsTrue(false);

				//container.Verify();
			}
		}




		#region Verify
		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void VerifyThrowsExceptionWhenMissingRegistration()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFooBar>(y => new FooBar((IFoo)y.Resolve(typeof(IFoo)), (IBar)y.Resolve(typeof(IBar))));

				container.Verify();
			}
		}
		#endregion

		#region Compile
		[TestMethod]
		public void CompileDoesntBreakAnything()
		{
			// What to do with this ... ?
			// To find out if anything was actually compiled it needs to inspect the Expression/Lambda created

			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>("bar1name", x => new Bar1());
				container.Register<IFooBar>(y => new FooBar((IFoo)y.Resolve(typeof(IFoo)), (IBar)y.Resolve(typeof(IBar), "bar1name")));
				container.Register<IFooBar>("foobarname", xyz => new FooBar(xyz.Resolve<IFoo>(), xyz.Resolve<IBar>("bar1name")));

				// Act
				container.Compile();

				var foo = container.Resolve<IFoo>();
				var bar = container.Resolve<IBar>("bar1name");
				var foobar1 = container.Resolve<IFooBar>();
				var foobar2 = container.Resolve<IFooBar>("foobarname");

				// Assert
				Assert.AreNotSame(foo, null);
				Assert.AreNotSame(bar, null);
				Assert.AreNotSame(foobar1, null);
				Assert.AreNotSame(foobar2, null);

				Assert.AreNotSame(foobar1.Foo, null);
				Assert.AreNotSame(foobar1.Bar, null);
				Assert.AreNotSame(foobar2.Foo, null);
				Assert.AreNotSame(foobar2.Bar, null);

				Assert.IsInstanceOfType(foo, typeof(Foo1));
				Assert.IsInstanceOfType(bar, typeof(Bar1));
				Assert.IsInstanceOfType(foobar1, typeof(FooBar));
				Assert.IsInstanceOfType(foobar2, typeof(FooBar));

				Assert.IsInstanceOfType(foobar1.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(foobar1.Bar, typeof(Bar1));

				Assert.IsInstanceOfType(foobar2.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(foobar2.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void CompileWorksWithPropertyInjection()
		{
			// Can Resolve Property Injection !?
			// Make one using Resolve(Type) and one using Resolve<> generic ?

			// There need to be a check that the registration was actually compiled

			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(c => new Bar1());
				container.Register<IFooBar>(c => new FooBar() { Foo = c.Resolve<IFoo>(), Bar = c.Resolve<IBar>() });

				// Act
				container.Compile();

				var foo = container.Resolve<IFoo>();
				var bar = container.Resolve<IBar>();
				var foobar = container.Resolve<IFooBar>();

				// Assert
				Assert.AreNotSame(foo, null);
				Assert.AreNotSame(bar, null);
				Assert.AreNotSame(foobar, null);

				Assert.AreNotSame(foobar.Foo, null);
				Assert.AreNotSame(foobar.Bar, null);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void CompileThrowsExceptionIfRegistrationsAreNotValid()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>("Bar", x => new Bar1());
				container.Register<IFooBar>(y => new FooBar((IFoo)y.Resolve(typeof(IFoo)), (IBar)y.Resolve(typeof(IBar), "Bar")));
				container.Register<IFooBar>("FooBar", xyz => new FooBar(xyz.Resolve<IFoo>(), xyz.Resolve<IBar>()));

				// Act
				container.Compile();
			}
		}
		#endregion
	}
}
