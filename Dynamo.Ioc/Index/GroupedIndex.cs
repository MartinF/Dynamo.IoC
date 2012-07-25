using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// It is the best all-round with both Get() and Get<T>() etc.
// DirectIndex is a little bit faster when it comes to the Get() method but a lot slower using Get<T>()

namespace Dynamo.Ioc.Index
{
	public class GroupedIndex : IIndex
	{
		#region Fields
		private readonly Dictionary<Type, GroupedEntry> _index = new Dictionary<Type, GroupedEntry>();
		#endregion

		#region Methods
		public void Add(IRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			var type = registration.ReturnType;
			var key = registration.Key;

			GroupedEntry oldEntry;
			if (_index.TryGetValue(type, out oldEntry))
			{
				// Add to already existing entry
				oldEntry.Add(registration, key);
			}
			else
			{
				// Add new entry
				var newEntry = new GroupedEntry();
				newEntry.Add(registration, key);
				_index.Add(type, newEntry);
			}
		}

		public IRegistration Get(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _index[type].Get();
		}
		public IRegistration Get(Type type, object key)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return _index[type].Get(key);
		}
		public IRegistration Get<T>()
		{
			return _index[typeof(T)].Get();
		}
		public IRegistration Get<T>(object key)
		{
			return _index[typeof(T)].Get(key);
		}

		public bool TryGet(Type type, out IRegistration registration)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			GroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry.TryGet(out registration);
			}

			registration = null;
			return false;
		}
		public bool TryGet(Type type, object key, out IRegistration registration)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			GroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry.TryGet(key, out registration);
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

			return _index[type];
		}
		public IEnumerable<IRegistration> GetAll<T>()
		{
			return _index[typeof(T)];
		}

		public IEnumerable<IRegistration> TryGetAll(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			GroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry;
			}

			return Enumerable.Empty<IRegistration>();
		}
		public IEnumerable<IRegistration> TryGetAll<T>()
		{
			return TryGetAll(typeof(T));
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
			return Contains(typeof(T));
		}
		public bool Contains<T>(object key)
		{
			return Contains(typeof(T), key);
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

		public void Dispose()
		{
			foreach (var reg in this)
			{
				reg.Dispose();
			}

			_index.Clear();
		}
	}
}
