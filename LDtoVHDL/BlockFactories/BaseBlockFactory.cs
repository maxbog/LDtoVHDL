using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public class BaseBlockFactory : IBlockFactory
	{
		public virtual IEnumerable<string> BlockTypes
		{
			get
			{
				yield return BaseBlock.COIL;
				yield return BaseBlock.CONTACT;
				yield return BaseBlock.LEFT_RAIL;
				yield return BaseBlock.RIGHT_RAIL;
				yield return BaseBlock.ADD;
			}
		}
		public virtual BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			return new BaseBlock(GetBlockLocalId(xBlock), xBlock.Name.LocalName);
		}

		protected string GetBlockLocalId(XElement xBlock)
		{
			return (string)xBlock.Attribute("localId");
		}
	}

	class OutVariableBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes { get { yield return BaseBlock.OUT_VARIABLE; } }
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new OutVariableBlock(GetBlockLocalId(xBlock), varName, env.Variables[varName].SignalWidth);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}

	class InVariableBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes { get { yield return BaseBlock.IN_VARIABLE; } }
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new InVariableBlock(GetBlockLocalId(xBlock), varName, env.Variables[varName].SignalWidth);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}