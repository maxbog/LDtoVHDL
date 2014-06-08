using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("inVariable")]
	class InVariableBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			var expression = GetExpression(xBlock);

			var blockLocalId = GetBlockLocalId(xBlock);
			if (expression[0] == '%')
				yield return new InVariableBlock(blockLocalId, expression.Substring(1), env.ReadableVariables[expression.Substring(1)].SignalType);
			else
			{
				var parsedExpression = PlcOpenParser.ParseExpression(expression);
				yield return new ConstantBlock(blockLocalId, parsedExpression.Item1, parsedExpression.Item2);
			}
		}


		private string GetExpression(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}