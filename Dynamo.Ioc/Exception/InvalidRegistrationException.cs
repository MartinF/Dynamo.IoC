using System;

// Raised when verifying registrations etc.

namespace Dynamo.Ioc
{
	public class InvalidRegistrationException : Exception
	{
		public InvalidRegistrationException(IRegistration registration)
			: base(GetMessage(registration))
		{
		}

		public InvalidRegistrationException(IRegistration registration, Exception innerException)
			: base(GetMessage(registration), innerException)
		{
		}

		private static string GetMessage(IRegistration registration)
		{
			var msg = "Registration for type: " + registration.ReturnType;

			if (registration.Key != null)
				msg += " with key: " + registration.Key;

			msg += " is invalid.";

			return msg;
		}
	}
}
