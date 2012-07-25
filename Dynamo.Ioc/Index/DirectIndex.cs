using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Fast Get/TryGet, but slow Enumerate
// Also slow when using generic Get<T> - compared to GroupedEntry

// Use List<IRegistration> to speed up Enumerating ? 
// Also for Contains(Registration?) equal by reference or by type/key ?
// private readonly List<Registration> _allIndex = new List<IRegistration>();
// Could create an index that takes the best from this and the GroupedIndex.

namespace Dynamo.Ioc.Index
{
	public class DirectIndex : IIndex
	{
		#region Fields
		private readonly Dictionary<Type, IRegistration> _defaultIndex = new Dictionary<Type, IRegistration>();
		private readonly Dictionary<Type, Dictionary<object, IRegistration>> _keyedIndex = new Dictionary<Type, Dictionary<object, IRegistration>>();
		#endregion

		public void Add(IRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			var type = registration.ReturnType;
			var key = registration.Key;

			if (key == null)
			{
				// Default
				_defaultIndex.Add(type, registration);	
			}
			else
			{
				// Keyed
				Dictionary<object, IRegistration> keyedEntry;
				if (_keyedIndex.TryGetValue(type, out keyedEntry))
				{
					// Add to already existing entry
					keyedEntry.Add(key, registration);
				}
				else
				{
					// Add new keyed entry
					keyedEntry = new Dictionary<object, IRegistration>();
					keyedEntry.Add(key, registration);
					_keyedIndex.Add(type, keyedEntry);
				}
			}
		}

		public IRegistration Get(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _defaultIndex[type];
		}
		public IRegistration Get(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");

			return _keyedIndex[type][key];
		}
		public IRegistration Get<T>()
		{
			return _defaultIndex[typeof(T)];
		}
		public IRegistration Get<T>(object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return _keyedIndex[typeof(T)][key];
		}

		public bool TryGet(Type type, out IRegistration registration)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _defaultIndex.TryGetValue(type, out registration);
		}
		public bool TryGet(Type type, object key, out IRegistration registration)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");

			Dictionary<object, IRegistration> entry;
			if (_keyedIndex.TryGetValue(type, out entry))
			{
				return entry.TryGetValue(key, out registration);
			}

			registration = null;
			return false;
		}
		public bool TryGet<T>(out IRegistration registration)
		{
			return TryGet(typeof(T), out registration);
		}
		public bool TryGet<T>(object key, out IRegistration registration)
		{
			return TryGet(typeof(T), key, out registration);
		}

		public IEnumerable<IRegistration> GetAll(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// TryGet from both default and keyed - if no found in either throw exception - else yield return

			// Currently only structured this way to make the code throw exception if not found.
			// Should implement the correct logic here and throw the correct exception ... 

			IRegistration defaultRegistration;
			if (_defaultIndex.TryGetValue(type, out defaultRegistration))
			{
				yield return defaultRegistration;

				// Default registration was found - return _keyed results - but not required
				Dictionary<object, IRegistration> keyedRegistrations;
				if (_keyedIndex.TryGetValue(type, out keyedRegistrations))
				{
					foreach (var keyedRegistration in keyedRegistrations.Values)
					{
						yield return keyedRegistration;
					}
				}
			}
			else
			{
				// No default registration - require keyed registrations - else throw exception
				foreach (var registration in _keyedIndex[type].Values)
				{
					yield return registration;
				}
			}
		}
		public IEnumerable<IRegistration> GetAll<T>()
		{
			return GetAll(typeof(T));
		}

		public IEnumerable<IRegistration> TryGetAll(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			IRegistration defaultRegistration;
			if (_defaultIndex.TryGetValue(type, out defaultRegistration))
			{
				yield return defaultRegistration;
			}

			Dictionary<object, IRegistration> keyedRegistrations;
			if (_keyedIndex.TryGetValue(type, out keyedRegistrations))
			{
				foreach (var keyedRegistration in keyedRegistrations.Values)
				{
					yield return keyedRegistration;
				}
			}
		}
		public IEnumerable<IRegistration> TryGetAll<T>()
		{
			return TryGetAll(typeof(T));
		}

		public bool Contains(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _defaultIndex.ContainsKey(type);
		}
		public bool Contains(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (key == null)
				throw new ArgumentNullException("key");

			Dictionary<object, IRegistration> registrations;
			if (_keyedIndex.TryGetValue(type, out registrations))
			{
				return registrations.ContainsKey(key);
			}

			return false;
		}
		public bool Contains<T>()
		{
			return _defaultIndex.ContainsKey(typeof(T));
		}
		public bool Contains<T>(object key)
		{
			return Contains(typeof(T), key);
		}

		public bool ContainsAny(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _defaultIndex.ContainsKey(type) || _keyedIndex.ContainsKey(type);
		}
		public bool ContainsAny<T>()
		{
			return ContainsAny(typeof(T));
		}

		public IEnumerator<IRegistration> GetEnumerator()
		{
			// Could make this faster by keeping a combined index with all registrations no matter type - just a List<IRegistration> ?

			foreach (var registration in _defaultIndex.Values)
			{
				yield return registration;
			}

			foreach (var key in _keyedIndex.Values)
			{
				foreach (var registration in key.Values)
				{
					yield return registration;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			foreach (var reg in this)
			{
				reg.Dispose();
			}

			_defaultIndex.Clear();
			_keyedIndex.Clear();
		}
	}
}
