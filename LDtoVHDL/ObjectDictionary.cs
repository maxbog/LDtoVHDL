using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LDtoVHDL
{
	public class ObjectDictionary<TKey, TValue, TAttribute> where TKey : class
	{
		private readonly Dictionary<TKey, TValue> m_objects = new Dictionary<TKey, TValue>();
		private readonly Func<TKey, TKey> m_parentRetriever;

		private ObjectDictionary(Func<TKey, TKey> parentRetriever, Func<TAttribute, TKey> keySelector, object[] valueConstructorParams)
		{
			m_parentRetriever = parentRetriever;
			FillObjects(keySelector, valueConstructorParams);
		}

		private void FillObjects(Func<TAttribute, TKey> keySelector, object[] valueConstructorParams)
		{
			foreach (var valueType in TypeFinder.FindChildrenOf<TValue>().WithAttributes<TAttribute>())
				AddObject(keySelector, valueConstructorParams, valueType.Item1, valueType.Item2);
		}

		private void AddObject(Func<TAttribute, TKey> keySelector, object[] valueConstructorParams, Type valueType, IEnumerable<TAttribute> attributes)
		{
			var value = InstantiateObject(valueConstructorParams, valueType);
			foreach (var key in attributes.Select(keySelector))
				m_objects.Add(key, value);
		}

		private static TValue InstantiateObject(object[] valueConstructorParams, Type objectType)
		{
			var constructorInfo = GetValueConstructor(valueConstructorParams, objectType);
			Debug.Assert(constructorInfo != null, "constructorInfo != null");
			return (TValue) constructorInfo.Invoke(valueConstructorParams);
		}

		private static ConstructorInfo GetValueConstructor(IEnumerable<object> valueConstructorParams, Type type)
		{
			return type.GetConstructor(valueConstructorParams.Select(obj => obj.GetType()).ToArray());
		}


		public TValue Get(TKey key)
		{
			return m_parentRetriever == null 
				? m_objects[key] 
				: GetThisOrParent(key);
		}

		private TValue GetThisOrParent(TKey key)
		{
			if (!m_objects.ContainsKey(key))
				UseParentFor(key);

			return m_objects[key];
		}

		private void UseParentFor(TKey key)
		{
			var parent = FindStoredParent(key);
			if (parent != null)
				m_objects.Add(key, m_objects[parent]);
		}

		private TKey FindStoredParent(TKey key)
		{
			var baseType = key;
			while (baseType != null && !m_objects.ContainsKey(baseType))
				baseType = m_parentRetriever(baseType);
			return baseType;
		}

		public static ObjectDictionary<TKey, TValue, TAttribute> FromExecutingAssembly(Func<TKey, TKey> parentRetriever, Func<TAttribute, TKey> keySelector, object[] valueConstructorParams = null)
		{
			return new ObjectDictionary<TKey, TValue, TAttribute>(parentRetriever, keySelector, valueConstructorParams ?? new object[]{});
		}

		public static ObjectDictionary<TKey, TValue, TAttribute> FromExecutingAssembly(Func<TAttribute, TKey> keySelector, object[] valueConstructorParams = null)
		{
			return new ObjectDictionary<TKey, TValue, TAttribute>(null, keySelector, valueConstructorParams ?? new object[] { });
		}
	}
}