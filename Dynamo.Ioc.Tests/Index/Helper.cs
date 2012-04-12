using System.Collections.Generic;
using Dynamo.Ioc.Index;

namespace Dynamo.Ioc.Tests.Index
{
	public static class Helper
	{
		public static IEnumerable<IIndex> GetIndexes()
		{
			// Returns all index implementations that should be tested
			return new IIndex[] { new GroupedIndex(), new DirectIndex() };
		}
	}
}
