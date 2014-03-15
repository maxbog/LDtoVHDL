using System.IO;
using System.Linq;
using LDtoVHDL.Model.Blocks;

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
			var inputSignals = string.Join(" or ", powerOr.Inputs.Select(port => string.Format("signal_{0}", port.ConnectedSignal.Hash)));
			Writer.WriteLine("{0} <= {1};", string.Format("signal_{0}", powerOr.Output.ConnectedSignal.Hash), inputSignals);
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_POWER_OR";
		}
	}
}