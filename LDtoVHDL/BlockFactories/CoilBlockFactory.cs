using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	class CoilBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes { get { yield return CoilBlock.TYPE; } }
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new CoilBlock(GetBlockLocalId(xBlock), varName);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("variable".XName()).Single();
		}
	}
}