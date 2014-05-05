using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("CTU")]
	class CounterUpBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Program env)
		{
			return new CtuBlock(GetBlockLocalId(xBlock), GetInstanceName(xBlock));
		}

		private string GetInstanceName(XElement xBlock)
		{
			return (string)xBlock.Attribute("instanceName");
		}
	}

	[FactoryFor("CTD")]
	class CounterDownBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Program env)
		{
			return new CtdBlock(GetBlockLocalId(xBlock), GetInstanceName(xBlock));
		}

		private string GetInstanceName(XElement xBlock)
		{
			return (string)xBlock.Attribute("instanceName");
		}
	}
}