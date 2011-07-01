using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testing the Register() methods on the Container and the generic Register method on IResolverExtensions.

// Write tests for each Index supported ? 
// Write tests for each CompileMode ?
		
namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class RegisterTest
	{
		[TestMethod]
		public void RegisterReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var result = container.Register(typeof(IFoo), c => new Foo1());

				Assert.IsInstanceOfType(result, typeof(IConfigurableRegistration));
				Assert.IsInstanceOfType(result, typeof(ExpressionRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, null);
			}
		}

		[TestMethod]
		public void RegisterUsingKeyReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var result = container.Register(typeof(IFoo), "Bar", c => new Foo1());

				Assert.IsInstanceOfType(result, typeof(IConfigurableRegistration));
				Assert.IsInstanceOfType(result, typeof(ExpressionRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, "Bar");
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterUsingTypeThatAlreadyExistsThrowsException()
		{
			using (var container = new Container())
			{
				container.Register(typeof(IFoo), c => new Foo1());
				container.Register(typeof(IFoo), c => new Foo2());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterUsingKeyThatAlreadyExistsThrowsException()
		{
			using (var container = new Container())
			{
				container.Register(typeof(IFoo), "Bar", c => new Foo1());
				container.Register(typeof(IFoo), "Bar", c => new Foo2());
			}
		}

		[TestMethod]
		public void RegisterWithPropertyInjectionResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register(typeof(IFoo), c => new Foo1());
				container.Register(typeof(IBar), c => new Bar1());
				container.Register(typeof(IFooBar), c => new FooBar() { Foo = (IFoo)c.Resolve(typeof(IFoo)), Bar = (IBar)c.Resolve(typeof(IBar)) });

				var result = (IFooBar)container.Resolve(typeof(IFooBar));

				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Foo);
				Assert.IsNotNull(result.Bar);

				Assert.IsInstanceOfType(result, typeof(FooBar));
				Assert.IsInstanceOfType(result.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(result.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void RegisterUsingKeyWithPropertyInjectionResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register(typeof(IFoo), "Foo", c => new Foo1());
				container.Register(typeof(IBar), "Bar", c => new Bar1());
				container.Register(typeof(IFooBar), "PropInjection", c => new FooBar() { Foo = (IFoo)c.Resolve(typeof(IFoo), "Foo"), Bar = (IBar)c.Resolve(typeof(IBar), "Bar") });

				var result = (IFooBar)container.Resolve(typeof(IFooBar), "PropInjection");

				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Foo);
				Assert.IsNotNull(result.Bar);

				Assert.IsInstanceOfType(result, typeof(FooBar));
				Assert.IsInstanceOfType(result.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(result.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void RegisterThrowsExceptionIfExpressionDoesntReturnExpectedType()
		{
			using (var container = new Container())
			{
				var result = container.Register(typeof(IBar), c => new Foo1());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(RegistrationException))]
		public void RegisterUsingKeyThrowsExceptionIfExpressionDoesntReturnExpectedType()
		{
			using (var container = new Container())
			{
				var result = container.Register(typeof(IBar), "Foo", c => new Foo1());
			}
		}



		[TestMethod]
		public void RegisterCanRegisterAStruct()
		{
			// Works with CompileMode.Delegate
			// Also works with CompileMode.Dynamic - but only if value is included directly - cannot reference a local variable

			using (var container = new Container())
			{
				Type type = typeof(int);

				var registration = container.Register(type, x => 32);

				// Check registration
				Assert.IsInstanceOfType(registration, typeof(IConfigurableRegistration));
				Assert.IsInstanceOfType(registration, typeof(ExpressionRegistration));

				Assert.AreSame(registration.Type, type);
				Assert.AreEqual(registration.Key, null);

				// Try to resolve
				var result = container.Resolve(type);
				Assert.AreEqual(32, result);
			}
		}

		[TestMethod]
		public void RegisterGenericCanRegisterAStruct()
		{
			// Works with CompileMode.Delegate
			// Also works with CompileMode.Dynamic - but only if value is included directly - cannot reference a local variable

			using (var container = new Container())
			{
				var registration = container.Register<int>(x => 32);

				// Check registration
				Assert.IsInstanceOfType(registration, typeof(IConfigurableRegistration));
				Assert.IsInstanceOfType(registration, typeof(ExpressionRegistration));

				Assert.AreSame(registration.Type, typeof(int));
				Assert.AreEqual(registration.Key, null);

				// Try to resolve
				var result = container.Resolve<int>();
				Assert.AreEqual(32, result);
			}
		}







		#region Register Generic - ResolverExtensions
		[TestMethod]
		public void RegisterGenericReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var result = container.Register<IFoo>(c => new Foo1());

				Assert.IsInstanceOfType(result, typeof(IConfigurableRegistration));
				Assert.IsInstanceOfType(result, typeof(ExpressionRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, null);
			}
		}

		[TestMethod]
		public void RegisterGenericUsingKeyReturnsCorrectType()
		{
			using (var container = new Container())
			{
				var result = container.Register<IFoo>("Bar", c => new Foo1());

				Assert.IsInstanceOfType(result, typeof(IConfigurableRegistration));
				Assert.IsInstanceOfType(result, typeof(ExpressionRegistration));

				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, "Bar");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterGenericUsingTypeThatAlreadyExistsThrowsException()
		{
			using (var container = new Container())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IFoo>(c => new Foo2());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterGenericUsingKeyThatAlreadyExistsThrowsException()
		{
			using (var container = new Container())
			{
				container.Register<IFoo>("Bar", c => new Foo1());
				container.Register<IFoo>("Bar", c => new Foo2());
			}
		}

		[TestMethod]
		public void RegisterGenericWithPropertyInjectionResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(c => new Bar1());
				container.Register<IFooBar>(c => new FooBar() { Foo = c.Resolve<IFoo>(), Bar = c.Resolve<IBar>() });

				var result = container.Resolve<IFooBar>();

				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Foo);
				Assert.IsNotNull(result.Bar);

				Assert.IsInstanceOfType(result, typeof(FooBar));
				Assert.IsInstanceOfType(result.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(result.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void RegisterGenericUsingKeyWithPropertyInjectionResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register<IFoo>("Foo", c => new Foo1());
				container.Register<IBar>("Bar", c => new Bar1());
				container.Register<IFooBar>("PropInjection", c => new FooBar() { Foo = c.Resolve<IFoo>("Foo"), Bar = c.Resolve<IBar>("Bar") });

				var result = container.Resolve<IFooBar>("PropInjection");

				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Foo);
				Assert.IsNotNull(result.Bar);

				Assert.IsInstanceOfType(result, typeof(FooBar));
				Assert.IsInstanceOfType(result.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(result.Bar, typeof(Bar1));
			}
		}
		#endregion
	}
}