using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("SUB")]
	public class SubBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			yield return new SubBlock(GetBlockLocalId(xBlock));
		}
	}
}