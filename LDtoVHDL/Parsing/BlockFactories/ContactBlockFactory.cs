using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("contact")]
	class ContactBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			var varName = GetVariableName(xBlock);
			if(IsNegated(xBlock))
				yield return new NccBlock(GetBlockLocalId(xBlock), varName);
			else if (IsRisingEdge(xBlock))
				yield return new PcBlock(GetBlockLocalId(xBlock), varName);
			else if (IsFallingEdge(xBlock))
				yield return new NcBlock(GetBlockLocalId(xBlock), varName);
			else
				yield return new NocBlock(GetBlockLocalId(xBlock), varName);
		}

		private bool IsRisingEdge(XElement xBlock)
		{
			var negatedAttr = xBlock.Attribute("edge");
			if (negatedAttr == null)
				return false;
			return (string) negatedAttr == "rising";
		}

		private bool IsFallingEdge(XElement xBlock)
		{
			var negatedAttr = xBlock.Attribute("edge");
			if (negatedAttr == null)
				return false;
			return (string)negatedAttr == "falling";
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