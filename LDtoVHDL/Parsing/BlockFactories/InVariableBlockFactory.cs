using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("inVariable")]
	class InVariableBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Program env)
		{
			var expression = GetExpression(xBlock);

			var blockLocalId = GetBlockLocalId(xBlock);
			if (expression[0] == '%')
				return new InVariableBlock(blockLocalId, expression.Substring(1), env.ReadableVariables[expression.Substring(1)].SignalType);

			bool boolValue;
			if (bool.TryParse(expression, out boolValue))
				return new ConstantBlock(blockLocalId, BuiltinType.Boolean, boolValue);

			if (expression.StartsWith("BOOL#", StringComparison.InvariantCultureIgnoreCase))
				return CreateBoolBlock(blockLocalId, expression.Substring(5));

			if (expression.StartsWith("DINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("INT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("SINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("UDINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("UINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("USINT#", StringComparison.InvariantCultureIgnoreCase))
			{
				var type = expression.Substring(0, expression.IndexOf('#'));
				return CreateIntBlock(blockLocalId, expression.Substring(expression.IndexOf('#') + 1), PlcOpenParser.VarTypes[type]);
			}

			if (expression.StartsWith("T#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("TIME#", StringComparison.InvariantCultureIgnoreCase))
				return CreateTimeBlock(blockLocalId, expression.Substring(expression.IndexOf('#') + 1));

			return CreateIntBlock(blockLocalId, expression.Substring(expression.IndexOf('#') + 1), null);
		}

		private static readonly Dictionary<string, long> Multiplier = new Dictionary<string, long>
		{
			{"us", 10L},
			{"ms", 10L*1000},
			{"s", 10L*1000*1000},
			{"m", 10L*1000*1000*60},
			{"h", 10L*1000*1000*60*60},
			{"d", 10L*1000*1000*60*60*24}
		};

		private BaseBlock CreateTimeBlock(string blockLocalId, string timeValue)
		{
			timeValue = timeValue.Replace("_", "");
			long ticksCount = 0;
			int currentIndex = 0;
			while (currentIndex < timeValue.Length)
			{
				int unitIdx = timeValue.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray(), currentIndex + 1);
				var currentValue = double.Parse(timeValue.Substring(currentIndex, unitIdx));
				int nextValueIndex = timeValue.IndexOfAny("0123456789.".ToCharArray(), unitIdx+1);
				var unit = nextValueIndex == -1 ? timeValue.Substring(unitIdx) : timeValue.Substring(unitIdx, nextValueIndex - 1);
				ticksCount += (long)(currentValue*Multiplier[unit]);
				currentIndex = nextValueIndex == -1 ? timeValue.Length : nextValueIndex;
			}
			return new ConstantBlock(blockLocalId, BuiltinType.Time, new TimeSpan(ticksCount));
		}

		private BaseBlock CreateIntBlock(string blockLocalId, string intValue, SignalType varType)
		{
			intValue = intValue.Replace("_", "");
			if(intValue.StartsWith("2#"))
				return new ConstantBlock(blockLocalId, varType, Convert.ToInt64(intValue, 2));
			if (intValue.StartsWith("8#"))
				return new ConstantBlock(blockLocalId, varType, Convert.ToInt64(intValue, 8));
			if (intValue.StartsWith("16#"))
				return new ConstantBlock(blockLocalId, varType, Convert.ToInt64(intValue, 16));

			return new ConstantBlock(blockLocalId, varType, Convert.ToInt64(intValue, 10));
		}

		private BaseBlock CreateBoolBlock(string blockLocalId, string boolValue)
		{
			return new ConstantBlock(blockLocalId, BuiltinType.Boolean, bool.Parse(boolValue));
		}


		private string GetExpression(XElement xBlock)
		{
			return (string)xBlock.Descendants("expression".XName()).Single();
		}
	}
}