using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public abstract class BaseBlockFactory : IBlockFactory
	{
		public abstract IEnumerable<string> BlockTypes { get; }
		public abstract BaseBlock CreateBlock(XElement xBlock, Environment env);

		protected string GetBlockLocalId(XElement xBlock)
		{
			return (string)xBlock.Attribute("localId");
		}
	}

	class AddBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes
		{
			get { yield return AddBlock.TYPE; }
		}

		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new AddBlock(GetBlockLocalId(xBlock));
		}
	}

	class TonBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes
		{
			get { yield return TonBlock.TYPE; }
		}

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