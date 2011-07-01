using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Make Methods virtual ?

namespace Dynamo.Ioc.Index
{
	public class GroupedEntry : IEnumerable<IRegistration>
	{
		#region Fields
		private IRegistration _default;
		private readonly Dictionary<object, IRegistration> _keyed = new Dictionary<object, IRegistration>();
		#endregion

		#region Methods
		public void Add(IRegistration registration)
		{
			if (registration == null)
				throw new ArgumentNullException("registration");

			if (registration.Key == null)
			{
				// Default
				if (_default != null)
					throw new ArgumentException("Default is already registered for Type: " + registration.Type.Name);
				_default = registration;
			}
			else
			{
				// Keyed
				_keyed.Add(registration.Key, registration);
			}
		}

		public IRegistration GetDefault()
		{
			if (_default == null)
				throw new KeyNotFoundException("The given key was not present in the dictionary.");

			return _default;
		}

		public IRegistration GetKeyed(object key)
		{
			return _keyed[key];
		}

		public bool TryGetDefault(out IRegistration registration)
		{
			registration = _default;
			return (registration != null);
		}

		public bool TryGetKeyed(object key, out IRegistration registration)
		{
			return _keyed.TryGetValue(key, out registration);
		}

		//public bool ContainsDefault()
		//{
		//    return _default != null;
		//}

		//public bool ContainsNamed(string name)
		//{
		//    return _named.ContainsKey(name);
		//}

		// ContainsAny / Contains () ?

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