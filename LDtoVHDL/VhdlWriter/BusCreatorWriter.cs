using System.IO;
using System.Linq;
using System.Text;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(BusCreator))]
	class BusCreatorWriter : BaseBlockWriter
	{
		public BusCreatorWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
			var busCreator = (BusCreator) block;
			if (busCreator.Output.ConnectedSignal == null)
				return;
			var outputPortName = ProgramWriter.GetSignalName(busCreator.Output.ConnectedSignal);
			var inputPorts = busCreator.Ports.Where(port => port.Key.StartsWith("IN")).Select(port => port.Value);

			var builder = new StringBuilder();

			foreach (var pair in inputPorts.Select((port, idx) => new {Signal = port.ConnectedSignal, Idx = idx}))
				builder.AppendFormat("    {0}({1}) <= {2};", outputPortName, pair.Idx, ProgramWriter.GetSignalName(pair.Signal));

			writer.WriteLine(builder);
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_BUS_CREATOR";
		}
	}
}