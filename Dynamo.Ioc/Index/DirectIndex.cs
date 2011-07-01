using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Fast Get/TryGet, but slow Enumerate

// Remove it ? - there is no real difference compared to the GroupedIndex ? 

namespace Dynamo.Ioc.Index
{
	public class DirectIndex : IIndex
	{
		#region Fields
		private readonly Dictionary<Type, IRegistration> _defaultIndex = new Dictionary<Type, IRegistration>();
		private readonly Dictionary<Type, Dictionary<object, IRegistration>> _keyedIndex = new Dictionary<Type, Dictionary<object, IRegistration>>();

		// Use List<IRegistration> to speed up Enumerating ? 
		// Also for Contains(Registration?) equal by reference or by type/key ?
		//private readonly List<Registration> _allIndex = new List<IRegistration>();
		// Could create an index that takes the best from this and the GroupedIndex.
		#endregion

		public void Add(IRegistration registration)
		{
			if (registration.Key == null)
			{
				// Default
				_defaultIndex.Add(registration.Type, registration);	
			}
			else
			{
				// Keyed

				Dictionary<object, IRegistration> keyedEntry;
				if (_keyedIndex.TryGetValue(registration.Type, out keyedEntry))
				{
					// Add to already existing entry
					keyedEntry.Add(registration.Key, registration);
				}
				else
				{
					// Add new keyed entry
					keyedEntry = new Dictionary<object, IRegistration>();
					keyedEntry.Add(registration.Key, registration);
					_keyedIndex.Add(registration.Type, keyedEntry);
				}
			}
		}

		public IRegistration Get(Type type)
		{
			return _defaultIndex[type];
		}

		public IRegistration Get(Type type, object key)
		{
			return _keyedIndex[type][key];
		}

		public bool TryGet(Type type, out IRegistration registration)
		{
			return _defaultIndex.TryGetValue(type, out registration);
		}

		public bool TryGet(Type type, object key, out IRegistration registration)
		{
			Dictionary<object, IRegistration> entry;
			if (_keyedIndex.TryGetValue(type, out entry))
			{
				return entry.TryGetValue(key, out registration);
			}

			registration = null;
			return false;
		}

		public IEnumerable<IRegistration> GetAll(Type type)
		{
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

		public IEnumerable<IRegistration> TryGetAll(Type type)
		{
			// Make faster implementation !

			foreach (var registration in this)
			{
				if (registration.Type == type)
					yield return registration;
			}
		}

		public bool Contains(Type type)
		{
			return _defaultIndex.ContainsKey(type);
		}

		public bool Contains(Type type, object key)
		{
			Dictionary<object, IRegistration> registrations;
			if (_keyedIndex.TryGetValue(type, out registrations))
			{
				return registrations.ContainsKey(key);
			}

			return false;
		}

		public bool ContainsAny(Type type)
		{
			return _defaultIndex.ContainsKey(type) || _keyedIndex.ContainsKey(type);
		}

		public IEnumerator<IRegistration> GetEnumerator()
		{
			// Could make this faster by keeping a combined index with all registrations no matter type - just a List<IRegistration>

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
	}
}
