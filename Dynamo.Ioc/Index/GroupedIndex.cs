using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Improve the GroupedEntry and work with it directly instead of through methods - just expose the default and keyed collection - only a small performance improvement

// Change name to GetDefault / GetKeyed ? ContainsDefault / ContainsKeyed to make it more clear ?

// Make methods virtual ?

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
			GroupedEntry entry;
			if (_index.TryGetValue(registration.Type, out entry))
			{
				// Add to already existing entry
				entry.Add(registration);
			}
			else
			{
				// Add new entry
				entry = new GroupedEntry();
				entry.Add(registration);
				_index.Add(registration.Type, entry);
			}
		}

		public IRegistration Get(Type type)
		{
			return _index[type].GetDefault();
		}

		public IRegistration Get(Type type, object key)
		{
			return _index[type].GetKeyed(key);
		}

		public bool TryGet(Type type, out IRegistration registration)
		{
			GroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry.TryGetDefault(out registration);
			}

			registration = null;
			return false;
		}

		public bool TryGet(Type type, object key, out IRegistration registration)
		{
			GroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				return entry.TryGetKeyed(key, out registration);
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

		public IEnumerable<IRegistration> TryGetAll(Type type)
		{
			GroupedEntry entry;
			if (_index.TryGetValue(type, out entry))
			{
				foreach (var registration in entry)
				{
					yield return registration;
				}
			}
		}

		public bool Contains(Type type)
		{
			// Improve ! shouldnt return the registration

			// Either create Contains method on GroupedEntry
			// Or just enumerate it and use linq ?

			IRegistration registration;
			return TryGet(type, out registration);
		}

		public bool Contains(Type type, object key)
		{
			// Improve ! shouldnt return the registration

			// Either create Contains method on GroupedEntry
			// Or just enumerate it and use linq ?

			IRegistration registration;
			return TryGet(type, key, out registration);
		}

		public bool ContainsAny(Type type)
		{
			// Improve - could make ContainsAny method on the GroupedEntry
			// Or access the default and keyed collections directly from here instead

			// Just check if GroupedEntry exists - if it does it is true ?

			GroupedEntry entry;
			return _index.TryGetValue(type, out entry);

			//return TryGetAll(type).Any();
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
