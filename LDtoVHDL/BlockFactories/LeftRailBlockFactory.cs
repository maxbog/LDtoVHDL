using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	[FactoryFor("leftPowerRail")]
	class LeftRailBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new LeftRailBlock(GetBlockLocalId(xBlock));
		}
	}
}