using System;
using System.Collections.Generic;
using Dynamo.Ioc.Index;

namespace Dynamo.Ioc
{
	public static class IndexAccessorExtensions
	{
		#region Get
		public static IRegistration Get<T>(this IIndexAccessor index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.Get(typeof(T));
		}
		public static IRegistration Get<T>(this IIndexAccessor index, object key)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.Get(typeof(T), key);
		}
		public static IRegistration Get(this IIndexAccessor index, IRegistrationInfo info)
		{
			if (index == null)
				throw new ArgumentNullException("index");
			if (info == null)
				throw new ArgumentNullException("info");

			return info.Key == null ? index.Get(info.Type) : index.Get(info.Type, info.Key);
		}
		#endregion

		#region TryGet
		public static bool TryGet<T>(this IIndexAccessor index, out IRegistration registration)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.TryGet(typeof(T), out registration);
		}
		public static bool TryGet<T>(this IIndexAccessor index, object key, out IRegistration registration)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.TryGet(typeof(T), key, out registration);
		}
		public static bool TryGet(this IIndexAccessor index, IRegistrationInfo info, out IRegistration registration)
		{
			if (index == null)
				throw new ArgumentNullException("index");
			if (info == null)
				throw new ArgumentNullException("info");

			return info.Key == null ? index.TryGet(info.Type, out registration) : index.TryGet(info.Type, info.Key, out registration);
		}
		#endregion

		#region GetAll
		public static IEnumerable<IRegistration> GetAll<T>(this IIndexAccessor index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.GetAll(typeof(T));
		}
		#endregion

		#region TryGetAll
		public static IEnumerable<IRegistration> TryGetAll<T>(this IIndexAccessor index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.TryGetAll(typeof(T));
		}
		#endregion

		#region Contains -Any
		public static bool Contains<T>(this IIndexAccessor index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.Contains(typeof(T));
		}
		public static bool Contains<T>(this IIndexAccessor index, object key)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.Contains(typeof(T), key);
		}
		public static bool Contains(this IIndexAccessor index, IRegistrationInfo info)
		{
			if (index == null)
				throw new ArgumentNullException("index");
			if (info == null)
				throw new ArgumentNullException("info");

			return info.Key == null ? index.Contains(info.Type) : index.Contains(info.Type, info.Key);
		}

		public static bool ContainsAny<T>(this IIndexAccessor index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			return index.ContainsAny(typeof(T));
		}
		#endregion
	}
}
