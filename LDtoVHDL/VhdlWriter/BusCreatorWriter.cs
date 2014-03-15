using System.IO;
using System.Linq;
using System.Text;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(BusCreator))]
	class BusCreatorWriter : BaseBlockWriter
	{
		public BusCreatorWriter(TextWriter writer) : base(writer)
		{
		}

		public override void WriteCode(BaseBlock block)
		{
			var busCreator = (BusCreator) block;
			if (busCreator.Output.ConnectedSignal == null)
				return;
			if (busCreator.Output.SignalType.Width == 1)
			{
				Writer.WriteLine("    {0} <= {1};", string.Format("signal_{0}", busCreator.Output.ConnectedSignal.Hash),
					string.Format("signal_{0}", busCreator.Ports.Single(port => port.Key.StartsWith("IN")).Value.ConnectedSignal.Hash));
			}
			else
			{
				var builder = new StringBuilder();
				var startingBit = 0;
				foreach (var port in busCreator.Ports.Where(port => port.Key.StartsWith("IN")))
				{
					var endingBit = startingBit + port.Value.SignalType.Width - 1;
					builder.AppendFormat("{0}({1} to {2}) <= {3}; ", string.Format("signal_{0}", busCreator.Output.ConnectedSignal.Hash), startingBit, endingBit,
						string.Format("signal_{0}", port.Value.ConnectedSignal.Hash));
					startingBit += port.Value.SignalType.Width;
				}
				Writer.WriteLine(builder);
			}
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_BUS_CREATOR";
		}
	}
}