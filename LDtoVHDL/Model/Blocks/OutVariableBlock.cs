using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Model.Blocks
{
	public interface IOutVariableBlock : IVariableBlock
	{
		Port MemoryOutput { get; }
		Port WriteCondition { get; }
	}
	public class OutVariableBlock : VariableBlock, IOutVariableBlock
	{
		private Port m_writeCondition;
		public const string TYPE = "outVariable";
		public OutVariableBlock(string id, string variableName, SignalType signalType)
			: base(id, variableName, signalType)
		{
			CreateOutputPort("MEM_OUT");
		}

		public Port Input { get { return Ports.Values.Single(port => port.Direction == PortDirection.Input); } }
		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
		public virtual Port WriteCondition 
		{ 
			get
			{
				if(m_writeCondition == null)
					ComputeWriteCondition();
				return m_writeCondition;
			}
		}

		public void ComputeWriteCondition()
		{
			m_writeCondition = Input.OtherSidePorts.Single().ParentBlock.Enable.OtherSidePorts.Single();
		}


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