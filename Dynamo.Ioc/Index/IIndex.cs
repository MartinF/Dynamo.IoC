using System;

namespace Dynamo.Ioc.Index
{
	public interface IIndex : IIndexReader, IIndexBuilder, IDisposable
	{
	}
}