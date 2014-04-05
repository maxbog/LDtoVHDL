using System.IO;
using System.Linq;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(LeftRailBlock))]
	class LeftRailWriter : BaseBlockWriter
	{

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_LEFT_RAIL";
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
			foreach (var signal in block.Ports.Values.Select(port => port.ConnectedSignal))
				writer.WriteLine("    {0} <= '1';", ProgramWriter.GetSignalName(signal));
		}
	}
}