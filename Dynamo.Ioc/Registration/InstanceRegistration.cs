using System;

namespace Dynamo.Ioc
{
	public class InstanceRegistration<T> : IRegistration<T>
	{
		#region Fields
		private readonly Type _implementationType;
		private readonly Type _returnType;
		private readonly T _instance;
		#endregion

		#region Constructors
		public InstanceRegistration(T instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			_instance = instance;

			_returnType = typeof(T);
			_implementationType = _instance.GetType();
		}
		#endregion

		#region Properties
		public Type ImplementationType { get { return _implementationType; } }
		public Type ReturnType { get { return _returnType; } }
		#endregion

		#region Methods
		public T GetInstance(IResolver resolver)
		{
			return _instance;
		}

		object IRegistration.GetInstance(IResolver resolver)
		{
			// In case of struct / value type 
			// could store instance as object when created to not get hit by the boxing (from int etc to object) every time GetInstance is called?
			return _instance;
		}
	
		public void Dispose()
		{
			// Remove this ?

			// Cant be sure that all instances registered should be disposed just because the container/registration is being disposed ?
				// Either you should be able to configure it or it should be removed ?
				// Resources shouldnt be open for the lifetime of the container anyway should they ?

			//var disposable = _instance as IDisposable;
			//if (disposable != null)
			//    disposable.Dispose();
		}
		#endregion
	}
}
