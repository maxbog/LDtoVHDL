using System.IO;
using System.Linq;
using System.Text;
using LDtoVHDL.Blocks;

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
				Writer.WriteLine("    {0} <= {1};", busCreator.Output.ConnectedSignal.VhdlName,
					busCreator.Ports.Single(port => port.Key.StartsWith("IN")).Value.ConnectedSignal.VhdlName);
			}
			else
			{
				var builder = new StringBuilder();
				var startingBit = 0;
				foreach (var port in busCreator.Ports.Where(port => port.Key.StartsWith("IN")))
				{
					var endingBit = startingBit + port.Value.SignalType.Width - 1;
					builder.AppendFormat("{0}({1} to {2}) <= {3}; ", busCreator.Output.ConnectedSignal.VhdlName, startingBit, endingBit,
						port.Value.ConnectedSignal.VhdlName);
					startingBit += port.Value.SignalType.Width;
				}
				Writer.WriteLine(builder);
			}
		}
	}
}