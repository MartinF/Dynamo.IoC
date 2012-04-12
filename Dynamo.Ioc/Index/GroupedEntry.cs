using System;
using System.Collections;
using System.Collections.Generic;

namespace Dynamo.Ioc.Index
{
	public class GroupedEntry<T> : IGroupedEntry, IEnumerable<IRegistration<T>>
	{
		#region Fields
		private IRegistration<T> _default;
		private readonly Dictionary<object, IRegistration<T>> _keyed = new Dictionary<object, IRegistration<T>>();
		#endregion

		#region Methods
		public void Add(IRegistration<T> registration, object key)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			if (key == null)
			{
				// Default
				if (_default != null)
					throw new ArgumentException("Default is already registered for Type: " + typeof(T).Name);

				_default = registration;
			}
			else
			{
				// Keyed
				_keyed.Add(key, registration);
			}
		}

		IRegistration IGroupedEntry.Get()
		{
			if (_default == null)
				throw new KeyNotFoundException("The given key was not present in the dictionary.");

			return _default;
		}
		IRegistration IGroupedEntry.Get(object key)
		{
			return _keyed[key];
		}

		public bool TryGet(out IRegistration registration)
		{
			return (registration = _default) != null;
		}
		public bool TryGet(object key, out IRegistration registration)
		{
			IRegistration<T> reg;
			if (_keyed.TryGetValue(key, out reg))
			{
				registration = reg;
				return true;
			}

			registration = null;
			return false;
		}

		public IRegistration<T> Get()
		{
			if (_default == null)
				throw new KeyNotFoundException("The given key was not present in the dictionary.");

			return _default;
		}
		public IRegistration<T> Get(object key)
		{
			return _keyed[key];
		}

		public bool TryGet(out IRegistration<T> registration)
		{
			return (registration = _default) != null;
		}
		public bool TryGet(object key, out IRegistration<T> registration)
		{
			return _keyed.TryGetValue(key, out registration);
		}

		public IEnumerator<IRegistration<T>> GetEnumerator()
		{
			if (_default != null)
				yield return _default;

			foreach (var reg in _keyed.Values)
			{
				yield return reg;
			}
		}
		IEnumerator<IRegistration> IEnumerable<IRegistration>.GetEnumerator()
		{
			return GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}