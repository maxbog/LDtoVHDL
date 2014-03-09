using System.IO;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(PowerOrBlock))]
	class PowerOrWriter : BaseBlockWriter
	{
		public PowerOrWriter(TextWriter writer)
			: base(writer)
		{
		}

		public override void WriteCode(BaseBlock block)
		{
			var powerOr = (PowerOrBlock) block;
			var inputSignals = string.Join(" or ", powerOr.Inputs.Select(port => port.ConnectedSignal.VhdlName));
			Writer.WriteLine("{0} <= {1};", powerOr.Output.ConnectedSignal.VhdlName, inputSignals);
		}
	}
}