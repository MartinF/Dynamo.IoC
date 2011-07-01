
namespace Dynamo.Ioc.Tests
{
	public interface IFoo
	{
		string Name { get; }
	}

	public class Foo1 : IFoo
	{
		public string Name
		{
			get { return "Foo1"; }
		}
	}

	public class Foo2 : IFoo
	{
		public string Name
		{
			get { return "Foo2"; }
		}
	}

	public interface IBar
	{
	}

	public class Bar1 : IBar
	{
	}

	public class Bar2 : IBar
	{
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
}