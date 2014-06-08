using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("TON")]
	class TonBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			yield return new TonBlock(GetBlockLocalId(xBlock), GetInstanceName(xBlock));
		}

		private string GetInstanceName(XElement xBlock)
		{
			return (string)xBlock.Attribute("instanceName");
		}
	}

	[FactoryFor("TOF")]
	class TofBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			yield return new TofBlock(GetBlockLocalId(xBlock), GetInstanceName(xBlock));
		}

		private string GetInstanceName(XElement xBlock)
		{
			return (string)xBlock.Attribute("instanceName");
		}
	}
}