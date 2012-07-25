using System;

// Doesnt need to be generic, but helps to ensure the contraint and define the return type

namespace Dynamo.Ioc
{
	public class InstanceRegistration<T> : RegistrationBase<T>
	{
		#region Fields
		private readonly object _instance;
		#endregion

		#region Constructors
		public InstanceRegistration(T instance, object key = null) 
			: base(key: key)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			_instance = instance;
		}
		#endregion

		#region Methods
		public override object GetInstance()
		{
			return _instance;
		}
		
		public override bool Verify()
		{
			return true;
		}

		//public override void Dispose()
		//{
		//	var disposable = _instance as IDisposable;
		//	if (disposable != null)
		//	{
		//		disposable.Dispose();
		//		_instance = null;
		//	}
		//}
		#endregion
	}
}
