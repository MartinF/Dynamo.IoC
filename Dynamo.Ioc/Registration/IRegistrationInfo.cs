using System;

namespace Dynamo.Ioc
{
	public interface IRegistrationInfo
	{
		Type Type { get; }
		object Key { get; }
	}
}