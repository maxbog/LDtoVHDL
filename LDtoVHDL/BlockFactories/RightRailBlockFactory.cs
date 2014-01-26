using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	class RightRailBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes
		{
			get { yield return RightRailBlock.TYPE; }
		}

		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new RightRailBlock(GetBlockLocalId(xBlock));
		}
	}
}