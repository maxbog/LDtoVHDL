using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	class OutVariableBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes { get { yield return OutVariableBlock.TYPE; } }
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new OutVariableBlock(GetBlockLocalId(xBlock), varName, env.Variables[varName].SignalType);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}