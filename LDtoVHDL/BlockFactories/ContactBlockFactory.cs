using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	[FactoryFor("contact")]
	class ContactBlockFactory : BaseBlockFactory
	{
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