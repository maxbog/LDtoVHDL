using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	class LeftRailBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes
		{
			get { yield return LeftRailBlock.TYPE; }
		}

		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new LeftRailBlock(GetBlockLocalId(xBlock));
		}
	}
}