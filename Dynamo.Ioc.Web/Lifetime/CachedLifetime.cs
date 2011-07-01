using System;
using System.Web;
using System.Web.Caching;

// Do not include absolute expiration ? - what will happen when the specific date is meet ? Then it will keep creating a new instance ?

// Add Clear/Reset method ?

namespace Dynamo.Ioc
{
	public sealed class CachedLifetime : LifetimeBase
	{
		#region Fields

		private string _key;
		private readonly CacheDependency _dependency = null;	
		private readonly CacheItemPriority _itemPriority = CacheItemPriority.Default;
		private readonly CacheItemRemovedCallback _itemRemovedCallback = null;
		private readonly DateTime _absoluteExpiration = Cache.NoAbsoluteExpiration;
		private readonly TimeSpan _slidingExpiration = Cache.NoSlidingExpiration;
		
		#endregion

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dependency">Sets the Cache Dependencies for this Lifetime instance.</param>
		/// <param name="itemPriority">Sets the priority of the item in the cache.</param>
		/// <param name="itemRemovedCallback">Sets a callback method for when an item is removed (expires).</param>
		public CachedLifetime(CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		{
			_dependency = dependency;
			_itemPriority = itemPriority;
			_itemRemovedCallback = itemRemovedCallback;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="absoluteExpiration">Sets the absolute time for when the cached instance expires.</param>
		/// <param name="dependency">Sets the Cache Dependencies for this Lifetime instance.</param>
		/// <param name="itemPriority">Sets the priority of the item in the cache.</param>
		/// <param name="itemRemovedCallback">Sets a callback method for when an item is removed (expires).</param>
		//public CachedLifetime(DateTime absoluteExpiration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
		//    : this(dependency, itemPriority, itemRemovedCallback)
		//{
		//    _absoluteExpiration = absoluteExpiration;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="slidingExpiration">Sets the duration for the cached instance will remain cached.</param>
		/// <param name="dependency">Sets the Cache Dependencies for this Lifetime instance.</param>
		/// <param name="itemPriority">Sets the priority of the item in the cache.</param>
		/// <param name="itemRemovedCallback">Sets a callback method for when an item is removed (expires).</param>
		public CachedLifetime(TimeSpan slidingExpiration, CacheDependency dependency = null, CacheItemPriority itemPriority = CacheItemPriority.Default, CacheItemRemovedCallback itemRemovedCallback = null)
			: this(dependency, itemPriority, itemRemovedCallback)
		{
			_slidingExpiration = slidingExpiration;
		}

		#endregion

		#region Methods

		public override void Init(IRegistrationInfo registration)
		{
			// TODO: Needs fixing doesnt it !? key hashcode ? or whatever should be used ? _key.GetType().Name ?

			// Create key
			_key = "#" + registration.Type.Name;	// Fullname ?
			if (registration.Key != null)
				_key += "-" + registration.Key;
		}

		public override object GetInstance(IInstanceFactory factory, IResolver resolver)
		{
			Cache cache = HttpRuntime.Cache;

			var instance = cache[_key];
			if (instance == null)
			{
				instance = factory.CreateInstance(resolver);
				cache.Insert(_key, instance, _dependency, _absoluteExpiration, _slidingExpiration, _itemPriority, _itemRemovedCallback);
			}

			return instance;
		}
		#endregion
	}
}