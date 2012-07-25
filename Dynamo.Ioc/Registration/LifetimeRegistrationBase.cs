using System;

namespace Dynamo.Ioc
{
	public abstract class LifetimeRegistrationBase<T> : RegistrationBase<T>, ILifetimeRegistration
	{
		protected ILifetime _lifetime;

		protected LifetimeRegistrationBase(ILifetime lifetime, object key = null)
			: base(key)
		{
			if (lifetime == null)
				throw new ArgumentNullException("lifetime");

			Lifetime = lifetime;
		}

		public ILifetime Lifetime
		{
			get { return _lifetime; }
			set
			{
				// Currently allows changing lifetime no matter if compiled or not and/or used in other compiled registrations
				// The change wont be reflected if changed after it have been compiled (and not re-compiled) which is a problem.
				// Throw an exception ? or just  dont care ? or recompile (if already compiled - requires all registrations to be recompiled as they might refer to this registration)

				if (value == null)
					throw new ArgumentNullException("lifetime");

				// Dispose the lifetime already set?
				if (_lifetime != null)
					_lifetime.Dispose();

				_lifetime = value;
			}
		}

		public abstract object CreateInstance();

		public override bool Verify()
		{
			// Only verifies that it can create instances
			// Tries to dispose instance created if possible

			object instance = CreateInstance();

			var result = instance is T;

			var disposable = instance as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}

			return result;
		}

		public override void Dispose()
		{
			if (_lifetime != null)
			{
				_lifetime.Dispose();
				_lifetime = null;
			}
		}
	}
}
