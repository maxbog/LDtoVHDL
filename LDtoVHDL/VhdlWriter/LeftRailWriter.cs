using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(LeftRailBlock))]
	class LeftRailWriter : BaseBlockWriter
	{
		public LeftRailWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_LEFT_RAIL";
		}
	}
}