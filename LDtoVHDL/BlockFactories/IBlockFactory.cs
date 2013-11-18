using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public interface IBlockFactory
	{
		IEnumerable<string> BlockTypes { get; }
		BaseBlock CreateBlock(XElement xBlock);
	}
}