using System.Collections.Generic;

namespace Dynamo.Ioc.Index
{
	public interface IGroupedEntry : IEnumerable<IRegistration>
	{
		IRegistration Get();
		IRegistration Get(object key);

		bool TryGet(out IRegistration registration);
		bool TryGet(object key, out IRegistration registration);
	}
}
