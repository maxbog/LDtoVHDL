using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Model.Blocks
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
		public IEnumerable<Port> Inputs { get { return Ports.Values.Where(port => port.Name.StartsWith("IN")); }}
		
		public const string TYPE = "_bus_creator";
		public override bool CanComputePortTypes
		{
			get { return Ports.Where(port => port.Key.StartsWith("IN")).All(port => port.Value.SignalType != null); }
		}

		public override void ComputePortTypes()
		{
			var firstType = Inputs.First().SignalType;
			Debug.Assert(Inputs.Skip(1).All(p => p.SignalType == firstType));
			Output.SignalType = new BusType(firstType, Inputs.Count());
		}
	}
}