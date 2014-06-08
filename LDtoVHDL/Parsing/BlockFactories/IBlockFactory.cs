using System.Collections.Generic;
using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	public interface IBlockFactory
	{
		IEnumerable<BaseBlock> CreateBlock(XElement xBlock, Program env);
	}
}