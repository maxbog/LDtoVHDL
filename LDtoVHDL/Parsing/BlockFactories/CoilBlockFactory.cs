using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("coil")]
	class CoilBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			var varName = GetVariableName(xBlock);
			if (IsNegated(xBlock))
				yield return new NegatedCoilBlock(GetBlockLocalId(xBlock), varName);
			else
				yield return new CoilBlock(GetBlockLocalId(xBlock), varName);
		}
		
		private bool IsNegated(XElement xBlock)
		{
			var negatedAttr = xBlock.Attribute("negated");
			if (negatedAttr == null)
				return false;
			return (bool)negatedAttr;
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("variable".XName()).Single();
		}
	}
}