using System;
using System.Linq;
using Dynamo.Ioc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.Ioc.Extensions.Tests
{
	[TestClass]
	public class ResolverScopeTest
	{
		[TestMethod]
		public void ContainerScopeResolveWorks()
		{
			using (var container = new IocContainer())
			{
				// Register
				container.Register<IFoo, Foo1>();
				container.Register<IFoo, Foo2>("Key1");
				container.Register<IBar, Bar1>();

				using (var scope = container.GetScope())
				{
					var instance1 = scope.Resolve<IFoo>();
					var instance2 = scope.Resolve<IFoo>();

					Assert.AreSame(instance1, instance2);
				}
			}
		}

		[TestMethod]
		public void ContainerScopeTryResolveWorks()
		{
			using (var container = new IocContainer())
			{
				// Register
				container.Register<IFoo, Foo1>();
				container.Register<IFoo, Foo2>("Key1");
				container.Register<IBar, Bar1>();

				using (var scope = container.GetScope())
				{
					IFoo instance1, instance2;
					var result1 = scope.TryResolve<IFoo>(out instance1);
					var result2 = scope.TryResolve<IFoo>(out instance2);

					Assert.AreSame(instance1, instance2);

					IFooBar instance3, instance4;
					var result3 = scope.TryResolve<IFooBar>(out instance3);
					var result4 = scope.TryResolve<IFooBar>(out instance4);

					Assert.AreSame(instance3, instance4);
				}
			}
		}

		[TestMethod]
		public void ContainerScopeResolveAllWorks()
		{
			using (var container = new IocContainer())
			{
				// Register
				container.Register<IFoo, Foo1>();
				container.Register<IFoo, Foo2>("Key1");
				container.Register<IBar, Bar1>();

				using (var scope = container.GetScope())
				{
					var all = scope.ResolveAll<IFoo>().ToList();
					
					Assert.IsTrue(all.Count == 2);

					Assert.IsInstanceOfType(all[0], typeof(Foo1));
					Assert.IsInstanceOfType(all[1], typeof(Foo2));
			
					// Scope
					var all2 = scope.ResolveAll<IFoo>().ToList();

					Assert.AreSame(all[0], all2[0]);
					Assert.AreSame(all[1], all2[1]);

					using (var scope2 = container.GetScope())
					{
						var all3 = scope2.ResolveAll<IFoo>().ToList();

						Assert.IsTrue(all3.Count == 2);

						Assert.IsInstanceOfType(all[0], typeof(Foo1));
						Assert.IsInstanceOfType(all[1], typeof(Foo2));

						Assert.AreNotSame(all[0], all3[0]);
						Assert.AreNotSame(all[1], all3[1]);
					}
				}
			}
		}

		[TestMethod]
		public void ContainerScopeTryResolveAllWorks()
		{
			using (var container = new IocContainer())
			{
				// Register
				container.Register<IFoo, Foo1>();
				container.Register<IFoo, Foo2>("Key1");
				container.Register<IBar, Bar1>();

				using (var scope = container.GetScope())
				{
					var all = scope.TryResolveAll<IFoo>().ToList();

					Assert.IsTrue(all.Count == 2);

					Assert.IsInstanceOfType(all[0], typeof(Foo1));
					Assert.IsInstanceOfType(all[1], typeof(Foo2));

					// Scope
					var all2 = scope.TryResolveAll<IFoo>().ToList();

					Assert.AreSame(all[0], all2[0]);
					Assert.AreSame(all[1], all2[1]);

					using (var scope2 = container.GetScope())
					{
						var all3 = scope2.TryResolveAll<IFoo>().ToList();

						Assert.IsTrue(all3.Count == 2);

						Assert.IsInstanceOfType(all[0], typeof(Foo1));
						Assert.IsInstanceOfType(all[1], typeof(Foo2));

						Assert.AreNotSame(all[0], all3[0]);
						Assert.AreNotSame(all[1], all3[1]);
					}
				}
			}
		}
	}
}
