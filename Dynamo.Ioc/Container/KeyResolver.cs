using System;

namespace Dynamo.Ioc
{
	public class KeyResolver<T, TKey> : IKeyResolver<T, TKey>
	{
		private readonly IResolver _resolver;
		private readonly Type _type;

		public KeyResolver(IResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			_resolver = resolver;

			_type = typeof(T);

			// Get the registrations already here instead of looking up later ?
			// Look up and store all available keys - and expose them via property ?
		}

		public T this[TKey key]
		{
			get { return (T) _resolver.Resolve(_type, key); }
		}

		//public TReturn Resolve(TKey key)
		//{
		//    // Could use generic version of Resolve
		//    return (TReturn)_resolver.Resolve(_type, key);
		//}

		//public bool TryResolve(TKey key, out TReturn instance)
		//{
		//    // Could use generic version of TryResolve
		//    return _resolver.TryResolve(_type, key, out instance);
		//}
	}
}