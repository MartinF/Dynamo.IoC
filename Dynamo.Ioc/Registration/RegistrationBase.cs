using System;

// Doesnt need to be generic
// Currently only required incase fully generic IRegistration<T> should be supported later

namespace Dynamo.Ioc
{
	public abstract class RegistrationBase<T> : IRegistration
	{
		private readonly Type _returnType;
		private readonly object _key;
		
		protected RegistrationBase(object key = null)
		{
			_returnType = typeof(T);
			_key = key;
		}

		public Type ReturnType
		{
			get { return _returnType; }
		}
		public object Key
		{
			get { return _key; }
		}

		public abstract object GetInstance();

		public abstract bool Verify();

		public virtual void Dispose()
		{
			// Only handle disposing of dynamo ioc related references
			// If instance registered uses unmanaged resources and should be disposed the user should implement a finalizer
		}
	}
}