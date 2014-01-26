using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	class ContactBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<string> BlockTypes { get { yield return ContactBlock.TYPE; } }
		public override BaseBlock CreateBlock(XElement xBlock, Environment env)
		{
			var varName = GetVariableName(xBlock);
			return new ContactBlock(GetBlockLocalId(xBlock), varName);
		}

		private string GetVariableName(XElement xBlock)
		{
			return (string)xBlock.Descendants("variable".XName()).Single();
		}
	}
}