using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("MUL")]
	public class MulBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			yield return new MulBlock(GetBlockLocalId(xBlock));
		}
	}
}