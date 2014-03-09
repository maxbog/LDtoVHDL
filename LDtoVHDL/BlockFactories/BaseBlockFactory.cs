using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public abstract class BaseBlockFactory : IBlockFactory
	{
		public abstract BaseBlock CreateBlock(XElement xBlock, Environment env);

		protected string GetBlockLocalId(XElement xBlock)
		{
			return (string)xBlock.Attribute("localId");
		}
	}

	[FactoryFor("ADD")]
	class AddBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new AddBlock(GetBlockLocalId(xBlock));
		}
	}

	[FactoryFor("TON")]
	class TonBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new TonBlock(GetBlockLocalId(xBlock), GetInstanceName(xBlock));
		}

		private string GetInstanceName(XElement xBlock)
		{
			return (string)xBlock.Attribute("instanceName");
		}
	}
}