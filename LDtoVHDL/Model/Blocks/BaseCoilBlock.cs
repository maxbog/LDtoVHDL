﻿namespace LDtoVHDL.Model.Blocks
{
	public abstract class BaseCoilBlock : BaseBlock, IOutVariableBlock
	{
		protected BaseCoilBlock(string id, string variableName)
			: base(id)
		{
			CreateOutputPort("MEM_OUT");
			CreateOutputPort("WRITE_CONDITION");
			VariableName = variableName;
		}

		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
		public Port WriteCondition { get { return Ports["WRITE_CONDITION"]; } }

		protected override string GetNewPortName(PortDirection direction)
		{
			return direction == PortDirection.Input ? "EN" : "ENO";
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

		public string VariableName { get; private set; }
	}

	class CoilBlock : BaseCoilBlock
	{
		public CoilBlock(string id, string variableName) : base(id, variableName)
		{
		}
	}

	class NegatedCoilBlock : BaseCoilBlock
	{
		public NegatedCoilBlock(string id, string variableName) : base(id, variableName)
		{
		}
	}
}