using System;
using System.Collections;
using System.Collections.Generic;

namespace Dynamo.Ioc.Index
{
	public class GroupedEntry : IEnumerable<IRegistration>
	{
		#region Fields
		private IRegistration _default;
		private readonly Dictionary<object, IRegistration> _keyed = new Dictionary<object, IRegistration>();
		#endregion

		#region Methods
		public void Add(IRegistration registration, object key)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			if (key == null)
			{
				// Default
				if (_default != null)
					throw new ArgumentException("Default is already registered for Type: " + registration.ReturnType);

				_default = registration;
			}
			else
			{
				// Keyed
				_keyed.Add(key, registration);
			}
		}

		public IRegistration Get()
		{
			if (_default == null)
				throw new KeyNotFoundException("The given key was not present in the dictionary.");

			return _default;
		}
		public IRegistration Get(object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return _keyed[key];
		}

		public bool TryGet(out IRegistration registration)
		{
			return (registration = _default) != null;
		}
		public bool TryGet(object key, out IRegistration registration)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return _keyed.TryGetValue(key, out registration);
		}

		public IEnumerator<IRegistration> GetEnumerator()
		{
			if (_default != null)
				yield return _default;

			foreach (var reg in _keyed.Values)
			{
				yield return reg;
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}