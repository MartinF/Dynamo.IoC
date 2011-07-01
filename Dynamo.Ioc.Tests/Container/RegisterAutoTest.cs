using System;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testing the Register<TImpl, TType>() helper in the ContainerExtensions.

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class RegisterAutoTest
	{
		[TestMethod]
		public void RegisterAutoByTypeReturnsIConfigurableRegistration()
		{
			using (var container = new Container())
			{
				var result = container.Register<IFoo, Foo1>();

				Assert.IsInstanceOfType(result, typeof(IConfigurableRegistration));
				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, null);
			}
		}

		[TestMethod]
		public void RegisterAutoByNameReturnsIConfigurableRegistration()
		{
			using (var container = new Container())
			{
				var result = container.Register<IFoo, Foo1>("Bar");

				Assert.IsInstanceOfType(result, typeof(IConfigurableRegistration));
				Assert.AreSame(result.Type, typeof(IFoo));
				Assert.AreEqual(result.Key, "Bar");
			}
		}

		[TestMethod]
		public void RegisterAutoByTypeResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register<IFoo, Foo1>();
				var result = container.Resolve<IFoo>();

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void RegisterAutoByNameResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register<IFoo, Foo1>("Bar");
				var result = container.Resolve<IFoo>("Bar");

				Assert.IsNotNull(result);
				Assert.IsInstanceOfType(result, typeof(Foo1));
			}
		}

		[TestMethod]
		public void RegisterAutoByTypeWithParametersResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register<IFoo, Foo1>();
				container.Register<IBar, Bar1>();
				container.Register<IFooBar, FooBar>();

				IFooBar result = container.Resolve<IFooBar>();

				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Foo);
				Assert.IsNotNull(result.Bar);

				Assert.IsInstanceOfType(result, typeof(FooBar));
				Assert.IsInstanceOfType(result.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(result.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void RegisterAutoByNameWithParametersResolvesToCorrectType()
		{
			using (var container = new Container())
			{
				container.Register<IFoo, Foo1>();
				container.Register<IBar, Bar1>();
				container.Register<IFooBar, FooBar>("Bar");

				IFooBar result = container.Resolve<IFooBar>("Bar");

				Assert.IsNotNull(result);
				Assert.IsNotNull(result.Foo);
				Assert.IsNotNull(result.Bar);

				Assert.IsInstanceOfType(result, typeof(FooBar));
				Assert.IsInstanceOfType(result.Foo, typeof(Foo1));
				Assert.IsInstanceOfType(result.Bar, typeof(Bar1));
			}
		}

		[TestMethod]
		public void RegisterAutoIsAbleToRegisterAndResolveClassWithInternalConstructor()
		{
			using (var container = new Container())
			{
				// Possible to register
				var result = container.Register<IConstructor, InternalConstructor>();

				// Possible to resolve
				var instance = container.Resolve<IConstructor>();

				Assert.IsNotNull(instance);
				Assert.IsInstanceOfType(instance, typeof(InternalConstructor));
			}
		}

		[TestMethod]
		public void RegisterAutoIsAbleToRegisterAndResolveClassWithInternalProtectedConstructor()
		{
			using (var container = new Container())
			{
				// Possible to register
				var result = container.Register<IConstructor, InternalProtectedConstructor>();

				// Possible to resolve
				var instance = container.Resolve<IConstructor>();

				Assert.IsNotNull(instance);
				Assert.IsInstanceOfType(instance, typeof(InternalProtectedConstructor));
			}
		}

		[TestMethod]
		public void RegisterAutoChoosePublicOverInternalConstructorEvenThoughInternalHaveMoreParameters()
		{
			using (var container = new Container())
			{
				// Possible to register
				var result = container.Register<IConstructor, PublicConstructor>();

				// Possible to resolve
				var instance = container.Resolve<IConstructor>();

				Assert.IsNotNull(instance);
				Assert.IsInstanceOfType(instance, typeof(PublicConstructor));

				// Used the correct constructor
				Assert.IsTrue(((PublicConstructor)instance).IsUsingPublicConstructor);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterAutoThrowsExceptionIfClassHaveNoPublicOrInternalConstructor()
		{
			using (var container = new Container())
			{
				// Only a private constructor exists
				container.Register<IConstructor, PrivateConstructor>();
			}
		}



		#region Stubs

		public interface IConstructor
		{
		}

		public class PrivateConstructor : IConstructor
		{
			private PrivateConstructor()
			{
			}
		}

		internal class InternalConstructor : IConstructor
		{
			internal InternalConstructor()
			{
			}

			private InternalConstructor(string notbeingselected)
			{
			}
		}

		internal class InternalProtectedConstructor : IConstructor
		{
			internal protected InternalProtectedConstructor()
			{
			}

			private InternalProtectedConstructor(string notbeingselected)
			{
			}
		}

		public class PublicConstructor : IConstructor
		{
			public PublicConstructor()
			{
				IsUsingPublicConstructor = true;
			}

			internal PublicConstructor(IFoo foo)
			{
				Foo = foo;
				IsUsingPublicConstructor = false;
			}

			public IFoo Foo { get; private set; }
			public bool IsUsingPublicConstructor { get; private set; }
		}

		#endregion
	}
}