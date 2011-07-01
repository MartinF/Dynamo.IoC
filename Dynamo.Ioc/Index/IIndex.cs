using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Should the IIndex be resposible for disposing all registrations ? 

namespace Dynamo.Ioc.Index
{
	public interface IIndex : IIndexAccessor, IIndexBuilder
	{
		// Implement IDisposable ?

		// Or/And
		
		// Clear() and Remove() methods ? 
	}
}