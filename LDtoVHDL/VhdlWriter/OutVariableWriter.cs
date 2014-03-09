using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(OutVariableBlock))]
	class OutVariableWriter : BaseBlockWriter
	{
		public OutVariableWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_OUT_VARIABLE";
		}
	}
}