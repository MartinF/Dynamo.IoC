using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Testing the Register(Type, Type) and Register<TImpl, TType>() methods

// The methods forwarding calls to the generic version throws TargetInvocationException instead of the expected InvalidOperationException

// Test selector (Func<ContructorInfo[], ConstructorInfo>) constructor parameter option
// Also test defaults - compileMode, includeInternalCtor etc ? 

namespace Dynamo.Ioc.Tests.Container
{
	[TestClass]
	public class RegisterAutoTest
	{
		// Non Generic - Register(Type, Type) and Register(Type, Type, Key)
		[TestMethod]
		public void RegisterAuto()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), typeof(Foo1));
				container.Register(typeof(IBar), typeof(Bar1));

				var reg = container.Register(typeof(IFooBar), typeof(FooBar), compileMode: CompileMode.Delegate);

				TestRegister(container, reg);
			}
		}
		[TestMethod]
		public void RegisterAutoUsingKey()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), typeof(Foo1));
				container.Register(typeof(IBar), typeof(Bar1));

				var reg = container.Register(typeof(IFooBar), typeof(FooBar), "TheKey", compileMode: CompileMode.Delegate);
				TestRegister(container, reg, "TheKey");
			}
		}

		// Generic - Register<,>() and Register<,>(Key)
		[TestMethod]
		public void RegisterAutoGeneric()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo, Foo1>();
				container.Register<IBar, Bar1>();

				var reg = container.Register<IFooBar, FooBar>(compileMode: CompileMode.Delegate);
				TestRegister(container, reg);
			}
		}
		[TestMethod]
		public void RegisterAutoGenericUsingKey()
		{
			using (var container = new IocContainer())
			{
				container.Register<IFoo, Foo1>();
				container.Register<IBar, Bar1>();

				var reg = container.Register<IFooBar, FooBar>("TheKey", compileMode: CompileMode.Delegate);
				TestRegister(container, reg, "TheKey");
			}
		}

		private void TestRegister(IocContainer container, IExpressionRegistration registration, object key = null)
		{
			Assert.IsInstanceOfType(registration, typeof(ExpressionRegistration<IFooBar>));

			// Check registration
			Assert.IsTrue(registration.ImplementationType == typeof(FooBar));
			Assert.IsTrue(registration.ReturnType == typeof(IFooBar));

			Assert.IsTrue(registration.CompileMode == CompileMode.Delegate);

			// Check resolved type
			var instance = (IFooBar)container.Resolve(registration);	// cast not needed when IExpressionRegistration<T> can be used
			Assert.IsInstanceOfType(instance, typeof(FooBar));
			Assert.IsInstanceOfType(instance.Foo, typeof(Foo1));
			Assert.IsInstanceOfType(instance.Bar, typeof(Bar1));

			// Check index
			Assert.IsTrue(key == null ? container.Index.Contains(registration.ReturnType) : container.Index.Contains(registration.ReturnType, key));
		}




		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterAutoThrowsExceptionIfImplementationTypeIsNotAssignableToRegistrationType()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), typeof(Bar1), includeInternalCtor: true);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterAutoThrowsExceptionIfImplementationTypeIsAnInterface()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), typeof(IBar), includeInternalCtor: true);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(TargetInvocationException))]
		public void RegisterAutoThrowsExceptionIfImplementationTypeIsAbstract()
		{
			using (var container = new IocContainer())
			{
				container.Register(typeof(IFoo), typeof(AbstractFoo), includeInternalCtor: true);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(TargetInvocationException))]
		public void RegisterAutoThrowsExceptionIfThereIsNoPublicOrInternalConstructor()
		{
			// Throws InvalidOperationException, which is then wrapped in the TargetInvocationException

			using (var container = new IocContainer())
			{
				// Only a private constructor exists
				container.Register(typeof(IFoo), typeof(PrivateCtorFoo), includeInternalCtor: true);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RegisterAutoGenericThrowsExceptionIfThereIsNoPublicOrInternalConstructor()
		{
			using (var container = new IocContainer())
			{
				// Only a private constructor exists
				container.Register<IFoo, PrivateCtorFoo>(includeInternalCtor: true);
			}
		}



		[TestMethod]
		public void RegisterAutoChoosePublicCtor()
		{
			// Test if chooses public and dont include internal when not set to be included

			using (var container = new IocContainer())
			{
				// Possible to register
				var result = container.Register<IConstructor, PublicConstructor>(includeInternalCtor: false);

				// Possible to resolve
				var instance = container.Resolve<IConstructor>();

				Assert.IsInstanceOfType(instance, typeof(PublicConstructor));

				// Used the correct constructor (internal)
				Assert.IsTrue(((PublicConstructor)instance).IsUsingPublicConstructor);
			}
		}

		[TestMethod]
		public void RegisterAutoChooseCtorWithMostParamatersIncludingInternal()
		{
			// Tests if chooses internal method with most parameters when set to be included

			using (var container = new IocContainer())
			{
				// Possible to register
				container.Register<IFoo, Foo1>();
				var result = container.Register<IConstructor, PublicConstructor>(includeInternalCtor: true);

				// Possible to resolve
				var instance = container.Resolve<IConstructor>();

				Assert.IsInstanceOfType(instance, typeof(PublicConstructor));

				// Used the correct constructor (internal)
				Assert.IsTrue(((PublicConstructor)instance).IsUsingInternalConstructor);
			}
		}

		[TestMethod]
		public void RegisterAutoWorksWithInternalCtor()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register(typeof(IFoo), typeof(InternalFoo), compileMode: CompileMode.Delegate, includeInternalCtor: true);
				var reg2 = container.Register(typeof(IFoo), typeof(InternalFoo), "TheKey", compileMode: CompileMode.Delegate, includeInternalCtor: true);

				var reg3 = container.Register<InternalFoo, InternalFoo>(compileMode: CompileMode.Delegate, includeInternalCtor: true);
				var reg4 = container.Register<InternalFoo, InternalFoo>("TheKey", compileMode: CompileMode.Delegate, includeInternalCtor: true);

				container.Compile();

				var instance1 = container.Resolve(reg1);
				var instance2 = container.Resolve(reg2);
				var instance3 = container.Resolve(reg3);
				var instance4 = container.Resolve(reg4);

				Assert.IsInstanceOfType(instance1, typeof(InternalFoo));
				Assert.IsInstanceOfType(instance2, typeof(InternalFoo));
				Assert.IsInstanceOfType(instance3, typeof(InternalFoo));
				Assert.IsInstanceOfType(instance4, typeof(InternalFoo));
			}
		}

		[TestMethod]
		public void RegisterAutoWorksWithInternalProtectedCtor()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register(typeof(IFoo), typeof(InternalProtectedFoo), compileMode: CompileMode.Delegate, includeInternalCtor: true);
				var reg2 = container.Register(typeof(IFoo), typeof(InternalProtectedFoo), "TheKey", compileMode: CompileMode.Delegate, includeInternalCtor: true);

				// Register as InternalProtectedFoo because default for IFoo is registered above
				var reg3 = container.Register<InternalProtectedFoo, InternalProtectedFoo>(compileMode: CompileMode.Delegate, includeInternalCtor: true);
				var reg4 = container.Register<InternalProtectedFoo, InternalProtectedFoo>("TheKey", compileMode: CompileMode.Delegate, includeInternalCtor: true);

				container.Compile();

				var instance1 = container.Resolve(reg1);
				var instance2 = container.Resolve(reg2);
				var instance3 = container.Resolve(reg3);
				var instance4 = container.Resolve(reg4);

				Assert.IsInstanceOfType(instance1, typeof(InternalProtectedFoo));
				Assert.IsInstanceOfType(instance2, typeof(InternalProtectedFoo));
				Assert.IsInstanceOfType(instance3, typeof(InternalProtectedFoo));
				Assert.IsInstanceOfType(instance4, typeof(InternalProtectedFoo));
			}
		}

		[TestMethod]
		public void RegisterAutoWorksWithCompileModeDynamicAndInternalCtor()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register(typeof(IFoo), typeof(InternalFoo), compileMode: CompileMode.Dynamic, includeInternalCtor: true);
				var reg2 = container.Register(typeof(IFoo), typeof(InternalFoo), "TheKey", compileMode: CompileMode.Dynamic, includeInternalCtor: true);

				var reg3 = container.Register<InternalFoo, InternalFoo>(compileMode: CompileMode.Dynamic, includeInternalCtor: true);
				var reg4 = container.Register<InternalFoo, InternalFoo>("TheKey", compileMode: CompileMode.Dynamic, includeInternalCtor: true);

				container.Compile();

				var instance1 = container.Resolve(reg1);
				var instance2 = container.Resolve(reg2);
				var instance3 = container.Resolve(reg3);
				var instance4 = container.Resolve(reg4);

				Assert.IsInstanceOfType(instance1, typeof(InternalFoo));
				Assert.IsInstanceOfType(instance2, typeof(InternalFoo));
				Assert.IsInstanceOfType(instance3, typeof(InternalFoo));
				Assert.IsInstanceOfType(instance4, typeof(InternalFoo));
			}
		}

		[TestMethod]
		public void RegisterAutoWorksWithCompileModeDynamicAndInternalProtectedCtor()
		{
			using (var container = new IocContainer())
			{
				var reg1 = container.Register(typeof(IFoo), typeof(InternalProtectedFoo), compileMode: CompileMode.Dynamic, includeInternalCtor: true);
				var reg2 = container.Register(typeof(IFoo), typeof(InternalProtectedFoo), "TheKey", compileMode: CompileMode.Dynamic, includeInternalCtor: true);

				// Register as InternalProtectedFoo because default for IFoo is registered above
				var reg3 = container.Register<InternalProtectedFoo, InternalProtectedFoo>(compileMode: CompileMode.Dynamic, includeInternalCtor: true);
				var reg4 = container.Register<InternalProtectedFoo, InternalProtectedFoo>("TheKey", compileMode: CompileMode.Dynamic, includeInternalCtor: true);

				container.Compile();

				var instance1 = container.Resolve(reg1);
				var instance2 = container.Resolve(reg2);
				var instance3 = container.Resolve(reg3);
				var instance4 = container.Resolve(reg4);

				Assert.IsInstanceOfType(instance1, typeof(InternalProtectedFoo));
				Assert.IsInstanceOfType(instance2, typeof(InternalProtectedFoo));
				Assert.IsInstanceOfType(instance3, typeof(InternalProtectedFoo));
				Assert.IsInstanceOfType(instance4, typeof(InternalProtectedFoo));
			}
		}




		#region Throws exception if set to not use internal ctor (and class is internal)
		[TestMethod]
		[ExpectedException(typeof(TargetInvocationException))]
		public void RegisterAutoThrowsExceptionIfSetToNotUseInternalCtor()
		{
			// Throws InvalidOperationException, which is then wrapped in the TargetInvocationException

			using (var container = new IocContainer())
			{
				var reg = container.Register(typeof(IFoo), typeof(InternalFoo), compileMode: CompileMode.Dynamic, includeInternalCtor: false);
			}
		}
		[TestMethod]
		[ExpectedException(typeof(TargetInvocationException))]
		public void RegisterAutoUsingKeyThrowsExceptionIfSetToNotUseInternalCtor()
		{
			// Throws InvalidOperationException, which is then wrapped in the TargetInvocationException

			using (var container = new IocContainer())
			{
				var reg = container.Register(typeof(IFoo), typeof(InternalFoo), "TheKey", compileMode: CompileMode.Dynamic, includeInternalCtor: false);
			}
		}
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RegisterAutoGenericThrowsExceptionIfSetToNotUseInternalCtor()
		{
			using (var container = new IocContainer())
			{
				var reg = container.Register<IFoo, InternalFoo>(compileMode: CompileMode.Dynamic, includeInternalCtor: false);
			}
		}
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RegisterAutoGenericUsingKeyThrowsExceptionIfSetToNotUseInternalCtor()
		{
			using (var container = new IocContainer())
			{
				var reg = container.Register<IFoo, InternalFoo>("TheKey", compileMode: CompileMode.Dynamic, includeInternalCtor: false);
			}
		}
		#endregion

		#region Stubs
		public interface IConstructor
		{
		}
		public class PublicConstructor : IConstructor
		{
			public PublicConstructor()
			{
				IsUsingPublicConstructor = true;
				IsUsingInternalConstructor = false;
			}

			internal PublicConstructor(IFoo foo)
			{
				Foo = foo;
				IsUsingPublicConstructor = false;
				IsUsingInternalConstructor = true;
			}

			public IFoo Foo { get; private set; }
			public bool IsUsingPublicConstructor { get; private set; }
			public bool IsUsingInternalConstructor { get; private set; }
		}
		#endregion
	}
}