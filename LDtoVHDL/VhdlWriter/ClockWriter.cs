using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ClockBlock))]
	class ClockWriter : BaseBlockWriter
	{
		public ClockWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CLOCK";
		}

		public override void WriteCode(BaseBlock block)
		{
			var clockBlock = (ClockBlock) block;
			Writer.WriteLine("{0} <= CLK;", ProgramWriter.GetSignalName(clockBlock.ClockOut.ConnectedSignal));
		}
	}
}