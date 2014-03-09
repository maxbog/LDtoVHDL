using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	[FactoryFor("outVariable")]
	class OutVariableBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new OutVariableBlock(GetBlockLocalId(xBlock), varName, env.WritableVariables[varName].SignalType);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}