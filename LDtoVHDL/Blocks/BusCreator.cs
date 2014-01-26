using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LDtoVHDL.Blocks
{
	class BusCreator : InternalBlock
	{
		public BusCreator(IEnumerable<Port> connectedPorts)
		{
			int i = 1;
			foreach (var connectedPort in connectedPorts)
			{
				var inPort = CreateInputPort("IN{0}", i++);
				inPort.Connect(connectedPort);
			}
			CreateOutputPort("OUT");
		}

		public Port Output { get { return Ports["OUT"]; }}

		public override string VhdlCode
		{
			get
			{
				if (Output.ConnectedSignal == null)
					return "";
				if (Output.Width == 1)
					return string.Format("{0} <= {1};", Output.ConnectedSignal.VhdlName, Ports.Single(port => port.Key.StartsWith("IN")).Value.ConnectedSignal.VhdlName);
				var builder = new StringBuilder();
				var startingBit = 0;
				foreach (var port in Ports.Where(port => port.Key.StartsWith("IN")))
				{
					var endingBit = startingBit + port.Value.Width-1;
					builder.AppendFormat("{0}({1} to {2}) <= {3}; ", Output.ConnectedSignal.VhdlName, startingBit, endingBit, port.Value.ConnectedSignal.VhdlName);
					startingBit += port.Value.Width;
				}
				return builder.ToString();
			}
		}

		public const string TYPE = "_bus_creator";
		public override bool CanComputePortWidths
		{
			get { return Ports.Where(port => port.Key.StartsWith("IN")).All(port => port.Value.Width != 0); }
		}

		public override void ComputePortWidths()
		{
			Output.Width = Ports.Values.Where(port => port.Name.StartsWith("IN")).Aggregate(0, (agg, port) => agg + port.Width);
		}
	}
}