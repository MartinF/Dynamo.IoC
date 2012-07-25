using System;

// TryResolve? 
// Resolve method instead of this[] ?

namespace Dynamo.Ioc
{
	public class KeyResolver<T, TKey>
	{
		private readonly IResolver _resolver;
		
		public KeyResolver(IResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			_resolver = resolver;
		}

		public T this[TKey key]
		{
			get { return _resolver.Resolve<T>(key); }
		}
	}
}