using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// Rename to DependencyResolverScope ?

// IScopeManager to handle multiple scopes using keys? and default?

// Currently there is no way for the Registration and/or lifetime to indicate whether the created instance can be Disposed by the scope
	// Currently only transient instances can be disposed

// Let Constructor take Func<object> or something that can be used to execute logic (dispose) instances in Dispose method.

namespace Dynamo.Ioc
{
	public class ResolverScope : IResolverScope
	{
		#region Fields
		private readonly IResolver _resolver;
		private readonly ConcurrentDictionary<Type, object> _default = new ConcurrentDictionary<Type, object>();
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> _keyed = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
		#endregion

		#region Constructors
		public ResolverScope(IResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			_resolver = resolver;
		}
		#endregion

		#region Properties
		public Index.IIndexReader Index { get { return _resolver.Index; } }
		#endregion

		#region Methods
		public object Resolve(Type type)
		{
			var instance = _default.GetOrAdd(type, t => _resolver.Resolve(t));

			return instance;
		}
		public object Resolve(Type type, object key)
		{
			var keys = _keyed.GetOrAdd(type, t => new ConcurrentDictionary<object, object>());
			var instance = keys.GetOrAdd(key, k => _resolver.Resolve(type, k));

			return instance;
		}
		
		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}
		public T Resolve<T>(object key)
		{
			return (T) Resolve(typeof(T), key);
		}

		public bool TryResolve(Type type, out object instance)
		{
			instance = _default.GetOrAdd(type, t =>
			{
				object obj;
				_resolver.TryResolve(t, out obj);
				return obj;
			});

			return (instance != null);
		}
		public bool TryResolve(Type type, object key, out object instance)
		{
			var keys = _keyed.GetOrAdd(type, t => new ConcurrentDictionary<object, object>());

			instance = keys.GetOrAdd(type, t =>
			{
				object obj;
				_resolver.TryResolve(t, out obj);
				return obj;
			});

			return (instance != null);
		}

		public bool TryResolve<T>(out T instance)
		{
			object obj;
			if (TryResolve(typeof(T), out obj))
			{
				instance = (T)obj;
				return true;
			}

			instance = default(T);
			return false;
		}
		public bool TryResolve<T>(object key, out T instance)
		{
			object obj;
			if (TryResolve(typeof(T), key, out obj))
			{
				instance = (T)obj;
				return true;
			}

			instance = default(T);
			return false;
		}

		public IEnumerable<object> ResolveAll(Type type)
		{
			foreach (var registration in _resolver.Index.GetAll(type))
			{
				object instance;

				if (registration.Key == null)
				{
					// Default
					instance = _default.GetOrAdd(type, t => registration.GetInstance());
				}
				else
				{
					// Keyed
					var keys = _keyed.GetOrAdd(type, t => new ConcurrentDictionary<object, object>());
					instance = keys.GetOrAdd(registration.Key, k => registration.GetInstance());
				}

				yield return instance;
			}
		}
		public IEnumerable<T> ResolveAll<T>()
		{
			return ResolveAll(typeof(T)).Cast<T>();
		}

		public IEnumerable<object> TryResolveAll(Type type)
		{
			foreach (var registration in _resolver.Index.TryGetAll(type))
			{
				object instance;

				if (registration.Key == null)
				{
					// Default
					instance = _default.GetOrAdd(type, t => registration.GetInstance());
				}
				else
				{
					// Keyed
					var keys = _keyed.GetOrAdd(type, t => new ConcurrentDictionary<object, object>());
					instance = keys.GetOrAdd(registration.Key, k => registration.GetInstance());
				}

				yield return instance;
			}
		}
		public IEnumerable<T> TryResolveAll<T>()
		{
			return TryResolveAll(typeof(T)).Cast<T>();
		}

		public void Dispose()
		{
			// Lock both dictionaries and enumerate and dispose all resolved instances 
				// - but need a way to know if instance is allowed to be disposed - should not if container lifetime etc. - only TransientLifetime instances should be disposed
				// Would require a wrapper object and registration/lifetime to have a "bool Scopable" property etc.

			// Clear both dictionaries?
			_default.Clear();
			_keyed.Clear();
		}
		#endregion
	}
}
