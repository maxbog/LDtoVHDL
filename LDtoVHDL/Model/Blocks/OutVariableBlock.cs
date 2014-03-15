using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Model.Blocks
{
	public interface IOutVariableBlock : IVariableBlock
	{
		Port MemoryOutput { get; }
		Signal WriteCondition { get; }
	}
	public class OutVariableBlock : VariableBlock, IOutVariableBlock
	{
		public const string TYPE = "outVariable";
		public OutVariableBlock(string id, string variableName, SignalType signalType)
			: base(id, variableName, signalType)
		{
			CreateOutputPort("MEM_OUT");
		}

		public Port Input { get { return Ports.Values.Single(port => port.Direction == PortDirection.Input); } }
		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
		public virtual Signal WriteCondition { get { return Input.OtherSidePorts.Single().ParentBaseBlock.Enable.ConnectedSignal; }}

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			foreach (var port in Ports.Values)
				port.SignalType = SignalType;
		}

		protected override string GetNewPortName(PortDirection direction)
		{
			Debug.Assert(direction == PortDirection.Input);
			return "IN";
		}
	}
}