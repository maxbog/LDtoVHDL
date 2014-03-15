using System.Xml.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Parsing.BlockFactories
{
	public interface IBlockFactory
	{
		BaseBlock CreateBlock(XElement xBlock, Program env);
	}
}