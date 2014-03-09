using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public interface IBlockFactory
	{
		BaseBlock CreateBlock(XElement xBlock, Environment env);
	}
}