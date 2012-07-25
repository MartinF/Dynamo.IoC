using System;
using System.Runtime.Caching;

// Could add support for Get and Add with region ?

namespace Dynamo.Ioc
{
	public class CachedLifetime : ILifetime
	{
		#region Fields
		private readonly ObjectCache _cache;
		private readonly CacheItemPolicy _cachePolicy;
		private readonly string _key = Guid.NewGuid().ToString();
		private readonly object _lock = new object();
		#endregion

		#region Constructors
		public CachedLifetime(CacheItemPolicy policy) : this(MemoryCache.Default, policy)
		{
		}
		public CachedLifetime(ObjectCache cache, CacheItemPolicy policy)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");
			if (policy == null)
				throw new ArgumentNullException("policy");

			_cache = cache;
			_cachePolicy = policy;
		}
		#endregion

		#region Properties
		protected ObjectCache Cache { get { return _cache; } }
		protected string Key { get { return _key; } }
		#endregion

		#region Methods
		public object GetInstance(IInstanceFactoryRegistration registration)
		{
			var instance = _cache.Get(_key);

			if (instance == null)
			{
				lock (_lock)
				{
					instance = _cache.Get(_key);
					if (instance == null)
					{
						instance = registration.CreateInstance();
						_cache.Add(_key, instance, _cachePolicy);
					}
				}
			}

			return instance;
		}
		#endregion

		public void Dispose()
		{
			// Only handle disposing of dynamo ioc related references
			// If instance registered uses unmanaged resources and should be disposed the user should implement a finalizer

			// Dispose item added?
		}
	}
}