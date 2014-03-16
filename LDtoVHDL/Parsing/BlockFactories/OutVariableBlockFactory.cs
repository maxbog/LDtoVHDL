using System;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("outVariable")]
	class OutVariableBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Program env)
		{
			var expression = GetVariableName(xBlock);
			var blockLocalId = GetBlockLocalId(xBlock);
			if (expression[0] == '%')
				return new OutVariableBlock(blockLocalId, expression.Substring(1), env.ReadableVariables[expression.Substring(1)].SignalType);
			throw new InvalidOperationException("OutVariable must write to a variable");
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}