using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dynamo.Ioc;

// Somehow make a way to test if the registration was actually compiled - expect the expression created (but it is currently not stored) !?

// Test different CompileMode's
// Dynamic CompileMode will break if there is any reference to fields or the like in the registration

// Test ThreadLifetime

// Compile it with Dynamic where it tries to inline a ContainerLifetime or InstanceRegistration ? - Will probably throw exception 
// - fix it so it doesnt try to inline when that compile mode!

// What happens if valueType is used when inlining as a constant ? RegisterInstance etc ?

// **************************************************

// Need to test that it doesnt break if CompileMode is set to Dynamic and InstanceRegistration and ContainerLifetime is used 
// They will currently automatically be inlined - which they shouldnt when usign Dynamic compiel !!!!!!!!!!

namespace Dynamo.Ioc.Tests
{
	[TestClass]
	public class CompileTest
	{
		[TestMethod]
		public void CompileDoesntBreakUsingCompileModeDynamicWhenUsingInstanceRegistrationOrContainerLifetime()
		{
			// Test that InstanceRegistration and ContainerLifetime works using CompileMode Dynamic (should not be inlined)

			using (var container = new IocContainer(defaultCompileMode: CompileMode.Dynamic))
			{
				var barInstance = new Bar1();

				var reg1 = container.Register<IFoo, Foo1>().WithContainerLifetime();
				var reg2 = container.RegisterInstance<IBar>(barInstance);

				var reg3 = container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>(), x.Resolve<IBar>()));

				container.Compile();

				var res1 = container.Resolve<IFooBar>();
				var res2 = container.Resolve<IFooBar>();

				// Assert
				Assert.AreNotSame(res1, res2);
				Assert.AreSame(barInstance, res1.Bar);
				Assert.AreSame(res1.Foo, res2.Foo);
			}
		}

		[TestMethod]
		public void CompileUsingKeyDoesntBreakUsingCompileModeDynamicWhenUsingInstanceRegistrationOrContainerLifetime()
		{
			// Test that InstanceRegistration and ContainerLifetime works using CompileMode Dynamic (should not be inlined)

			using (var container = new IocContainer(defaultCompileMode: CompileMode.Dynamic))
			{
				var barInstance = new Bar1();

				var reg1 = container.Register<IFoo, Foo1>("FirstKey").WithContainerLifetime();
				var reg2 = container.RegisterInstance<IBar>(barInstance, "SecondKey");

				var reg3 = container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>("FirstKey"), x.Resolve<IBar>("SecondKey")));

				container.Compile();

				var res1 = container.Resolve<IFooBar>();
				var res2 = container.Resolve<IFooBar>();

				// Assert
				Assert.AreNotSame(res1, res2);
				Assert.AreSame(barInstance, res1.Bar);
				Assert.AreSame(res1.Foo, res2.Foo);
			}
		}

		[TestMethod]
		public void CompileCanInlineInstanceRegistrations()
		{
			// Doesnt check that Compile actually works - just that it doesnt break anything

			using (var container = new IocContainer())
			{
				var i1 = new Foo1();
				var i2 = new Bar1();

				container.RegisterInstance(i1);
				container.RegisterInstance(i2);

				var reg1 = container.Register<IFooBar>(x => new FooBar(x.Resolve<Foo1>(), x.Resolve<Bar1>()));

				container.Compile();

				var result = (IFooBar)reg1.GetInstance();

				Assert.AreEqual(i1, result.Foo);
			}
		}

		[TestMethod]
		public void CompileCanInlineRegistrationsWithContainerLifetime()
		{
			// Doesnt check that Compile actually works - just that it doesnt break anything

			using (var container = new IocContainer())
			{
				container.Register<IFoo>(x => new Foo1()).WithContainerLifetime();
				container.Register<IBar>(x => new Bar1());

				var reg = container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>(), x.Resolve<IBar>()));

				container.Compile();

				var result1 = container.Resolve<IFoo>();
				var result2 = (IFooBar)reg.GetInstance();

				Assert.AreEqual(result1, result2.Foo);
			}
		}

		[TestMethod]
		public void CompileCanInlineRegistrationsWithTransientLifetime()
		{
			// Doesnt check that Compile actually works - just that it doesnt break anything

			using (var container = new IocContainer())
			{
				container.Register<IFoo>(x => new Foo1()).WithTransientLifetime();
				container.Register<IBar>(x => new Bar1());

				var reg1 = container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>(), x.Resolve<IBar>()));
				var reg2 = container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>(), x.Resolve<IBar>()), "Key");

				container.Compile();

				var result1 = container.Resolve<IFoo>();
				var result2 = (IFooBar)reg1.GetInstance();
				var result3 = (IFooBar)reg2.GetInstance();
				
				Assert.AreNotEqual(result1, result2.Foo);
				Assert.AreNotEqual(result1, result3.Foo);
				Assert.AreNotEqual(result2.Foo, result3.Foo);
			}
		}

		[TestMethod]
		public void CompileDoesntBreakAnything()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(x => new Bar1(), "BarKey");
				container.Register<IFooBar>(y => new FooBar((IFoo)y.Resolve(typeof(IFoo)), (IBar)y.Resolve(typeof(IBar), "BarKey")));
				container.Register<IFooBar>(xyz => new FooBar(xyz.Resolve<IFoo>(), xyz.Resolve<IBar>("BarKey")), "FooBarKey");

				// Act
				container.Compile();

				var foo = container.Resolve<IFoo>();
				var bar = container.Resolve<IBar>("BarKey");
				var foobar1 = container.Resolve<IFooBar>();
				var foobar2 = container.Resolve<IFooBar>("FooBarKey");

				// Assert
				Assert.IsNotNull(foo);
				Assert.IsNotNull(bar);
				Assert.IsNotNull(foobar1);
				Assert.IsNotNull(foobar2);

				Assert.IsNotNull(foobar1.Foo, null);
				Assert.IsNotNull(foobar1.Bar, null);
				Assert.IsNotNull(foobar2.Foo, null);
				Assert.IsNotNull(foobar2.Bar, null);
			}
		}

		[TestMethod]
		public void CompileWorksWithNesting1()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo, Foo1>().SetLifetime(new TransientLifetime());
				container.Register<IBar, Bar1>().SetLifetime(new ContainerLifetime());
				container.Register<IFooBar, FooBar>().WithTransientLifetime();
				container.Register<IFooBarContainer, FooBarContainer>().WithTransientLifetime();

				// Act
				container.Compile();

				var fooBarContainer = container.Resolve<IFooBarContainer>();

				// Assert
				Assert.IsNotNull(fooBarContainer);
				Assert.IsNotNull(fooBarContainer.FooBar);
				Assert.IsNotNull(fooBarContainer.FooBar.Foo);
				Assert.IsNotNull(fooBarContainer.FooBar.Bar);
			}
		}

		[TestMethod]
		public void CompileWorksWithNesting2()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo, Foo1>().SetLifetime(new TransientLifetime());
				container.Register<IBar, Bar1>().SetLifetime(new ContainerLifetime());
				container.Register<IFooBar, FooBar>().WithContainerLifetime();
				container.Register<IFooBarContainer, FooBarContainer>().WithContainerLifetime();

				// Act
				container.Compile();

				var fooBarContainer = container.Resolve<IFooBarContainer>();

				// Assert
				Assert.IsNotNull(fooBarContainer);
				Assert.IsNotNull(fooBarContainer.FooBar);
				Assert.IsNotNull(fooBarContainer.FooBar.Foo);
				Assert.IsNotNull(fooBarContainer.FooBar.Bar);
			}
		}

		[TestMethod]
		public void CompileWorksWithNesting3()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var foo = new Foo1();
				var bar = new Bar1();
				container.RegisterInstance<IFoo>(foo);
				container.RegisterInstance<IBar>(bar);
				container.Register<IFooBar, FooBar>().WithContainerLifetime();
				container.Register<IFooBarContainer, FooBarContainer>().WithContainerLifetime();

				// Act
				container.Compile();

				var fooBarContainer = container.Resolve<IFooBarContainer>();

				// Assert
				Assert.IsNotNull(fooBarContainer);
				Assert.IsNotNull(fooBarContainer.FooBar);
				Assert.IsNotNull(fooBarContainer.FooBar.Foo);
				Assert.IsNotNull(fooBarContainer.FooBar.Bar);

				Assert.AreEqual(foo, fooBarContainer.FooBar.Foo);
				Assert.AreEqual(bar, fooBarContainer.FooBar.Bar);
			}
		}

		[TestMethod]
		public void CompileWorksWithPropertyInjection()
		{
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
				Assert.IsNotNull(foo);
				Assert.IsNotNull(bar);
				Assert.IsNotNull(foobar);

				Assert.IsNotNull(foobar.Foo);
				Assert.IsNotNull(foobar.Bar);
			}
		}

		[TestMethod]
		public void CompileWorksWhenCompiledMultipleTimes()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				var foo = new Foo1();
				var bar = new Bar1();
				container.RegisterInstance<IFoo>(foo);
				container.RegisterInstance<IBar>(bar);
				container.Register<IFooBar, FooBar>().WithContainerLifetime();
				container.Register<IFooBarContainer, FooBarContainer>().WithContainerLifetime();

				// Act
				container.Compile();
				container.Compile();

				var fooBarContainer = container.Resolve<IFooBarContainer>();

				// Assert
				Assert.IsNotNull(fooBarContainer);
				Assert.IsNotNull(fooBarContainer.FooBar);
				Assert.IsNotNull(fooBarContainer.FooBar.Foo);
				Assert.IsNotNull(fooBarContainer.FooBar.Bar);

				Assert.AreEqual(foo, fooBarContainer.FooBar.Foo);
				Assert.AreEqual(bar, fooBarContainer.FooBar.Bar);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CompileThrowsExceptionIfRegistrationsAreNotValid()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(x => new Bar1(), "Bar");
				container.Register<IFooBar>(x => new FooBar((IFoo)x.Resolve(typeof(IFoo)), (IBar)x.Resolve(typeof(IBar), "Bar")));
				container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>(), x.Resolve<IBar>()), "FooBar");

				// Act
				container.Compile();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CompileProtectsAgainstInfiniteLoop()
		{
			using (var container = new IocContainer())
			{
				// Arrange
				container.Register<IFoo>(c => new Foo1());
				container.Register<IBar>(x => new Bar2(x.Resolve<IFooBar>()));
				container.Register<IFooBar>(x => new FooBar(x.Resolve<IFoo>(), x.Resolve<IBar>()));
				
				// Act
				container.Compile();
			}
		}
	}
}
