using System;

// Doesnt need to be generic, but helps to ensure the contraint and define the return type

namespace Dynamo.Ioc
{
	public class InstanceRegistration<T> : IRegistration
	{
		#region Fields
		private readonly Type _implementationType;
		private readonly Type _returnType;
		private readonly object _instance;
		#endregion

		#region Constructors
		public InstanceRegistration(T instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			_instance = instance;

			_returnType = typeof(T);
			_implementationType = instance.GetType();
		}
		#endregion

		#region Properties
		public Type ImplementationType { get { return _implementationType; } }
		public Type ReturnType { get { return _returnType; } }
		#endregion

		#region Methods
		public object GetInstance(IResolver resolver)
		{
			return _instance;
		}
	
		//public void Dispose()
		//{
		//    // Remove this ?

		//    // Cant be sure that all instances registered should be disposed just because the container/registration is being disposed ?
		//        // Either you should be able to configure it or it should be removed ?
		//        // Resources shouldnt be open for the lifetime of the container anyway should they ?

		//    //var disposable = _instance as IDisposable;
		//    //if (disposable != null)
		//    //    disposable.Dispose();
		//}
		#endregion
	}
}
