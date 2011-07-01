using System.Threading;

// Reset/Clear method ?

// ThreadLocal<> needs to be disposed ?

namespace Dynamo.Ioc
{
	public sealed class ThreadLocalLifetime : LifetimeBase
	{
		private readonly ThreadLocal<object> _threadLocalInstance = new ThreadLocal<object>();

		public override object GetInstance(IInstanceFactory factory, IResolver resolver)
		{
			if (!_threadLocalInstance.IsValueCreated)
				_threadLocalInstance.Value = factory.CreateInstance(resolver);

			return _threadLocalInstance.Value;
		}
	}
}