using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[FactoryFor("MOD")]
	public class ModBlockFactory : BaseBlockFactory
	{
		public override IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env)
		{
			yield return new ModBlock(GetBlockLocalId(xBlock));
		}
	}
}