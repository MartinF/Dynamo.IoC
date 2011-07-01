
// Create non generic IKeyResolver ?

namespace Dynamo.Ioc
{
	public interface IKeyResolver<out T, in TKey>
	{
		T this[TKey key] { get; }

		//T Resolve(TKey key);
		//bool TryResolve(TKey key, out T instance);
	}

	//public interface IKeyResolver : IKeyResolver<object, object>
	//{
	//}
}
