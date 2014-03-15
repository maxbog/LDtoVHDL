using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LDtoVHDL.TypeFinder
{
	public class ObjectCollection<T> : IEnumerable<T>
	{
		private readonly List<T> m_objects = new List<T>();

		private ObjectCollection()
		{
			foreach (var phaseType in TypeFinder.FindChildrenOf<T>())
				m_objects.Add(InstantiateObject(phaseType));
		}

		private static T InstantiateObject(Type phaseType)
		{
			var constructorInfo = phaseType.GetConstructor(new Type[] {});
			Debug.Assert(constructorInfo != null, "constructorInfo != null");
			return (T) constructorInfo.Invoke(new object[] {});
		}

		public static ObjectCollection<T> FromExecutingAssembly()
		{
			return new ObjectCollection<T>();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return m_objects.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}