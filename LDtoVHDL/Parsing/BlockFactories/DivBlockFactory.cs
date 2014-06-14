using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("DIV")]
	public class DivBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			yield return new DivBlock(GetBlockLocalId(xBlock));
		}
	}
}