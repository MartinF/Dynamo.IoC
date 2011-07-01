using System;

namespace Dynamo.Ioc
{
	public class RegistrationException : Exception
	{
		private readonly string _errorMessage;

		public RegistrationException(Type type, object key, Exception innerException = null) : base(null, innerException)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			_errorMessage = "Registration for Type: " + type.Name;
			if (key != null)
				_errorMessage += " with Key: " + key;

			_errorMessage += " is not valid.\n";
		}

		public RegistrationException(Type type, object key, string additionalErrorDescription) : this(type, key)
		{
			if (additionalErrorDescription == null)
				throw new ArgumentNullException("additionalErrorDescription");
			
			_errorMessage += additionalErrorDescription;
		}

		public RegistrationException(IRegistrationInfo info, Exception innerException = null) : this(info.Type, info.Key, innerException)
		{
		}

		public RegistrationException(IRegistrationInfo info, string additionalErrorDescription) : this(info.Type, info.Key, additionalErrorDescription)
		{
		}

		public override string Message
		{
			get { return _errorMessage; }
		}
	}
}
