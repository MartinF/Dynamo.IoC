
using System;

namespace Dynamo.Ioc.Tests
{
	public interface IFoo
	{
		string Name { get; }
	}

	public class Foo1 : IFoo
	{
		public Foo1()
		{
			Name = "Foo1";
		}

		public string Name { get; set; }
	}

	public class Foo2 : IFoo
	{
		public string Name { get { return "Foo2"; } }
	}

	public interface IBar
	{
	}

	public class Bar1 : IBar
	{
	}

	public class Bar2 : IBar
	{
		// Used to test for infinite loop

		private readonly IFooBar _fooBar;

		public Bar2(IFooBar fooBar)
		{
			if (fooBar == null)
				throw new ArgumentNullException("fooBar");

			_fooBar = fooBar;
		}
	}

	public interface IFooBar
	{
		IFoo Foo { get; }
		IBar Bar { get; }
	}

	public class FooBar : IFooBar
	{
		public IFoo Foo { get; set; }
		public IBar Bar { get; set; }

		public FooBar(IFoo f, IBar b)
		{
			if (f == null)
				throw new ArgumentNullException("f");
			if (b == null)
				throw new ArgumentNullException("b");

			Foo = f;
			Bar = b;
		}

		public FooBar()
		{
		}
	}

	public interface IFooBarContainer
	{
		IFooBar FooBar { get; set; }
	}

	public class FooBarContainer : IFooBarContainer
	{
		public IFooBar FooBar { get; set; }

		public FooBarContainer()
		{
		}

		public FooBarContainer(IFooBar fooBar)
		{
			if (fooBar == null)
				throw new ArgumentNullException("fooBar");

			FooBar = fooBar;
		}
	}

	internal class InternalFoo : IFoo
	{
		internal InternalFoo()
		{
		}

		public string Name { get { return "Name"; } }
	}

	internal class InternalProtectedFoo : IFoo
	{
		internal protected InternalProtectedFoo()
		{
		}

		public string Name { get { return "Name"; } }
	}

	internal class ProtectedFoo : IFoo
	{
		internal protected ProtectedFoo()
		{
		}

		public string Name { get { return "Name"; } }
	}

	public class PrivateCtorFoo : IFoo
	{
		private PrivateCtorFoo()
		{
		}
	
		public string Name { get { return "Name"; } }
	}

	public abstract class AbstractFoo : IFoo
	{
		public AbstractFoo()
		{		
		}

		//protected AbstractFoo()
		//{	
		//}

		public string Name
		{
			get { return "Name"; }
		}
	}
}