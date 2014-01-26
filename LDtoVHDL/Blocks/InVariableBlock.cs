using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public class InVariableBlock : VariableBlock
	{
		public InVariableBlock(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth)
		{
			CreateInputPort("MEM_IN");
		}

		public Port Output { get { return Ports.Values.Single(port => port.Direction == PortDirection.Output); } }
		public Port MemoryInput { get { return Ports["MEM_IN"]; } }
		public const string TYPE = "inVariable";

		public override bool CanComputePortWidths
		{
			get { return true; }
		}

		public override void ComputePortWidths()
		{
			foreach (var port in Ports.Values)
			{
				port.Width = SignalWidth;
			}
		}

		protected override string GetNewPortName(PortDirection direction)
		{
			Debug.Assert(direction == PortDirection.Output);
			return "OUT";
		}
	}
}