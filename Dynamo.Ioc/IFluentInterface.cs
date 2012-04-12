using System;
using System.ComponentModel;

// Keep or remove ?
// Used for hiding the methods in the intellisense to get a more fluent syntax feel

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