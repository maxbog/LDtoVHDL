using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	[FactoryFor("inVariable")]
	class InVariableBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new InVariableBlock(GetBlockLocalId(xBlock), varName, env.ReadableVariables[varName].SignalType);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}