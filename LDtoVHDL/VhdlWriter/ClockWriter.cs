using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ClockBlock))]
	class ClockWriter : BaseBlockWriter
	{
		public ClockWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CLOCK";
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
			var clockBlock = (ClockBlock) block;
			writer.WriteLine("    {0} <= CLK;", ProgramWriter.GetSignalName(clockBlock.ClockOut.ConnectedSignal));
		}
	}
}