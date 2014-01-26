using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public class OutVariableBlock : VariableBlock
	{
		public const string TYPE = "outVariable";
		public OutVariableBlock(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth)
		{
			CreateOutputPort("MEM_OUT");
		}

		public Port Input { get { return Ports.Values.Single(port => port.Direction == PortDirection.Input); } }
		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
		public virtual Signal WriteCondition { get { return Input.OtherSidePorts.Single().ParentBaseBlock.Enable.ConnectedSignal; }}

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
			Debug.Assert(direction == PortDirection.Input);
			return "IN";
		}
	}
}