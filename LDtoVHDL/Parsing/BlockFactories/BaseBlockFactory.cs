using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	public abstract class BaseBlockFactory : IBlockFactory
	{
		public abstract BaseBlock CreateBlock(XElement xBlock, Program env);

		protected string GetBlockLocalId(XElement xBlock)
		{
			return (string)xBlock.Attribute("localId");
		}
	}
}