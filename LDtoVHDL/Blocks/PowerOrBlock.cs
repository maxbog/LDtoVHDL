using System;
using System.Collections.Generic;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public class PowerOrBlock : InternalBlock
	{
		public PowerOrBlock()
		{
			CreateOutputPort("OUT");
		}

		public Port Output { get { return Ports["OUT"]; } }
		public IEnumerable<Port> Inputs { get { return Ports.Values.Where(port => port.Direction == PortDirection.Input); } }

		public override string VhdlCode
		{
			get
			{
				var inputSignals = String.Join(" or ", Inputs.Select(port => port.ConnectedSignal.VhdlName));
				return String.Format("{0} <= {1};", Output.ConnectedSignal.VhdlName, inputSignals);
			}
		}

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			foreach (var port in Ports.Values)
				port.SignalType = BuiltinType.Boolean;
		}

		public const string TYPE = "_power_or";

		private int m_nextInPortIndex;
		public void AddOrredSignal(Signal orredSignal)
		{
			var port = CreateInputPort("IN{0}", m_nextInPortIndex++);
			port.Connect(orredSignal.InputPort);
		}
	}
}