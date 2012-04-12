
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
		private readonly IFooBar _foobar;

		public Bar2(IFooBar foobar)
		{
			_foobar = foobar;
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
			Foo = f;
			Bar = b;
		}

		public FooBar()
		{
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