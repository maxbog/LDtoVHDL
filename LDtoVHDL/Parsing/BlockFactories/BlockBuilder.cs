using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;
using LDtoVHDL.TypeFinder;

namespace LDtoVHDL.Parsing.BlockFactories
{
	public class BlockBuilder
	{
		private readonly ObjectDictionary<string, IBlockFactory, FactoryForAttribute> m_factories;

		public BlockBuilder()
		{
			m_factories = ObjectDictionary<string, IBlockFactory, FactoryForAttribute>.FromExecutingAssembly(ffa => ffa.CreatedType);
		}

		public IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			var elementName = xBlock.Name.LocalName;
			var elementType = elementName == "block" 
				? (string) xBlock.Attribute("typeName") 
				: elementName;
			return m_factories.Get(elementType).CreateBlock(xBlock, env);
		}

	}
}