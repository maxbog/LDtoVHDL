namespace LDtoVHDL.Model.Blocks
{
	class ContactBlock : BaseBlock, IInVariableBlock
	{
		public const string TYPE = "contact";

		public ContactBlock(string id, string variableName)
			: base(id)
		{
			VariableName = variableName;
			CreateInputPort("MEM_IN");
		}

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
		public Port MemoryInput { get { return Ports["MEM_IN"]; } }
	}
}