using System;
using System.Collections.Generic;
using System.Linq;

namespace LDtoVHDL
{
	public static class EnumerableEx
	{
		public static IEnumerable<Tuple<Type, IEnumerable<T>>> WithAttributes<T>(this IEnumerable<Type> @this)
		{
			return @this.Select(type => Tuple.Create(type, GetAttributesOfType<T>(type))).Where(tuple => tuple.Item2.Any());
		}

		public static IEnumerable<T> GetAttributesOfType<T>(this Type type)
		{
			return type.GetCustomAttributes(typeof (T), false).OfType<T>();
		}
	}
}