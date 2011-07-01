using System;
using System.ComponentModel;

// TODO: Keep or remove ?
// Used for hiding the methods in the intellisense

namespace Dynamo.Ioc
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IFluentInterface
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);
	}
}