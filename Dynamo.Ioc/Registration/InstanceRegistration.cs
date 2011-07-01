using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dynamo.Ioc
{
	public class InstanceRegistration : IRegistration
	{
		#region Fields
		private readonly Type _type;
		private readonly object _key;
		private readonly object _instance;
		#endregion

		#region Constructors
		public InstanceRegistration(Type type, object instance, object key = null)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			_type = type;
			_instance = instance;
			_key = key;
		}
		#endregion

		#region Properties
		public Type Type { get { return _type; } }
		public object Key { get { return _key; } }
		#endregion

		#region Methods
		public object GetInstance(IResolver resolver)
		{
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
