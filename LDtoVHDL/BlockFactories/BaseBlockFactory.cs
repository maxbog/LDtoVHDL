using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public class BaseBlockFactory : IBlockFactory
	{
		public IEnumerable<string> BlockTypes
		{
			get
			{
				yield return "coil";
				yield return "contact";
				yield return "leftPowerRail";
				yield return "rightPowerRail";
				yield return "inVariable";
				yield return "outVariable";
				yield return "ADD";
			}
		}
		public BaseBlock CreateBlock(XElement xBlock)
		{
			return new BaseBlock(GetBlockLocalId(xBlock), xBlock.Name.LocalName);
		}

		protected int GetBlockLocalId(XElement xBlock)
		{
			return (int)xBlock.Attribute("localId");
		}
	}
}