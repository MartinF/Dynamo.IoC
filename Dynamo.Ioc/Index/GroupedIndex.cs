using System;
using System.Collections;
using System.Collections.Generic;

// Choosen as default because it is the best all-round with both Get() and Get<T>() etc.
// DirectIndex is a little bit faster when it comes to the Get() method but a lot slower using Get<T>()

// Using "as" to cast instead of explicit cast is maybe slightly faster.
// Using method with no overloads might also be slightly faster ?

namespace Dynamo.Ioc.Index
{
	public class GroupedIndex : IIndex
	{
		#region Fields
		private readonly Dictionary<Type, IGroupedEntry> _index = new Dictionary<Type, IGroupedEntry>();
		#endregion

		#region Methods
		public void Add<T>(IRegistration<T> registration, object key = null)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			var type = typeof(T);

			IGroupedEntry oldEntry;
			if (_index.TryGetValue(type, out oldEntry))
			{
				// Add to already existing entry
				//oldEntry.Add(registration, key);		// create virtual add method on GroupedEntry - will it hit the generic one or the base one ?
				((GroupedEntry<T>)oldEntry).Add(registration, key);
			}
			else
			{
				// Add new entry
				var newEntry = new GroupedEntry<T>();
				newEntry.Add(registration, key);
				_index.Add(type, newEntry);
			}
		}

		public IRegistration Get(Type type)
		{
			return _index[type].Get();
		}
		public IRegistration Get(Type type, object key)
		{
			return _index[type].Get(key);
		}
		public IRegistration<T> Get<T>()
		{
			return ((GroupedEntry<T>)_index[typeof(T)]).Get();
		}
		public IRegistration<T> Get<T>(object key)
		{
			//return (IRegistration<T>)_index[typeof(T)].Get(key);
			return ((GroupedEntry<T>)_index[typeof(T)]).Get(key);
		}

		public bool TryGet(Type type, out IRegistration registration)
		{
			IGroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry.TryGet(out registration);
			}

			registration = null;
			return false;
		}
		public bool TryGet(Type type, object key, out IRegistration registration)
		{
			IGroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry.TryGet(key, out registration);
			}

			registration = null;
			return false;
		}
		public bool TryGet<T>(out IRegistration<T> registration)
		{
			IGroupedEntry entry;
			if (_index.TryGetValue(typeof(T), out entry))
			{
				return ((GroupedEntry<T>)entry).TryGet(out registration);
			}

			registration = null;
			return false;
		}
		public bool TryGet<T>(object key, out IRegistration<T> registration)
		{
			IGroupedEntry entry;
			if (_index.TryGetValue(typeof(T), out entry))
			{
				return ((GroupedEntry<T>)entry).TryGet(key, out registration);
			}

			registration = null;
			return false;
		}

		public IEnumerable<IRegistration> GetAll(Type type)
		{
			foreach (var registration in _index[type])
			{
				yield return registration;
			}
		}
		public IEnumerable<IRegistration<T>> GetAll<T>()
		{
			foreach (var registration in (GroupedEntry<T>)_index[typeof(T)])
			{
				yield return registration;
			}
		}

		public IEnumerable<IRegistration> TryGetAll(Type type)
		{
			IGroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				foreach (var registration in entry)
				{
					yield return registration;
				}
			}
		}
		public IEnumerable<IRegistration<T>> TryGetAll<T>()
		{
			IGroupedEntry entry;
			if (_index.TryGetValue(typeof(T), out entry))
			{
				foreach (var registration in (GroupedEntry<T>)entry)
				{
					yield return registration;
				}
			}
		}

		public bool Contains(Type type)
		{
			IRegistration registration;
			return TryGet(type, out registration);
		}
		public bool Contains(Type type, object key)
		{
			IRegistration registration;
			return TryGet(type, key, out registration);
		}
		public bool Contains<T>()
		{
			IRegistration registration;
			return TryGet(typeof(T), out registration);
		}
		public bool Contains<T>(object key)
		{
			IRegistration registration;
			return TryGet(typeof(T), key, out registration);
		}

		public bool ContainsAny(Type type)
		{
			return _index.ContainsKey(type);
		}
		public bool ContainsAny<T>()
		{
			return _index.ContainsKey(typeof(T));
		}

		public IEnumerator<IRegistration> GetEnumerator()
		{
			foreach (var entryValue in _index.Values)
			{
				foreach (var registration in entryValue)
				{
					yield return registration;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
