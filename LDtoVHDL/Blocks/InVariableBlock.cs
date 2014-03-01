using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public interface IInVariableBlock : IVariableBlock
	{
		Port MemoryInput { get; }
	}

	public class InVariableBlock : VariableBlock, IInVariableBlock
	{
		public InVariableBlock(string id, string variableName, SignalType signalType)
			: base(id, variableName, signalType)
		{
			CreateInputPort("MEM_IN");
		}

		public Port Output { get { return Ports.Values.Single(port => port.Direction == PortDirection.Output); } }
		public Port MemoryInput { get { return Ports["MEM_IN"]; } }
		public const string TYPE = "inVariable";

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
			Debug.Assert(direction == PortDirection.Output);
			return "OUT";
		}
	}
}