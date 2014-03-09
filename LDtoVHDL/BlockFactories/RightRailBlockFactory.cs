using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	[FactoryFor("rightPowerRail")]
	class RightRailBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new RightRailBlock(GetBlockLocalId(xBlock));
		}
	}
}