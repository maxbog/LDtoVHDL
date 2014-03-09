using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public class BlockBuilder
	{
		private readonly ObjectDictionary<string, IBlockFactory, FactoryForAttribute> m_factories;

		public BlockBuilder()
		{
			m_factories = ObjectDictionary<string, IBlockFactory, FactoryForAttribute>.FromExecutingAssembly(ffa => ffa.CreatedType);
		}

		public BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var elementName = xBlock.Name.LocalName;
			var elementType = elementName == "block" 
				? (string) xBlock.Attribute("typeName") 
				: elementName;
			return m_factories.Get(elementType).CreateBlock(xBlock, env);
		}

	}
}