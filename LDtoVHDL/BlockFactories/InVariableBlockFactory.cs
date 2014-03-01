using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	class InVariableBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes { get { yield return InVariableBlock.TYPE; } }
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new InVariableBlock(GetBlockLocalId(xBlock), varName, env.Variables[varName].SignalType);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}