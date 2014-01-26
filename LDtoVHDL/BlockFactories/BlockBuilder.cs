using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public class BlockBuilder
	{
		private readonly Dictionary<string, IBlockFactory> m_factories;

		public BlockBuilder()
		{
			m_factories = new Dictionary<string, IBlockFactory>();
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var factoryType in types.Where(typeof(IBlockFactory).IsAssignableFrom).Except(Enumerable.Repeat(typeof(IBlockFactory), 1)))
			{
				var factory = (IBlockFactory)factoryType.GetConstructor(new Type[] {}).Invoke(new object[] {});
				foreach (var type in factory.BlockTypes)
					m_factories.Add(type, factory);
			}
		}

		public BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var elementName = xBlock.Name.LocalName;
			var elementType = elementName == "block" 
				? (string) xBlock.Attribute("typeName") 
				: elementName;
			return m_factories[elementType].CreateBlock(xBlock, env);
		}


	}
}