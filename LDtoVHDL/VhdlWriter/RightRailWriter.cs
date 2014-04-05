using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(RightRailBlock))]
	class RightRailWriter : BaseBlockWriter
	{
		public RightRailWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_RIGHT_RAIL";
		}

		public override void WriteCode(BaseBlock block)
		{
		}
	}
}