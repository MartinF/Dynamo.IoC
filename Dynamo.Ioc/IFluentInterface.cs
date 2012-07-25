using System;
using System.ComponentModel;

// Used for hiding the methods in the intellisense to get a more fluent syntax feel

namespace Dynamo.Ioc.Fluent
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