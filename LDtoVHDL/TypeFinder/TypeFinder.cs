using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LDtoVHDL.TypeFinder
{
	public class TypeFinder
	{
		private readonly static Type[] Types = Assembly.GetExecutingAssembly().GetTypes();
		public static IEnumerable<Type> FindChildrenOf<T>()
		{
			return Types.Where(type => typeof (T).IsAssignableFrom(type) && !type.IsAbstract);
		}
	}
}