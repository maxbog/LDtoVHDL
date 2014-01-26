using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LDtoVHDL.Blocks
{
	public class InternalBlock : BaseBlock
	{
		public static int NextId;
		public InternalBlock(string type)
			: base(GetNextId(), type)
		{
		}

		public static string GetNextId()
		{
			return (NextId++).ToString(CultureInfo.InvariantCulture);
		}
	}

	class BusCreator : InternalBlock
	{
		public BusCreator(IEnumerable<Port> connectedPorts) : base(BUS_CREATOR)
		{
			int i = 0;
			foreach (var connectedPort in connectedPorts)
			{
				var inPort = new Port(PortDirection.Input, string.Format("IN{0}", i++));
				inPort.Connect(connectedPort);
				AddPort(inPort);
			}
			AddPort(new Port(PortDirection.Output, "OUT"));
		}

		public Port Output { get { return Ports["OUT"]; }}

		public override string VhdlCode
		{
			get
			{
				if (Output.ConnectedSignal == null)
					return "";
				var builder = new StringBuilder();
				var startingBit = 0;
				foreach (var port in Ports.Where(port => port.Key.StartsWith("IN")))
				{
					var endingBit = startingBit + port.Value.SignalWidth;
					builder.AppendFormat("{0}({1} to {2}) <= {3};\n", Output.ConnectedSignal.VhdlName, startingBit, endingBit, port.Value.ConnectedSignal.VhdlName);
					startingBit += port.Value.SignalWidth;
				}
				return builder.ToString();
			}
		}
	}

	class VarSelector : InternalBlock
	{
		public VarSelector() : base(VAR_SELECTOR)
		{
			AddPort(new Port(PortDirection.Output, "OUT"));
			AddPort(new Port(PortDirection.Input, "IN"));
			AddPort(new Port(PortDirection.Input, "CONTROL"));
			AddPort(new Port(PortDirection.Input, "MEMORY_IN"));
		}

		public Port Output { get { return Ports["OUT"]; } }
		public Port Controls { get { return Ports["CONTROL"]; } }
		public Port Inputs { get { return Ports["IN"]; } }
		public Port MemoryInput { get { return Ports["MEMORY_IN"]; } }
	}

	public class PowerOrBlock : InternalBlock
	{
		public PowerOrBlock()
			: base(POWER_OR)
		{
			AddPort(new Port(PortDirection.Output, "OUT"));
		}

		public Port Output { get { return Ports["OUT"]; } }
		public IEnumerable<Port> Inputs { get { return Ports.Values.Where(port => port.Direction == PortDirection.Input); } }

		public override string VhdlCode
		{
			get
			{
				var inputSignals = string.Join(" or ", Inputs.Select(port => port.ConnectedSignal.VhdlName));
				return string.Format("{0} <= {1};", Output.ConnectedSignal.VhdlName, inputSignals);
			}
		}
	}
}