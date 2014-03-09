using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	[FactoryFor("coil")]
	class CoilBlockFactory : BaseBlockFactory
	{
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