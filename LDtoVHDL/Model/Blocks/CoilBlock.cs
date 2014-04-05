namespace LDtoVHDL.Model.Blocks
{
	public class CoilBlock : BaseBlock, IOutVariableBlock
	{
		public const string TYPE = "coil";
		public CoilBlock(string id, string variableName)
			: base(id)
		{
			CreateOutputPort("MEM_OUT");
			VariableName = variableName;
		}

		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
		public Signal WriteCondition { get { return Enable.ConnectedSignal; } }

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
}