using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("contact")]
	class ContactBlockFactory : BaseBlockFactory
	{
		public override BaseBlock CreateBlock(XElement xBlock, Program env)
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